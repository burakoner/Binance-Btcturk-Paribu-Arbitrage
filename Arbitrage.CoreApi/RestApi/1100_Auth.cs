using Arbitrage.CoreApi.Database;
using Arbitrage.CoreApi.Database.Poco;
using Arbitrage.CoreApi.Extensions;
using Arbitrage.CoreApi.Models;
using Arbitrage.CoreApi.Models.Auth.Requests;
using Arbitrage.CoreApi.Models.Auth.Responses;
using Dapper;
using Gizza.Data.Security;
using Gizza.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.CoreApi.RestApi
{
    public partial class CoreApiController
    {
        #region EpochSignatureSecurity: Not Logged In Methods
        // 1181-1199
        [HttpPost("auth/login")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Auth_Login_Post()
        {
            // Api Security
            string bodyString = await Request.GetRawBodyStringAsync(Encoding.UTF8);
            Models.Access.AccessSecurityReport securityReport = EpochSignatureSecurity(bodyString);
            if (securityReport.Error != null)
            {
                return Ok(new ApiResponse(null, securityReport.Error));
            }

            // Request Object
            AuthLoginRequest json = null;
            try
            {
                json = JsonConvert.DeserializeObject<AuthLoginRequest>(bodyString);
            }
            catch { }

            // Api Response
            ApiResponse apiResponse = new ApiResponse();

            // Check Point
            if (json == null)
            {
                apiResponse.Error = new ErrorResponse(1181, "Geçersiz İstek");
                return Ok(apiResponse);
            }

            // Check Point
            if (json.Email.IsNullOrEmpty())
            {
                apiResponse.Error = new ErrorResponse(1182, "E-posta boş");
                return Ok(apiResponse);
            }

            // Check Point
            if (json.Email.Length > 100)
            {
                apiResponse.Error = new ErrorResponse(1183, "E-posta uzunluğu 0-100 karakter arasında olmalıdır");
                return Ok(apiResponse);
            }

            // Check Point
            if (json.Password.IsNullOrEmpty())
            {
                apiResponse.Error = new ErrorResponse(1185, "Şifre boş");
                return Ok(apiResponse);
            }

            // Get User
            json.Password = Crypto.Hash(json.Password, Crypto.HashType.SHA256);
            USR_DATA userPoco = AppConn.dbConn.GetConnection().Query<USR_DATA>("SELECT * FROM " + Tables.USR_DATA + " WHERE " + AppConn.dbConn.Helpers.LOWER("EMAIL") + "=@Email AND PASSWORD=@Password", json).FirstOrDefault();

            // Check Point
            if (userPoco == null)
            {
                apiResponse.Error = new ErrorResponse(1191, "E-posta veya şifre yanlış");
                return Ok(apiResponse);
            }

            // Get Server Time
            DateTime serverTime = AppStatic.Now;
            long serverTimeLong = serverTime.ToUnixTimeMilliSeconds();

            // AccessToken
            USR_TOKEN apiToken = new USR_TOKEN
            {
                USER_ID = userPoco.ID,
                LOGIN_AT = serverTimeLong,
                LOGIN_EXPIRES = serverTime.AddYears(10).ToUnixTimeMilliSeconds(),
                LOGIN_IPADDRESS = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                LOGIN_ACCESSTOKEN = RandomString.Generate(40, true, true, true, false),
            };
            apiToken.SubmitChanges(AppConn.dbConn);

            // Login Response
            AuthLoginResponse loginResponse = new AuthLoginResponse(apiToken);

            // Return
            apiResponse.Data = loginResponse;
            return Ok(apiResponse);
        }
        #endregion

        #region AccessTokenSecurity: Logged In Methods
        // 1241-1249
        [HttpPost("auth/logout")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Auth_Logout_Post()
        {
            // Api Security
            string bodyString = await Request.GetRawBodyStringAsync(Encoding.UTF8);
            Models.Access.AccessSecurityReport securityReport = AccessTokenSecurity(bodyString);
            if (securityReport.Error != null)
            {
                return Ok(new ApiResponse(null, securityReport.Error));
            }

            // Api Token
            securityReport.ApiTokenPoco.LOGIN_EXPIRES = AppStatic.Epoch;
            securityReport.ApiTokenPoco.SubmitChanges(AppConn.dbConn);

            // Return
            ApiResponse apiResponse = new ApiResponse
            {
                Data = true
            };
            return Ok(apiResponse);
        }

        // 1251-1259
        [HttpGet("auth/session")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Auth_Session_Get()
        {
            // Api Security
            string bodyString = await Request.GetRawBodyStringAsync(Encoding.UTF8);
            Models.Access.AccessSecurityReport securityReport = AccessTokenSecurity(bodyString);
            if (securityReport.Error != null)
            {
                return Ok(new ApiResponse(null, securityReport.Error));
            }

            // Login Response
            AuthLoginResponse loginResponse = new AuthLoginResponse(securityReport.ApiTokenPoco);

            // Return
            ApiResponse apiResponse = new ApiResponse
            {
                Data = loginResponse
            };
            return Ok(apiResponse);
        }

        // 1261-1269
        // Extends to session expiration
        [HttpPost("auth/session")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> Auth_Session_Post()
        {
            // Api Security
            string bodyString = await Request.GetRawBodyStringAsync(Encoding.UTF8);
            Models.Access.AccessSecurityReport securityReport = AccessTokenSecurity(bodyString);
            if (securityReport.Error != null)
            {
                return Ok(new ApiResponse(null, securityReport.Error));
            }

            // Api Token
            securityReport.ApiTokenPoco.LOGIN_EXPIRES = AppStatic.Now.AddYears(10).ToUnixTimeMilliSeconds();
            securityReport.ApiTokenPoco.SubmitChanges(AppConn.dbConn);

            // Login Response
            AuthLoginResponse loginResponse = new AuthLoginResponse(securityReport.ApiTokenPoco);

            // Return
            ApiResponse apiResponse = new ApiResponse
            {
                Data = loginResponse
            };
            return Ok(apiResponse);
        }
        #endregion
    }
}