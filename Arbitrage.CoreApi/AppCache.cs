using Arbitrage.CoreApi.Database;
using Arbitrage.CoreApi.Database.Poco;
using Arbitrage.CoreApi.Enums;
using Arbitrage.CoreApi.Models.Exchange;
using Arbitrage.CoreApi.StreamApi.Exchange;
using Dapper;
using Gizza.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arbitrage.CoreApi
{
    public class AppCache
    {
        /* Memory Cache */
        public IMemoryCache MemoryCache { get; set; }
        public MemoryCacheEntryOptions MemoryCacheOptions { get; set; }
        public AppConnections AppConn { get; set; }
        public AppErrors AppErrors { get; set; }
        public AppSettings AppSettings { get; set; }

        /* Socket Handler */
        public SocketHandler SocketHandler { get; set; }

        /* Tickers */
        public Dictionary<ExchangePlatform, ExchangeTicker> USDTTRY { get; set; }
        public Dictionary<string, ExchangeTicker> BinanceTickers { get; set; }
        public Dictionary<string, ExchangeTicker> BtcTurkTickers { get; set; }
        public Dictionary<string, ExchangeTicker> ParibuTickers { get; set; }
        public Dictionary<string, ExchangeTracker> MarketTracker { get; set; }

        /* Cache Names */
        private string cacheName_ExcAssets { get; set; } = "ExcAssets";
        private string cacheName_ExcMarkets { get; set; } = "ExcMarkets";
        private string cacheName_UserData { get; set; } = "UserData";
        private string cacheName_UserTokens { get; set; } = "UserTokens";

        /* Lock Objects */
        public readonly object lock_BinanceTickers = new();
        public readonly object lock_BtcTurkTickers = new();
        public readonly object lock_ParibuTickers = new();
        public readonly object lock_MarketTracker = new();

        #region Constructor
        public AppCache(IMemoryCache imc, AppConnections appConn, AppErrors appErrors, AppSettings appSettings, SocketHandler socketHandler)
        {
            MemoryCache = imc;
            AppConn = appConn;
            AppErrors = appErrors;
            AppSettings = appSettings;

            MemoryCacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(10000),
                Priority = CacheItemPriority.NeverRemove
            };
            SocketHandler = socketHandler;
            SocketHandler.SetAppCache(this);

            /* Get Master Data */
            List<EXC_ASSET> assets = GetExchangeAssets();
            List<EXC_MARKET> markets = GetExchangeMarkets();

            /* UsdtTry Tickers */
            USDTTRY = new Dictionary<ExchangePlatform, ExchangeTicker>();
            USDTTRY.Add(ExchangePlatform.Binance, new ExchangeTicker { Symbol = "USDTTRY" });
            USDTTRY.Add(ExchangePlatform.BtcTurk, new ExchangeTicker { Symbol = "USDTTRY" });
            USDTTRY.Add(ExchangePlatform.Paribu, new ExchangeTicker { Symbol = "usdt-tl" });

            /* Binance Tickers */
            BinanceTickers = new Dictionary<string, ExchangeTicker>();
            foreach (EXC_MARKET market in markets.Where(x=>!x.BINANCE_SYMBOL.IsNullOrEmpty()))
                BinanceTickers.Add(market.SYMBOL, new ExchangeTicker { Symbol = market.BINANCE_SYMBOL });

            /* BtcTurk Tickers */
            BtcTurkTickers = new Dictionary<string, ExchangeTicker>();
            foreach (EXC_MARKET market in markets.Where(x=>!x.BTCTURK_SYMBOL.IsNullOrEmpty()))
                BtcTurkTickers.Add(market.SYMBOL, new ExchangeTicker { Symbol = market.BTCTURK_SYMBOL });

            /* Paribu Tickers */
            ParibuTickers = new Dictionary<string, ExchangeTicker>();
            foreach (EXC_MARKET market in markets.Where(x => !x.PARIBU_SYMBOL.IsNullOrEmpty()))
                ParibuTickers.Add(market.SYMBOL, new ExchangeTicker { Symbol = market.PARIBU_SYMBOL });

            /* Market Tracker */
            MarketTracker = new Dictionary<string, ExchangeTracker>();
            foreach (EXC_MARKET market in markets)
            {
                EXC_ASSET baseAsset = assets.FirstOrDefault(x => x.ID == market.BASE_ASSET_ID);
                if (baseAsset == null)
                    continue;

                EXC_ASSET quoteAsset = assets.FirstOrDefault(x => x.ID == market.QUOTE_ASSET_ID);
                if (quoteAsset == null)
                    continue;

                var any =
                market.BINANCE_TO_PARIBU_CLASSIC ||
                market.BINANCE_TO_PARIBU_CROSS ||
                market.BINANCE_TO_BTCTURK_CLASSIC ||
                market.BINANCE_TO_BTCTURK_CROSS ||
                market.PARIBU_TO_BINANCE_CLASSIC ||
                market.PARIBU_TO_BINANCE_CROSS ||
                market.PARIBU_TO_BTCTURK_CLASSIC ||
                market.PARIBU_TO_BTCTURK_CROSS ||
                market.BTCTURK_TO_BINANCE_CLASSIC ||
                market.BTCTURK_TO_BINANCE_CROSS ||
                market.BTCTURK_TO_PARIBU_CLASSIC ||
                market.BTCTURK_TO_PARIBU_CROSS;
                if (!any) continue;

                var tracker = new ExchangeTracker
                {
                    AppCache = this,
                    Market = market,
                    BaseAsset = baseAsset,
                    QuoteAsset = quoteAsset,
                };
                if (BinanceTickers.ContainsKey(market.SYMBOL)) tracker.BinanceTicker = BinanceTickers[market.SYMBOL];
                if (BtcTurkTickers.ContainsKey(market.SYMBOL)) tracker.BtcTurkTicker = BtcTurkTickers[market.SYMBOL];
                if (ParibuTickers.ContainsKey(market.SYMBOL)) tracker.ParibuTicker = ParibuTickers[market.SYMBOL];

                var binanceCrossTicker = BinanceTickers.Values.FirstOrDefault(x => x.Symbol == market.BINANCE_USDT_MARKET);
                if (binanceCrossTicker != null) tracker.BinanceCrossTicker = binanceCrossTicker;
                
                var btcturkCrossTicker = BtcTurkTickers.Values.FirstOrDefault(x => x.Symbol == market.BTCTURK_USDT_MARKET);
                if (btcturkCrossTicker != null) tracker.BtcTurkCrossTicker = btcturkCrossTicker;
                
                var paribuCrossTicker = ParibuTickers.Values.FirstOrDefault(x => x.Symbol == market.PARIBU_USDT_MARKET);
                if (paribuCrossTicker != null) tracker.ParibuCrossTicker = paribuCrossTicker;

                MarketTracker.Add(market.SYMBOL, tracker);
            }
        }
        #endregion

        #region Exchange
        // Asla Null Dönmez. En kötü ihtimal sıfır elemanlı bir liste döner
        public List<EXC_ASSET> GetExchangeAssets(bool forceReCache = false)
        {
            if (forceReCache || !MemoryCache.TryGetValue(cacheName_ExcAssets, out List<EXC_ASSET> cachedData))
            {
                // Get Data
                cachedData = AppConn.dbConn.GetConnection().Query<EXC_ASSET>("SELECT * FROM " + Tables.EXC_ASSETS + " ORDER BY SYMBOL").ToList();

                // Set Memory Cache
                MemoryCache.Set(cacheName_ExcAssets, cachedData, MemoryCacheOptions);
            }

            // Return
            return cachedData ?? new List<EXC_ASSET>();
        }

        // Asla Null Dönmez. En kötü ihtimal sıfır elemanlı bir liste döner
        public List<EXC_MARKET> GetExchangeMarkets(bool forceReCache = false)
        {
            if (forceReCache || !MemoryCache.TryGetValue(cacheName_ExcMarkets, out List<EXC_MARKET> cachedData))
            {
                // Get Data
                cachedData = AppConn.dbConn.GetConnection().Query<EXC_MARKET>("SELECT * FROM " + Tables.EXC_MARKETS + " WHERE " +
                    "    BINANCE_TO_PARIBU_CLASSIC =1 " +
                    " OR BINANCE_TO_PARIBU_CROSS   =1 " +
                    " OR BINANCE_TO_BTCTURK_CLASSIC=1 " +
                    " OR BINANCE_TO_BTCTURK_CROSS  =1 " +
                    " OR BTCTURK_TO_BINANCE_CLASSIC=1 " +
                    " OR BTCTURK_TO_BINANCE_CROSS  =1 " +
                    " OR BTCTURK_TO_PARIBU_CLASSIC =1 " +
                    " OR BTCTURK_TO_PARIBU_CROSS   =1 " +
                    " OR PARIBU_TO_BINANCE_CLASSIC =1 " +
                    " OR PARIBU_TO_BINANCE_CROSS   =1 " +
                    " OR PARIBU_TO_BTCTURK_CLASSIC =1 " +
                    " OR PARIBU_TO_BTCTURK_CROSS   =1 " +
                    " ORDER BY SYMBOL").ToList();

                // Set Memory Cache
                MemoryCache.Set(cacheName_ExcAssets, cachedData, MemoryCacheOptions);
            }

            // Return
            return cachedData ?? new List<EXC_MARKET>();
        }
        #endregion

        #region User
        // Null Dönebilir
        public USR_DATA GetUserData(int userId, bool forceReCache = false)
        {
            USR_DATA userdata = null;
            if (!MemoryCache.TryGetValue(cacheName_UserData, out Dictionary<int, USR_DATA> cachedData))
            {
                // Contruct Dictionary
                cachedData = new Dictionary<int, USR_DATA>();

                // Set Memory Cache
                MemoryCache.Set(cacheName_UserData, cachedData, MemoryCacheOptions);
            }

            if (forceReCache || !cachedData.ContainsKey(userId))
            {
                // Get Data
                userdata = AppConn.dbConn.GetConnection().Query<USR_DATA>("SELECT * FROM " + Tables.USR_DATA + " WHERE ID=" + userId).FirstOrDefault();
                cachedData[userId] = userdata;
            }
            else if (cachedData.ContainsKey(userId))
            {
                userdata = cachedData[userId];
            }

            // Return
            return userdata;
        }

        // Null Dönebilir
        public USR_TOKEN GetUserToken(string token, bool forceReCache = false)
        {
            USR_TOKEN usertoken = null;
            if (!MemoryCache.TryGetValue(cacheName_UserTokens, out Dictionary<string, USR_TOKEN> cachedData))
            {
                // Contruct Dictionary
                cachedData = new Dictionary<string, USR_TOKEN>();

                // Set Memory Cache
                MemoryCache.Set(cacheName_UserTokens, cachedData, MemoryCacheOptions);
            }

            if (forceReCache || !cachedData.ContainsKey(token))
            {
                // Get Data
                usertoken = AppConn.dbConn.GetConnection().Query<USR_TOKEN>("SELECT * FROM " + Tables.USR_TOKENS + " WHERE LOGIN_ACCESSTOKEN='" + token + "'").FirstOrDefault();
                cachedData[token] = usertoken;
            }
            else if (cachedData.ContainsKey(token))
            {
                usertoken = cachedData[token];
            }

            // Return
            return usertoken;
        }
        #endregion
    }
}