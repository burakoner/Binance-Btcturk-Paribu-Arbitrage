using Arbitrage.CoreApi.BaseStructure;
using Arbitrage.CoreApi.Database;
using Arbitrage.CoreApi.Database.Poco;
using Arbitrage.CoreApi.Enums;
using Arbitrage.CoreApi.Models.Exchange;
using BtcTurk.Net;
using Dapper;
using Gizza.Data.Attributes;
using Gizza.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Arbitrage.CoreApi.Services
{
    public class TelegramService : BaseService
    {
        private AppCache AppCache { get; set; }
        private AppConnections AppConn { get; set; }
        private AppSettings AppSettings { get; set; }
        private List<USR_DATA> UserAccounts { get; set; }
        private List<USR_MESSAGE> UserMessages { get; set; }
        private TelegramBotClient TelegramBotClient { get; set; }

        public TelegramService(AppCache appCache, AppConnections appConn, AppSettings appSettings)
        {
            AppCache = appCache;
            AppConn = appConn;
            AppSettings = appSettings;
            UserMessages = AppConn.dbConn.GetConnection().Query<USR_MESSAGE>(
            "SELECT * FROM " + Tables.USR_MESSAGES + " WHERE CAT>" + DateTime.Now.AddDays(-10).ToUnixTimeMilliSeconds() + " ORDER BY CAT")
            .ToList();
            TelegramBotClient = new TelegramBotClient(AppSettings.Telegram_Token);

            var userIds = AppConn.dbConn.GetConnection().Query<int>("SELECT ID FROM " + Tables.USR_DATA).ToList();
            UserAccounts = new List<USR_DATA>();
            foreach (var userId in userIds)
            {
                var user = AppCache.GetUserData(userId);
                if (user != null) UserAccounts.Add(user);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            // Wait 10 seconds for the first turn
            await Task.Delay(TimeSpan.FromSeconds(15), ct);

            // Telegram Notifications Timer
            Thread t01 = new Thread(TelegramNotifications)
            {
                Name = "Telegram Notifications Thread"
            };
            t01.Start();
        }

        private async void TelegramNotifications()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

            while (true)
            {
                try
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    foreach (var userAccount in UserAccounts)
                    {
                        if (!userAccount.TELEGRAM_ACTIVE) continue;
                        if (userAccount.TELEGRAM_USER_ID <= 0) continue;
                        if (userAccount.TELEGRAM_PERCENT <= 0) continue;
                        if (userAccount.TELEGRAM_INTERVAL <= 0) continue;
                        Dictionary<string, bool> telegramChannels = null;
                        try
                        {
                            telegramChannels = JsonConvert.DeserializeObject<Dictionary<string, bool>>(userAccount.TELEGRAM_CHANNELS);
                        } catch { }
                        if (telegramChannels == null) continue;

                        foreach (var tracker in AppCache.MarketTracker.Values)
                        {
                            if (tracker.BinanceToBtcTurkClassic != null
                                && telegramChannels.ContainsKey("B2TC")
                                && telegramChannels["B2TC"])
                                await CheckAndSend(userAccount, tracker, tracker.BinanceToBtcTurkClassic);

                            if (tracker.BinanceToParibuClassic != null
                                && telegramChannels.ContainsKey("B2PC")
                                && telegramChannels["B2PC"])
                                await CheckAndSend(userAccount, tracker, tracker.BinanceToParibuClassic);

                            if (tracker.BtcTurkToBinanceClassic != null
                                && telegramChannels.ContainsKey("T2BC")
                                && telegramChannels["T2BC"])
                                await CheckAndSend(userAccount, tracker, tracker.BtcTurkToBinanceClassic);

                            if (tracker.BtcTurkToParibuClassic != null
                                && telegramChannels.ContainsKey("T2PC")
                                && telegramChannels["T2PC"])
                                await CheckAndSend(userAccount, tracker, tracker.BtcTurkToParibuClassic);

                            if (tracker.ParibuToBinanceClassic != null
                                && telegramChannels.ContainsKey("P2BC")
                                && telegramChannels["P2BC"])
                                await CheckAndSend(userAccount, tracker, tracker.ParibuToBinanceClassic);

                            if (tracker.ParibuToBtcTurkClassic != null
                                && telegramChannels.ContainsKey("P2TC")
                                && telegramChannels["P2TC"])
                                await CheckAndSend(userAccount, tracker, tracker.ParibuToBtcTurkClassic);

                            /* -------- */

                            if (tracker.BinanceToBtcTurkCross != null
                                && telegramChannels.ContainsKey("B2TX")
                                && telegramChannels["B2TX"])
                                await CheckAndSend(userAccount, tracker, tracker.BinanceToBtcTurkCross);

                            if (tracker.BinanceToParibuCross != null
                                && telegramChannels.ContainsKey("B2PX")
                                && telegramChannels["B2PX"])
                                await CheckAndSend(userAccount, tracker, tracker.BinanceToParibuCross);

                            if (tracker.BtcTurkToBinanceCross != null
                                && telegramChannels.ContainsKey("T2BX")
                                && telegramChannels["T2BX"])
                                await CheckAndSend(userAccount, tracker, tracker.BtcTurkToBinanceCross);

                            if (tracker.BtcTurkToParibuCross != null
                                && telegramChannels.ContainsKey("T2PX")
                                && telegramChannels["T2PX"])
                                await CheckAndSend(userAccount, tracker, tracker.BtcTurkToParibuCross);

                            if (tracker.ParibuToBinanceCross != null
                                && telegramChannels.ContainsKey("P2BX")
                                && telegramChannels["P2BX"])
                                await CheckAndSend(userAccount, tracker, tracker.ParibuToBinanceCross);

                            if (tracker.ParibuToBtcTurkCross != null
                                && telegramChannels.ContainsKey("P2TX")
                                && telegramChannels["P2TX"])
                                await CheckAndSend(userAccount, tracker, tracker.ParibuToBtcTurkCross);
                        }
                    }
                    sw.Stop();
                    Debug.WriteLine("TelegramNotifications loop completed in " + sw.Elapsed);
                }
                catch (Exception ex)
                {
                    Exception a = ex;
                }
                finally
                {
                    // Wait 5 seconds for the next turn
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }

        private async Task CheckAndSend(USR_DATA userAccount, ExchangeTracker tracker, ArbitrageOpportunity opportunity)
        {
            try
            {
                if (userAccount.TELEGRAM_PERCENT > opportunity.ProfitPercent)
                    return;

                var latestMessage = this.UserMessages.Where(x =>
                    x.USER_ID == userAccount.ID &&
                    x.MARKET_ID == tracker.Market.ID &&
                    x.MODE == opportunity.Mode &&
                    x.SENDER == opportunity.Sender &&
                    x.RECIPIENT == opportunity.Recipient).OrderByDescending(x => x.CAT).FirstOrDefault();
                if (latestMessage == null || latestMessage.CAT < DateTime.Now.AddMinutes(-userAccount.TELEGRAM_INTERVAL).ToUnixTimeMilliseconds())
                {
                    var message = "<b>Yeni Arbitraj Fırsatı</b>\n" +
                        "\n" +
                        "Parite: <b>" + opportunity.Symbol + "</b>\n" +
                        "Mod: <b>" + opportunity.Mode.GetLabel() + "</b>\n" +
                        "\n" +
                        "Alış Borsası: <b>" + opportunity.Sender.GetLabel() + "</b>\n" +
                        "Alış Marketi: <b>" + opportunity.BuyMarketSymbol + "</b>\n" +
                        "Alış Fiyatı: <b>" + opportunity.BuyPriceInSender + "</b>\n" +
                        "Alış Miktarı: <b>" + opportunity.BuyAmountInSender + "</b>\n" +
                        "\n";
                    if (opportunity.Mode == ArbitrageMode.Classic)
                    {
                        message +=
                            "Satış Borsası: <b>" + opportunity.Recipient.GetLabel() + "</b>\n" +
                            "Satış Marketi: <b>" + opportunity.SellMarketSymbol + "</b>\n" +
                            "Satış Fiyatı: <b>" + opportunity.SellPriceInRecipient + "</b>\n" +
                            "Satış Miktarı: <b>" + opportunity.SellAmountInRecipient + "</b>\n" +
                            "\n";
                    }
                    else if (opportunity.Mode == ArbitrageMode.Cross)
                    {
                        message +=
                            "Satış Borsası: <b>" + opportunity.Recipient.GetLabel() + "</b>\n" +
                            "Satış Marketi: <b>" + opportunity.SellMarketSymbol + "</b>\n" +
                            "Satış Fiyatı: <b>" + opportunity.CrossSellPriceInRecipient + "</b>\n" +
                            "Satış Miktarı: <b>" + opportunity.CrossSellAmountInRecipient + "</b>\n" +
                            "\n";
                    }
                    message +=
                        "İşlem Varlığı: <b>" + opportunity.TransferAsset + "</b>\n" +
                        "İşlem Miktarı: <b>" + opportunity.TransferAmount + "</b>\n" +
                        "\n" +
                        "Maliyet: <b>" + opportunity.Expense.ToString("0.00") + "</b>\n" +
                        "Gelir: <b>" + opportunity.Revenue.ToString("0.00") + "</b>\n" +
                        "Kazanç: <b>" + opportunity.ProfitAmount.ToString("0.00") + " " + opportunity.ProfitAsset + "</b>\n" +
                        "Kar (%): <b>" + opportunity.ProfitPercent + "</b>\n" +
                        "\n" +
                        "TRY Kazanç (Yaklaşık): <b>" + opportunity.ProfitAmountTRY.ToString("0.00") + "</b>\n" +
                        "USDT Kazanç (Yaklaşık): <b>" + opportunity.ProfitAmountUSDT.ToString("0.00") + "</b>";

                    var status = UserMessageStatus.New;
                    try
                    {
                        status = UserMessageStatus.Sending;
                        var sentMessage = await TelegramBotClient.SendTextMessageAsync(chatId: userAccount.TELEGRAM_USER_ID, text: message, parseMode: ParseMode.Html);
                        status = UserMessageStatus.Sent;
                    }
                    catch
                    {
                        status = UserMessageStatus.Failed;
                    }

                    //try
                    //{
                        var userMessage = new USR_MESSAGE
                        {
                            CAT = DateTime.Now.ToUnixTimeMilliSeconds(),
                            TYPE = UserMessageType.Telegram,
                            STATUS = status,
                            USER_ID = userAccount.ID,
                            MARKET_ID = tracker.Market.ID,
                            MODE = opportunity.Mode,
                            SENDER = opportunity.Sender,
                            RECIPIENT = opportunity.Recipient,
                            PROFIT_PERCENT = opportunity.ProfitPercent,
                            MESSAGE = message,
                        };
                        this.UserMessages.Add(userMessage);
                        userMessage.SubmitChanges(this.AppConn.dbConn);
                    //}
                    //catch { }
                }
            }
            catch { }
        }

    }
}