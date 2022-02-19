using Arbitrage.CoreApi.BaseStructure;
using BtcTurk.Net;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.RateLimiter;
using Gizza.Extensions;
using Microsoft.Extensions.Logging;
using Paribu.Net;
using Paribu.Net.CoreObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arbitrage.CoreApi.Services
{
    public class ParibuTickerService : BaseService
    {
        private AppCache AppCache { get; set; }
        private DateTime SpotTickerLastUpdate { get; set; }
        private ParibuClient ParibuClient { get; set; }
        private ParibuSocketClient ParibuSocketClient { get; set; }

        private List<string> MarketsToListen { get; set; }

        public ParibuTickerService(AppCache appCache)
        {
            AppCache = appCache;
            SpotTickerLastUpdate = DateTime.MinValue;
            ParibuClient = new ParibuClient(new ParibuClientOptions
            {
                // LogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
                LogLevel = LogLevel.Warning,

                // Rate Limiters
                RateLimiters = new List<IRateLimiter>
                {
                    // Limitleri Bilmiyorum. Ben de saniyede en fazla 5 istek göndereceğim
                    new RateLimiterTotal(5, TimeSpan.FromMilliseconds(1000)),
                },

                // Rate Limiting Behaviour
                RateLimitingBehaviour = RateLimitingBehaviour.Wait,

                // The time the server has to respond to a request before timing out
                // Default: 30 Seconds
                RequestTimeout = TimeSpan.FromSeconds(15),
            });
            ParibuSocketClient = new ParibuSocketClient(new ParibuSocketClientOptions
            {
                AutoReconnect = true,
                ReconnectInterval = TimeSpan.FromSeconds(10),
            });

            MarketsToListen = AppCache
                .GetExchangeMarkets()
                .Where(x =>
                    x.PARIBU_SYMBOL.IsNotNullOrEmpty() &&
                    AppCache.ParibuTickers.Keys.Contains(x.SYMBOL))
                .Select(x => x.PARIBU_SYMBOL)
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
                Name = "Paribu Rest-Api Spot Thread"
            };
            t01.Start();

            // WebSocket Api Spot Tickers
            var t02 = new Thread(this.WSApiSpotTickers);
            t02.Name = "Paribu WebSocket-Api Spot Thread";
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
                        WebCallResult<Paribu.Net.RestObjects.ParibuMarketData> marketdata = await ParibuClient.GetMarketDataAsync(market);
                        if (marketdata.Success && marketdata.Data != null)
                        {
                            Paribu.Net.RestObjects.ParibuOrderBook orderbook = marketdata.Data.OrderBook;
                            if (orderbook == null)
                            {
                                continue;
                            }

                            if (orderbook.Asks == null || orderbook.Asks.Count == 0)
                            {
                                continue;
                            }

                            if (orderbook.Bids == null || orderbook.Bids.Count == 0)
                            {
                                continue;
                            }

                            Models.Exchange.ExchangeTicker exchangeTicker = AppCache.ParibuTickers.Values.FirstOrDefault(x => x.Symbol == market);
                            if (exchangeTicker == null)
                            {
                                continue;
                            }

                            // ASK: Satış Emirleri
                            // BID: Alış Emirleri
                            Paribu.Net.RestObjects.ParibuOrderBookEntry bestAsk = orderbook.Asks.OrderBy(x => x.Price).FirstOrDefault();
                            Paribu.Net.RestObjects.ParibuOrderBookEntry bestBid = orderbook.Bids.OrderByDescending(x => x.Price).FirstOrDefault();

                            if (market == "usdt-tl")
                            {
                                AppCache.USDTTRY[Enums.ExchangePlatform.Paribu].AskPrice = bestAsk.Price;
                                AppCache.USDTTRY[Enums.ExchangePlatform.Paribu].AskQuantity = bestAsk.Amount;
                                AppCache.USDTTRY[Enums.ExchangePlatform.Paribu].BidPrice = bestBid.Price;
                                AppCache.USDTTRY[Enums.ExchangePlatform.Paribu].BidQuantity = bestBid.Amount;
                            }

                            lock (AppCache.lock_ParibuTickers)
                            {
                                // ASK: Satış Emirleri
                                exchangeTicker.AskPrice = bestAsk.Price;
                                exchangeTicker.AskQuantity = bestAsk.Amount;

                                // BID: Alış Emirleri
                                exchangeTicker.BidPrice = bestBid.Price;
                                exchangeTicker.BidQuantity = bestBid.Amount;
                            }
                        }
                    }
                    sw.Stop();
                    Debug.WriteLine("Paribu loop completed in " + sw.Elapsed);
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
            await ParibuSocketClient.SubscribeToTickersAsync((ticker) =>
            {
                // Check Point
                if ((AppStatic.Now - SpotTickerLastUpdate) < TimeSpan.FromSeconds(3))
                {
                    return;
                }

                // Set Tickers
                try
                {
                    if (ticker == null)
                        return;

                    var exchangeTicker = AppCache.ParibuTickers.Values.FirstOrDefault(x => x.Symbol == ticker.Symbol);
                    if (exchangeTicker == null)
                        return;

                    if (ticker.Symbol == "usdt-tl")
                    {
                        if (ticker.Ask.HasValue) AppCache.USDTTRY[Enums.ExchangePlatform.Paribu].AskPrice = ticker.Ask.Value;
                        // AppCache.USDTTRY[Enums.ExchangePlatform.Paribu].AskQuantity = ticker.AskQuantity;
                        if (ticker.Bid.HasValue) AppCache.USDTTRY[Enums.ExchangePlatform.Paribu].BidPrice = ticker.Bid.Value;
                        // AppCache.USDTTRY[Enums.ExchangePlatform.Paribu].BidQuantity = ticker.BidQuantity;
                    }

                    lock (AppCache.lock_ParibuTickers)
                    {
                        if (ticker.Ask.HasValue) exchangeTicker.AskPrice = ticker.Ask.Value;
                        // exchangeTicker.AskQuantity = ticker.AskQuantity;
                        if (ticker.Bid.HasValue) exchangeTicker.BidPrice = ticker.Bid.Value;
                        // exchangeTicker.BidQuantity = ticker.BidQuantity;
                    }

                    // GC
                    ticker = null;
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
            }, (data) =>
            {
                // GC
                data = null;
            });
        }

    }
}