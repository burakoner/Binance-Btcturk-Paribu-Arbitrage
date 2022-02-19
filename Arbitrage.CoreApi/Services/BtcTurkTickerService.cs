using Arbitrage.CoreApi.BaseStructure;
using BtcTurk.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiter;
using Gizza.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arbitrage.CoreApi.Services
{
    public class BtcTurkTickerService : BaseService
    {
        private AppCache AppCache { get; set; }
        private DateTime SpotTickerLastUpdate { get; set; }
        private BtcTurkClient BtcTurkClient { get; set; }
        private BtcTurkSocketClient BtcTurkSocketClient { get; set; }

        private List<string> MarketsToListen { get; set; }

        public BtcTurkTickerService(AppCache appCache)
        {
            AppCache = appCache;
            SpotTickerLastUpdate = DateTime.MinValue;
            BtcTurkClient = new BtcTurkClient(new BtcTurkClientOptions
            {
                // LogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
                LogLevel = LogLevel.Warning,

                // Rate Limiters
                RateLimiters = new List<IRateLimiter>
                {
                    // 60 Saniyede 600 istek kabul ediyor. Ben 500 gönderiyorum.
                    new RateLimiterTotal(500, TimeSpan.FromMilliseconds(60*1000)),
                },

                // Rate Limiting Behaviour
                RateLimitingBehaviour = RateLimitingBehaviour.Wait,

                // The time the server has to respond to a request before timing out
                // Default: 30 Seconds
                RequestTimeout = TimeSpan.FromSeconds(15),
            });
            BtcTurkSocketClient = new BtcTurkSocketClient(new BtcTurkSocketClientOptions
            {
                AutoReconnect = true,
                ReconnectInterval = TimeSpan.FromSeconds(10),
            });

            MarketsToListen = AppCache
                .GetExchangeMarkets()
                .Where(x =>
                    x.BTCTURK_SYMBOL.IsNotNullOrEmpty() &&
                    AppCache.BtcTurkTickers.Keys.Contains(x.SYMBOL))
                .Select(x => x.BTCTURK_SYMBOL)
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
                Name = "BtcTurk Rest-Api Spot Thread"
            };
            t01.Start();

            // WebSocket Api Spot Tickers
            Thread t02 = new Thread(WSApiSpotTickers)
            {
                Name = "BtcTurk WebSocket-Api Spot Thread"
            };
            t02.Start();
        }

        private async void RestApiSpotTickers()
        {
            while (true)
            {
                try
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    foreach (string market in MarketsToListen)
                    {
                        var orderbook = await BtcTurkClient.GetOrderBookAsync(market, 1);
                        if (orderbook.Success && orderbook.Data != null)
                        {
                            if (orderbook.Data.Asks == null || orderbook.Data.Asks.Length == 0)
                                continue;

                            if (orderbook.Data.Bids == null || orderbook.Data.Bids.Length == 0)
                                continue;

                            var exchangeTicker = AppCache.BtcTurkTickers.Values.FirstOrDefault(x => x.Symbol == market);
                            if (exchangeTicker == null)
                                continue;

                            // ASK: Satış Emirleri
                            // BID: Alış Emirleri
                            var bestAsk = orderbook.Data.Asks.FirstOrDefault();
                            var bestBid = orderbook.Data.Bids.FirstOrDefault();

                            if (market == "USDTTRY")
                            {
                                AppCache.USDTTRY[Enums.ExchangePlatform.BtcTurk].AskPrice = bestAsk.Price;
                                AppCache.USDTTRY[Enums.ExchangePlatform.BtcTurk].AskQuantity = bestAsk.Quantity;
                                AppCache.USDTTRY[Enums.ExchangePlatform.BtcTurk].BidPrice = bestBid.Price;
                                AppCache.USDTTRY[Enums.ExchangePlatform.BtcTurk].BidQuantity = bestBid.Quantity;
                            }

                            lock (AppCache.lock_BtcTurkTickers)
                            {
                                exchangeTicker.AskPrice = bestAsk.Price;
                                exchangeTicker.AskQuantity = bestAsk.Quantity;
                                exchangeTicker.BidPrice = bestBid.Price;
                                exchangeTicker.BidQuantity = bestBid.Quantity;
                            }
                        }
                    }
                    sw.Stop();
                    Debug.WriteLine("BtcTurk loop completed in " + sw.Elapsed);
                }
                catch (Exception ex)
                {
                    Exception a = ex;
                }
                finally
                {
                    // Set Flag
                    SpotTickerLastUpdate = AppStatic.Now;

                    // Wait 5 seconds for the next turn
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }

        private async void WSApiSpotTickers()
        {
            await BtcTurkSocketClient.SubscribeToTickersAsync((data) =>
            {
                // Check Point
                if ((AppStatic.Now - SpotTickerLastUpdate) < TimeSpan.FromSeconds(3))
                {
                    return;
                }

                // Set Tickers
                try
                {
                    foreach (var exchangeTicker in AppCache.BtcTurkTickers.Values)
                    {
                        var ticker = data.Items.FirstOrDefault(x => x.PairSymbol == exchangeTicker.Symbol);
                        if (ticker == null)
                        {
                            continue;
                        }

                        if (ticker.PairSymbol == "USDTTRY")
                        {
                            AppCache.USDTTRY[Enums.ExchangePlatform.BtcTurk].AskPrice = ticker.Ask;
                            // AppCache.USDTTRY[Enums.ExchangePlatform.BtcTurk].AskQuantity = ticker.AskQuantity;
                            AppCache.USDTTRY[Enums.ExchangePlatform.BtcTurk].BidPrice = ticker.Bid;
                            // AppCache.USDTTRY[Enums.ExchangePlatform.BtcTurk].BidQuantity = ticker.BidQuantity;
                        }

                        lock (AppCache.BtcTurkTickers)
                        {
                            exchangeTicker.AskPrice = ticker.Ask;
                            // exchangeTicker.AskQuantity = ticker.AskQuantity;
                            exchangeTicker.BidPrice = ticker.Bid;
                            // exchangeTicker.BidQuantity = ticker.BidQuantity;
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