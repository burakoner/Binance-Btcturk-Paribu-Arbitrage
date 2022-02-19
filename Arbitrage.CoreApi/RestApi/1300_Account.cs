using Arbitrage.CoreApi.Database;
using Arbitrage.CoreApi.Database.Poco;
using Arbitrage.CoreApi.Extensions;
using Arbitrage.CoreApi.Models;
using Arbitrage.CoreApi.Models.Account.Requests;
using Dapper;
using Gizza.Data.Security;
using Gizza.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.CoreApi.RestApi
{
    public partial class CoreApiController
    {
        [HttpGet("account/data")]
        public async Task<ActionResult> Account_Data_Get()
        {
            // Api Security
            var bodyString = await this.Request.GetRawBodyStringAsync(Encoding.UTF8);
            var securityReport = this.AccessTokenSecurity(bodyString);
            if (securityReport.Error != null)
                    return this.Ok(new ApiResponse(null, securityReport.Error));

            // Api Response
            var apiResponse = new ApiResponse();

            // Return
            apiResponse.Data = securityReport.UserDataPoco;
            return this.Ok(apiResponse);
        }

        [HttpPost("account/data")]
        public async Task<ActionResult> Account_Data_Post()
        {
            // Api Security
            var bodyString = await this.Request.GetRawBodyStringAsync(Encoding.UTF8);
            var securityReport = this.AccessTokenSecurity(bodyString);
            if (securityReport.Error != null)
                return this.Ok(new ApiResponse(null, securityReport.Error));

            // Request Object
            AccountDataRequest json = null;
            try
            {
                json = JsonConvert.DeserializeObject<AccountDataRequest>(bodyString);
            }
            catch { }

            // Api Response
            var apiResponse = new ApiResponse();

            // Check Point
            if (json == null)
            {
                apiResponse.Error = new ErrorResponse(1181, "Geçersiz İstek");
                return this.Ok(apiResponse);
            }

            // Action
            // securityReport.UserDataPoco.EMAIL = json.Email;
            securityReport.UserDataPoco.NAME = json.Name;
            securityReport.UserDataPoco.SURNAME = json.Surname;
            if (json.Password.IsNotNullOrEmpty())
                securityReport.UserDataPoco.PASSWORD = Crypto.Hash(json.Password, Crypto.HashType.SHA256);
            securityReport.UserDataPoco.SubmitChanges(this.AppConn.dbConn);

            // Return
            apiResponse.Data = true;
            return this.Ok(apiResponse);
        }

        [HttpPost("account/api")]
        public async Task<ActionResult> Account_Api_Post()
        {
            // Api Security
            var bodyString = await this.Request.GetRawBodyStringAsync(Encoding.UTF8);
            var securityReport = this.AccessTokenSecurity(bodyString);
            if (securityReport.Error != null)
                return this.Ok(new ApiResponse(null, securityReport.Error));

            // Request Object
            USR_DATA json = null;
            try
            {
                json = JsonConvert.DeserializeObject<USR_DATA>(bodyString);
            }
            catch { }

            // Api Response
            var apiResponse = new ApiResponse();

            // Check Point
            if (json == null)
            {
                apiResponse.Error = new ErrorResponse(1181, "Geçersiz İstek");
                return this.Ok(apiResponse);
            }

            // Action
            securityReport.UserDataPoco.PARIBU_TOKEN = json.PARIBU_TOKEN;
            securityReport.UserDataPoco.PARIBU_FEE_MAKER = json.PARIBU_FEE_MAKER;
            securityReport.UserDataPoco.PARIBU_FEE_TAKER = json.PARIBU_FEE_TAKER;
            securityReport.UserDataPoco.BINANCE_APIKEY = json.BINANCE_APIKEY;
            securityReport.UserDataPoco.BINANCE_SECRET = json.BINANCE_SECRET;
            securityReport.UserDataPoco.BINANCE_FEE_MAKER = json.BINANCE_FEE_MAKER;
            securityReport.UserDataPoco.BINANCE_FEE_TAKER = json.BINANCE_FEE_TAKER;
            securityReport.UserDataPoco.SubmitChanges(this.AppConn.dbConn);

            // Return
            apiResponse.Data = true;
            return this.Ok(apiResponse);
        }

        [HttpPost("account/telegram")]
        public async Task<ActionResult> Account_Telegram_Post()
        {
            // Api Security
            var bodyString = await this.Request.GetRawBodyStringAsync(Encoding.UTF8);
            var securityReport = this.AccessTokenSecurity(bodyString);
            if (securityReport.Error != null)
                return this.Ok(new ApiResponse(null, securityReport.Error));

            // Request Object
            USR_DATA json = null;
            try
            {
                json = JsonConvert.DeserializeObject<USR_DATA>(bodyString);
            }
            catch { }

            // Api Response
            var apiResponse = new ApiResponse();

            // Check Point
            if (json == null)
            {
                apiResponse.Error = new ErrorResponse(1181, "Geçersiz İstek");
                return this.Ok(apiResponse);
            }

            // Action
            securityReport.UserDataPoco.TELEGRAM_ACTIVE = json.TELEGRAM_ACTIVE;
            securityReport.UserDataPoco.TELEGRAM_USER_ID = json.TELEGRAM_USER_ID;
            securityReport.UserDataPoco.TELEGRAM_PERCENT = json.TELEGRAM_PERCENT;
            securityReport.UserDataPoco.TELEGRAM_INTERVAL = json.TELEGRAM_INTERVAL;
            securityReport.UserDataPoco.TELEGRAM_CHANNELS = json.TELEGRAM_CHANNELS;
            securityReport.UserDataPoco.SubmitChanges(this.AppConn.dbConn);

            // Return
            apiResponse.Data = true;
            return this.Ok(apiResponse);
        }

        [HttpGet("account/favs")]
        public async Task<ActionResult> Account_Favs_Get()
        {
            // Api Security
            var bodyString = await this.Request.GetRawBodyStringAsync(Encoding.UTF8);
            var securityReport = this.AccessTokenSecurity(bodyString);
            if (securityReport.Error != null)
                return this.Ok(new ApiResponse(null, securityReport.Error));

            // Api Response
            var apiResponse = new ApiResponse();
            apiResponse.Data = AppConn.dbConn.GetConnection().Query<USR_FAV>("SELECT * FROM " + Tables.USR_FAVS + " WHERE USER_ID=" + securityReport.UserDataPoco.ID).ToList();

            // Return
            return this.Ok(apiResponse);
        }

        [HttpPost("account/favs")]
        public async Task<ActionResult> Account_Favs_Post()
        {
            // Api Security
            var bodyString = await this.Request.GetRawBodyStringAsync(Encoding.UTF8);
            var securityReport = this.AccessTokenSecurity(bodyString);
            if (securityReport.Error != null)
                return this.Ok(new ApiResponse(null, securityReport.Error));

            // Request Object
            AccountFavsRequest json = null;
            try
            {
                json = JsonConvert.DeserializeObject<AccountFavsRequest>(bodyString);
            }
            catch { }

            // Api Response
            var apiResponse = new ApiResponse();

            // Check Point
            if (json == null)
            {
                apiResponse.Error = new ErrorResponse(1181, "Geçersiz İstek");
                return this.Ok(apiResponse);
            }

            // Action
            this.AppConn.dbConn.GetConnection().Execute($"DELETE FROM {Tables.USR_FAVS} WHERE USER_ID='{securityReport.UserDataPoco.ID}' AND EXC_SENDER='{(int)json.Sender}' AND EXC_RECIPIENT='{(int)json.Recipient}' AND MODE='{(int)json.Mode}'");
            foreach(var fav in json.Data)
            {
                if (fav.Value)
                {
                    var market = AppCache.GetExchangeMarkets().FirstOrDefault(x => x.SYMBOL == fav.Key);
                    if (market != null)
                    {
                        new USR_FAV
                        {
                            MODE = json.Mode,
                            MARKET_ID = market.ID,
                            EXC_SENDER = json.Sender,
                            EXC_RECIPIENT = json.Recipient,
                            USER_ID = securityReport.UserDataPoco.ID,
                        }.SubmitChanges(AppConn.dbConn);
                    }
                }
            }
            
            // Api Response
            apiResponse.Data = AppConn.dbConn.GetConnection().Query<USR_FAV>("SELECT * FROM " + Tables.USR_FAVS + " WHERE USER_ID=" + securityReport.UserDataPoco.ID).ToList();

            // Return
            return this.Ok(apiResponse);
        }
    }
}