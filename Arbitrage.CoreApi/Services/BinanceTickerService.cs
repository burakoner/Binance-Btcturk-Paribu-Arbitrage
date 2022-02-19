using Arbitrage.CoreApi.BaseStructure;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiter;
using Gizza.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arbitrage.CoreApi.Services
{
    public class BinanceTickerService : BaseService
    {
        private AppCache AppCache { get; set; }
        private DateTime SpotTickerLastUpdate { get; set; }
        private BinanceClient BinanceClient { get; set; }
        private BinanceSocketClient BinanceSocketClient { get; set; }

        private List<string> MarketsToListen { get; set; }

        public BinanceTickerService(AppCache appCache)
        {
            AppCache = appCache;
            SpotTickerLastUpdate = DateTime.MinValue;
            BinanceClient = new BinanceClient(new BinanceClientOptions
            {
                // Whether or not to automatically sync the local time with the server time
                // Default: true
                AutoTimestamp = true,

                // Interval for refreshing the auto timestamp calculation
                // Default: 3 Hours
                AutoTimestampRecalculationInterval = TimeSpan.FromMinutes(180),

                // LogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
                LogLevel = LogLevel.Warning,

                // Rate Limiters
                RateLimiters = new List<IRateLimiter>
                {
                    // 1 Saniyede 10 istek kabul ediyor. Ben 9 gönderiyorum.
                    // new CryptoExchange.Net.RateLimiter.RateLimiterTotal(9, TimeSpan.FromSeconds(1)),

                    // 60 Saniyede 1200 istek kabul ediyor. Ben 1100 gönderiyorum.
                    new RateLimiterTotal(1100, TimeSpan.FromMilliseconds(60*1000)),
                    
                    // 24 Saatte 100.000 istek kabul ediyor. Ben 99.000 gönderiyorum.
                    // new CryptoExchange.Net.RateLimiter.RateLimiterTotal(99000, TimeSpan.FromHours(24)),
                },

                // Rate Limiting Behaviour
                RateLimitingBehaviour = RateLimitingBehaviour.Wait,

                // The default receive window for requests
                // Default: 5 Seconds
                ReceiveWindow = TimeSpan.FromSeconds(55),

                // The time the server has to respond to a request before timing out
                // Default: 30 Seconds
                RequestTimeout = TimeSpan.FromSeconds(15),

                // Whether to check the trade rules when placing new orders and what to do if the trade isn't valid
                // Default: Binance.Net.Objects.TradeRulesBehaviour.None
                TradeRulesBehaviour = TradeRulesBehaviour.AutoComply,

                // How often the trade rules should be updated. Only used when TradeRulesBehaviour is not None
                // Default: 60 Minutes
                TradeRulesUpdateInterval = TimeSpan.FromMinutes(60),
            });
            BinanceSocketClient = new BinanceSocketClient(new BinanceSocketClientOptions
            {
                AutoReconnect = true,
                ReconnectInterval = TimeSpan.FromSeconds(10),
            });

            MarketsToListen = AppCache
                .GetExchangeMarkets()
                .Where(x =>
                    x.BINANCE_SYMBOL.IsNotNullOrEmpty() &&
                    AppCache.BinanceTickers.Keys.Contains(x.SYMBOL))
                .Select(x => x.BINANCE_SYMBOL)
                .OrderBy(x => x)
                .ToList();
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            // Wait 10 seconds for the first turn
            await Task.Delay(TimeSpan.FromSeconds(10), ct);

            // Rest Api Spot Tickers
            Thread t01 = new Thread(RestApiSpotTickers)
            {
                Name = "Binance Rest-Api Spot Thread"
            };
            t01.Start();

            // WebSocket Api Spot Tickers
            Thread t02 = new Thread(WSApiSpotTickers)
            {
                Name = "Binance WebSocket-Api Spot Thread"
            };
            t02.Start();
        }

        private async void RestApiSpotTickers()
        {
            while (true)
            {
                try
                {
                    var tickers = await BinanceClient.Spot.Market.GetTickersAsync();
                    if (tickers.Success && tickers.Data != null && tickers.Data.Count() > 0)
                    {
                        foreach (Models.Exchange.ExchangeTicker exchangeTicker in AppCache.BinanceTickers.Values)
                        {
                            Binance.Net.Interfaces.IBinanceTick ticker = tickers.Data.FirstOrDefault(x => x.Symbol == exchangeTicker.Symbol);
                            if (ticker == null)
                            {
                                continue;
                            }

                            if (ticker.Symbol == "USDTTRY")
                            {
                                AppCache.USDTTRY[Enums.ExchangePlatform.Binance].AskPrice = ticker.AskPrice;
                                AppCache.USDTTRY[Enums.ExchangePlatform.Binance].AskQuantity = ticker.AskQuantity;
                                AppCache.USDTTRY[Enums.ExchangePlatform.Binance].BidPrice = ticker.BidPrice;
                                AppCache.USDTTRY[Enums.ExchangePlatform.Binance].BidQuantity = ticker.BidQuantity;
                            }

                            lock (AppCache.lock_BinanceTickers)
                            {
                                exchangeTicker.AskPrice = ticker.AskPrice;
                                exchangeTicker.AskQuantity = ticker.AskQuantity;
                                exchangeTicker.BidPrice = ticker.BidPrice;
                                exchangeTicker.BidQuantity = ticker.BidQuantity;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Exception a = ex;
                }
                finally
                {
                    // Set Flag
                    SpotTickerLastUpdate = AppStatic.Now;

                    // Wait 30 seconds for the next turn
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            }
        }

        private async void WSApiSpotTickers()
        {
            await BinanceSocketClient.Spot.SubscribeToAllSymbolTickerUpdatesAsync((data) =>
            {
                // Check Point
                if ((AppStatic.Now - SpotTickerLastUpdate) < TimeSpan.FromSeconds(3))
                {
                    return;
                }

                // Set Tickers
                try
                {
                    foreach (Models.Exchange.ExchangeTicker exchangeTicker in AppCache.BinanceTickers.Values)
                    {
                        Binance.Net.Interfaces.IBinanceTick ticker = data.Data.FirstOrDefault(x => x.Symbol == exchangeTicker.Symbol);
                        if (ticker == null)
                        {
                            continue;
                        }

                        if (ticker.Symbol == "USDTTRY")
                        {
                            AppCache.USDTTRY[Enums.ExchangePlatform.Binance].AskPrice = ticker.AskPrice;
                            AppCache.USDTTRY[Enums.ExchangePlatform.Binance].AskQuantity = ticker.AskQuantity;
                            AppCache.USDTTRY[Enums.ExchangePlatform.Binance].BidPrice = ticker.BidPrice;
                            AppCache.USDTTRY[Enums.ExchangePlatform.Binance].BidQuantity = ticker.BidQuantity;
                        }

                        lock (AppCache.lock_BinanceTickers)
                        {
                            exchangeTicker.AskPrice = ticker.AskPrice;
                            exchangeTicker.AskQuantity = ticker.AskQuantity;

                            exchangeTicker.BidPrice = ticker.BidPrice;
                            exchangeTicker.BidQuantity = ticker.BidQuantity;
                        }
                    }

                    // GC
                    data = null;
                }
                catch (Exception ex)
                {
                    Exception a = ex;
                }
                finally
                {
                    // Set Flag
                    SpotTickerLastUpdate = AppStatic.Now;
                }
            });
        }

    }
}