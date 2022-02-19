using Arbitrage.CoreApi.Database.Poco;
using Arbitrage.CoreApi.Enums;
using Newtonsoft.Json;
using System;

namespace Arbitrage.CoreApi.Models.Exchange
{
    public class ExchangeTracker
    {
        internal AppCache AppCache { get; set; }

        [JsonProperty("Market")]
        public EXC_MARKET Market { get; set; }
        internal EXC_ASSET BaseAsset { get; set; }
        internal EXC_ASSET QuoteAsset { get; set; }

        internal ExchangeTicker BinanceTicker { get; set; }
        internal ExchangeTicker BinanceCrossTicker { get; set; }
        internal ExchangeTicker BtcTurkTicker { get; set; }
        internal ExchangeTicker BtcTurkCrossTicker { get; set; }
        internal ExchangeTicker ParibuTicker { get; set; }
        internal ExchangeTicker ParibuCrossTicker { get; set; }


        [JsonProperty("Symbol")]
        public string Symbol => Market.SYMBOL;


        [JsonProperty("BinanceSymbol")]
        public string BinanceSymbol => Market.BINANCE_SYMBOL;
        [JsonProperty("BtcTurkSymbol")]
        public string BtcTurkSymbol => Market.BTCTURK_SYMBOL;
        [JsonProperty("ParibuSymbol")]
        public string ParibuSymbol => Market.PARIBU_SYMBOL;


        [JsonProperty("BinanceCrossSymbol")]
        public string BinanceCrossSymbol => Market.BINANCE_USDT_MARKET;
        [JsonProperty("BtcTurkCrossSymbol")]
        public string BtcTurkCrossSymbol => Market.BTCTURK_USDT_MARKET;
        [JsonProperty("ParibuCrossSymbol")]
        public string ParibuCrossSymbol => Market.PARIBU_USDT_MARKET;


        [JsonProperty("BinanceToBtcTurkClassic")]
        public ArbitrageOpportunity BinanceToBtcTurkClassic
            => CheckClassicOpportunity(ExchangePlatform.Binance, ExchangePlatform.BtcTurk, ArbitrageMode.Classic);

        [JsonProperty("BinanceToParibuClassic")]
        public ArbitrageOpportunity BinanceToParibuClassic
            => CheckClassicOpportunity(ExchangePlatform.Binance, ExchangePlatform.Paribu, ArbitrageMode.Classic);

        [JsonProperty("BtcTurkToBinanceClassic")]
        public ArbitrageOpportunity BtcTurkToBinanceClassic
            => CheckClassicOpportunity(ExchangePlatform.BtcTurk, ExchangePlatform.Binance, ArbitrageMode.Classic);

        [JsonProperty("BtcTurkToParibuClassic")]
        public ArbitrageOpportunity BtcTurkToParibuClassic
            => CheckClassicOpportunity(ExchangePlatform.BtcTurk, ExchangePlatform.Paribu, ArbitrageMode.Classic);

        [JsonProperty("ParibuToBinanceClassic")]
        public ArbitrageOpportunity ParibuToBinanceClassic
            => CheckClassicOpportunity(ExchangePlatform.Paribu, ExchangePlatform.Binance, ArbitrageMode.Classic);

        [JsonProperty("ParibuToBtcTurkClassic")]
        public ArbitrageOpportunity ParibuToBtcTurkClassic
            => CheckClassicOpportunity(ExchangePlatform.Paribu, ExchangePlatform.BtcTurk, ArbitrageMode.Classic);

        [JsonProperty("BinanceToBtcTurkCross")]
        public ArbitrageOpportunity BinanceToBtcTurkCross
            => CheckClassicOpportunity(ExchangePlatform.Binance, ExchangePlatform.BtcTurk, ArbitrageMode.Cross);

        [JsonProperty("BinanceToParibuCross")]
        public ArbitrageOpportunity BinanceToParibuCross
            => CheckClassicOpportunity(ExchangePlatform.Binance, ExchangePlatform.Paribu, ArbitrageMode.Cross);

        [JsonProperty("BtcTurkToBinanceCross")]
        public ArbitrageOpportunity BtcTurkToBinanceCross
            => CheckClassicOpportunity(ExchangePlatform.BtcTurk, ExchangePlatform.Binance, ArbitrageMode.Cross);

        [JsonProperty("BtcTurkToParibuCross")]
        public ArbitrageOpportunity BtcTurkToParibuCross
            => CheckClassicOpportunity(ExchangePlatform.BtcTurk, ExchangePlatform.Paribu, ArbitrageMode.Cross);

        [JsonProperty("ParibuToBinanceCross")]
        public ArbitrageOpportunity ParibuToBinanceCross
            => CheckClassicOpportunity(ExchangePlatform.Paribu, ExchangePlatform.Binance, ArbitrageMode.Cross);

        [JsonProperty("ParibuToBtcTurkCross")]
        public ArbitrageOpportunity ParibuToBtcTurkCross
            => CheckClassicOpportunity(ExchangePlatform.Paribu, ExchangePlatform.BtcTurk, ArbitrageMode.Cross);

        public ArbitrageOpportunity CheckClassicOpportunity(ExchangePlatform sender, ExchangePlatform recipient, ArbitrageMode mode)
        {
            // Check Point
            if (mode == ArbitrageMode.Classic)
            {
                if (sender == ExchangePlatform.Binance)
                {
                    if (recipient == ExchangePlatform.Paribu && !this.Market.BINANCE_TO_PARIBU_CLASSIC)
                        return null;
                    if (recipient == ExchangePlatform.BtcTurk && !this.Market.BINANCE_TO_BTCTURK_CLASSIC)
                        return null;
                }
                else if (sender == ExchangePlatform.Paribu)
                {
                    if (recipient == ExchangePlatform.Binance && !this.Market.PARIBU_TO_BINANCE_CLASSIC)
                        return null;
                    if (recipient == ExchangePlatform.BtcTurk && !this.Market.PARIBU_TO_BTCTURK_CLASSIC)
                        return null;
                }
                else if (sender == ExchangePlatform.BtcTurk)
                {
                    if (recipient == ExchangePlatform.Binance && !this.Market.BTCTURK_TO_BINANCE_CLASSIC)
                        return null;
                    if (recipient == ExchangePlatform.Paribu && !this.Market.BTCTURK_TO_PARIBU_CLASSIC)
                        return null;
                }
            }
            else if (mode == ArbitrageMode.Cross)
            {
                if (sender == ExchangePlatform.Binance)
                {
                    if (recipient == ExchangePlatform.Paribu && !this.Market.BINANCE_TO_PARIBU_CROSS)
                        return null;
                    if (recipient == ExchangePlatform.BtcTurk && !this.Market.BINANCE_TO_BTCTURK_CROSS)
                        return null;
                }
                else if (sender == ExchangePlatform.Paribu)
                {
                    if (recipient == ExchangePlatform.Binance && !this.Market.PARIBU_TO_BINANCE_CROSS)
                        return null;
                    if (recipient == ExchangePlatform.BtcTurk && !this.Market.PARIBU_TO_BTCTURK_CROSS)
                        return null;
                }
                else if (sender == ExchangePlatform.BtcTurk)
                {
                    if (recipient == ExchangePlatform.Binance && !this.Market.BTCTURK_TO_BINANCE_CROSS)
                        return null;
                    if (recipient == ExchangePlatform.Paribu && !this.Market.BTCTURK_TO_PARIBU_CROSS)
                        return null;
                }
            }

            // Check Point
            if (mode == ArbitrageMode.Classic)
            {
                if (sender == ExchangePlatform.Binance || recipient == ExchangePlatform.Binance)
                    if (BinanceTicker == null) return null;
                if (sender == ExchangePlatform.Paribu || recipient == ExchangePlatform.Paribu)
                    if (ParibuTicker == null) return null;
                if (sender == ExchangePlatform.BtcTurk || recipient == ExchangePlatform.BtcTurk)
                    if (BtcTurkTicker == null) return null;
            }
            else if (mode == ArbitrageMode.Cross)
            {
                if (sender == ExchangePlatform.Binance || recipient == ExchangePlatform.Binance)
                    if (BinanceTicker == null) return null;
                if (sender == ExchangePlatform.Paribu || recipient == ExchangePlatform.Paribu)
                    if (ParibuTicker == null) return null;
                if (sender == ExchangePlatform.BtcTurk || recipient == ExchangePlatform.BtcTurk)
                    if (BtcTurkTicker == null) return null;

                if (recipient == ExchangePlatform.Binance)
                    if (BinanceCrossTicker == null) return null;
                if (recipient == ExchangePlatform.Paribu)
                    if (ParibuCrossTicker == null) return null;
                if (recipient == ExchangePlatform.BtcTurk)
                    if (BtcTurkCrossTicker == null) return null;
            }

            // Action
            var buyMarket = "";
            var sellMarket = "";
            var senderAskPrice = 0.0m;
            var senderAskAmount = 0.0m;
            var recipientBidPrice = 0.0m;
            var recipientBidAmount = 0.0m;
            var recipientCrossBidPrice = 0.0m;
            var recipientCrossBidAmount = 0.0m;

            if (sender == ExchangePlatform.Binance)
            {
                buyMarket = BinanceTicker.Symbol;
                senderAskPrice = BinanceTicker.AskPrice;
                senderAskAmount = BinanceTicker.AskQuantity;
            }
            else if (sender == ExchangePlatform.Paribu)
            {
                buyMarket = ParibuTicker.Symbol;
                senderAskPrice = ParibuTicker.AskPrice;
                senderAskAmount = ParibuTicker.AskQuantity;
            }
            else if (sender == ExchangePlatform.BtcTurk)
            {
                buyMarket = BtcTurkTicker.Symbol;
                senderAskPrice = BtcTurkTicker.AskPrice;
                senderAskAmount = BtcTurkTicker.AskQuantity;
            }

            if (mode == ArbitrageMode.Classic)
            {
                if (recipient == ExchangePlatform.Binance)
                {
                    sellMarket = BinanceTicker.Symbol;
                    recipientBidPrice = BinanceTicker.BidPrice;
                    recipientBidAmount = BinanceTicker.BidQuantity;
                }
                else if (recipient == ExchangePlatform.BtcTurk)
                {
                    sellMarket = BtcTurkTicker.Symbol;
                    recipientBidPrice = BtcTurkTicker.BidPrice;
                    recipientBidAmount = BtcTurkTicker.BidQuantity;
                }
                else if (recipient == ExchangePlatform.Paribu)
                {
                    sellMarket = ParibuTicker.Symbol;
                    recipientBidPrice = ParibuTicker.BidPrice;
                    recipientBidAmount = ParibuTicker.BidQuantity;
                }
            }
            else if (mode == ArbitrageMode.Cross)
            {
                if (recipient == ExchangePlatform.Binance)
                {
                    sellMarket = BinanceCrossTicker.Symbol;
                    recipientCrossBidPrice = BinanceCrossTicker.BidPrice;
                    recipientCrossBidAmount = BinanceCrossTicker.BidQuantity;
                }
                else if (recipient == ExchangePlatform.BtcTurk)
                {
                    sellMarket = BtcTurkCrossTicker.Symbol;
                    recipientCrossBidPrice = BtcTurkCrossTicker.BidPrice;
                    recipientCrossBidAmount = BtcTurkCrossTicker.BidQuantity;
                }
                else if (recipient == ExchangePlatform.Paribu)
                {
                    sellMarket = ParibuCrossTicker.Symbol;
                    recipientCrossBidPrice = ParibuCrossTicker.BidPrice;
                    recipientCrossBidAmount = ParibuCrossTicker.BidQuantity;
                }
            }

            // Return
            return new ArbitrageOpportunity
            {
                Mode = mode,
                Symbol = Symbol,
                Sender = sender,
                Recipient = recipient,
                AppCache = this.AppCache,
                BuyMarketSymbol = buyMarket,
                SellMarketSymbol = sellMarket,
                BuyPriceInSender = senderAskPrice,
                BuyAmountInSender = senderAskAmount,
                SellPriceInRecipient = recipientBidPrice,
                SellAmountInRecipient = recipientBidAmount,
                CrossSellPriceInRecipient = recipientCrossBidPrice,
                CrossSellAmountInRecipient = recipientCrossBidAmount,

                ProfitAsset = QuoteAsset.SYMBOL,
                TransferAsset = BaseAsset.SYMBOL,
            };
        }

    }

    public class ArbitrageOpportunity
    {
        internal AppCache AppCache { get; set; }

        [JsonProperty("Mode")]
        public ArbitrageMode Mode { get; set; }
        [JsonProperty("Sender")]
        public ExchangePlatform Sender { get; set; }
        [JsonProperty("Recipient")]
        public ExchangePlatform Recipient { get; set; }


        [JsonProperty("Symbol")]
        public string Symbol { get; set; }

        [JsonProperty("BuyMarketSymbol")]
        public string BuyMarketSymbol { get; set; }

        [JsonProperty("SellMarketSymbol")]
        public string SellMarketSymbol { get; set; }

        // = AskPriceInSender
        [JsonProperty("BuyPriceInSender")]
        public decimal BuyPriceInSender { get; set; }

        // = AskAmountInSender
        [JsonProperty("BuyAmountInSender")]
        public decimal BuyAmountInSender { get; set; }

        // = BidPriceInRecipient
        [JsonProperty("SellPriceInRecipient")]
        public decimal SellPriceInRecipient { get; set; }

        // = BidAmountInRecipient
        [JsonProperty("SellAmountInRecipient")]
        public decimal SellAmountInRecipient { get; set; }

        [JsonProperty("CrossSellPriceInRecipient")]
        public decimal CrossSellPriceInRecipient { get; set; }

        [JsonProperty("CrossSellAmountInRecipient")]
        public decimal CrossSellAmountInRecipient { get; set; }


        [JsonProperty("Expense")]
        public decimal Expense => Profit.expense;
        [JsonProperty("Revenue")]
        public decimal Revenue => Profit.revenue;

        [JsonProperty("TransferAsset")]
        public string TransferAsset { get; set; }
        [JsonProperty("TransferAmount")]
        public decimal TransferAmount =>
            Mode == ArbitrageMode.Classic
            ? Math.Min(BuyAmountInSender, SellAmountInRecipient)
            : Math.Min(BuyAmountInSender, CrossSellAmountInRecipient);

        [JsonProperty("ProfitAsset")]
        public string ProfitAsset { get; set; }
        [JsonProperty("ProfitAmount")]
        public decimal ProfitAmount => Profit.profitAmount;
        [JsonProperty("ProfitPercent")]
        public decimal ProfitPercent => Profit.profitPercent;

        // Yaklaşık Değer
        [JsonProperty("ProfitAmountTRY")]
        public decimal ProfitAmountTRY
        {
            get
            {
                if (ProfitAsset == "TRY") return ProfitAmount;
                if (ProfitAsset == "USDT")
                {
                    if (AppCache.USDTTRY[ExchangePlatform.Binance] != null && AppCache.USDTTRY[ExchangePlatform.Binance].AskPrice > 0)
                    {
                        return decimal.Round(ProfitAmount * AppCache.USDTTRY[ExchangePlatform.Binance].AskPrice, 2);
                    }
                }

                return 0.0m;
            }
        }

        // Yaklaşık Değer
        [JsonProperty("ProfitAmountUSDT")]
        public decimal ProfitAmountUSDT
        {
            get
            {
                if (ProfitAsset == "USDT") return ProfitAmount;
                if (ProfitAsset == "TRY")
                {
                    if (AppCache.USDTTRY[ExchangePlatform.Binance] != null && AppCache.USDTTRY[ExchangePlatform.Binance].AskPrice > 0)
                    {
                        return decimal.Round(ProfitAmount / AppCache.USDTTRY[ExchangePlatform.Binance].AskPrice, 2);
                    }
                }

                return 0.0m;
            }
        }

        private (decimal expense, decimal revenue, decimal profitAmount, decimal profitPercent) Profit
        {
            get
            {
                if (BuyPriceInSender <= 0 || BuyAmountInSender <= 0)
                    return (0.0m, 0.0m, 0.0m, 0.0m);

                if (Mode == ArbitrageMode.Classic && (SellPriceInRecipient <= 0 || SellAmountInRecipient <= 0))
                    return (0.0m, 0.0m, 0.0m, 0.0m);

                if (Mode == ArbitrageMode.Cross && (CrossSellPriceInRecipient <= 0 || CrossSellAmountInRecipient <= 0))
                    return (0.0m, 0.0m, 0.0m, 0.0m);

                decimal expense = BuyPriceInSender * TransferAmount;
                decimal revenue = Mode ==
                    ArbitrageMode.Classic
                    ? SellPriceInRecipient * TransferAmount
                    : CrossSellPriceInRecipient * TransferAmount;
                if (Mode == ArbitrageMode.Cross)
                {
                    // TRY'den alıp USDT'den satacak
                    if (SellMarketSymbol.EndsWith("USDT", StringComparison.InvariantCultureIgnoreCase))
                    {
                        revenue *= AppCache.USDTTRY[Recipient].BidPrice;
                    }

                    // USDT'den alıp TRY'den satacak
                    if (SellMarketSymbol.EndsWith("TRL", StringComparison.InvariantCultureIgnoreCase) ||
                        SellMarketSymbol.EndsWith("TL", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (AppCache.USDTTRY[Recipient].BidPrice != 0)
                        {
                            revenue *= AppCache.USDTTRY[Recipient].BidPrice;
                        }
                        else
                        {
                            expense = 0.0m;
                            revenue = 0.0m;
                        }
                    }
                }
                decimal profitAmount = decimal.Round(revenue - expense, 2);
                decimal profitPercent = expense == 0 ? 0 : decimal.Round(profitAmount * 100.0m / expense, 2);
                return (expense, revenue, profitAmount, profitPercent);
                // Buraya Transfer ücreti, alış komisyonu ve satış komisyonu da eklenebilir.
            }
        }

    }

}