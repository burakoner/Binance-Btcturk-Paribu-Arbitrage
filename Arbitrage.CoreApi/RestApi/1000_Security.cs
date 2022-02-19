using Arbitrage.CoreApi.Models;
using Arbitrage.CoreApi.Models.Access;
using Gizza.Extensions;
using System;
using System.Collections.Specialized;
using System.Web;

namespace Arbitrage.CoreApi.RestApi
{
    public partial class CoreApiController
    {
        private const string HeaderHolder_Request_AccessToken = "x-accesstoken";

        // 1011-1019
        private AccessSecurityReport EpochSignatureSecurity(string bodyString = "", bool useReceiveWindow = false, int rateLimitCredit = 1)
        {
            // First Step: Create Response Object
            AccessSecurityReport securityReport = new AccessSecurityReport()
            {
                AccessToken = Request.Headers[HeaderHolder_Request_AccessToken].ToString(),
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
            };

            // Query String
            string queryString = Request.QueryString.HasValue ? Request.QueryString.Value : "";
            if (queryString.IsNotNullOrEmpty() && queryString.StartsWith("?"))
            {
                queryString = queryString.TrimStart('?');
            }

            NameValueCollection queryStringColl = HttpUtility.ParseQueryString(queryString);

            // Check Timestamp
            if (queryStringColl["ts"].IsNullOrEmpty())
            {
                securityReport.Error = new ErrorResponse(1011, "Eksik Parametre: timestamp");
                return securityReport;
            }

            // Get Timestamp
            long requestTimestamp = 0;
            if (queryStringColl["ts"].IsNotNullOrEmpty() && queryStringColl["ts"].IsNumeric())
            {
                requestTimestamp = queryStringColl["ts"].ToInt64Safe();
            }

            // Check Timestamp
            if (requestTimestamp <= 0)
            {
                securityReport.Error = new ErrorResponse(1012, "Geçersiz Parametre: timestamp");
                return securityReport;
            }

            // Set Privileges
            securityReport.AuthendicationMethod = ApiAuthendicationMethod.EpochSignature;

            // Return
            return securityReport;
        }

        // 1021-1029
        private AccessSecurityReport AccessTokenSecurity(string bodyString = "", bool useReceiveWindow = true, int rateLimitCredit = 1)
        {
            // First Step: Create Response Object
            AccessSecurityReport securityReport = new AccessSecurityReport()
            {
                AccessToken = Request.Headers[HeaderHolder_Request_AccessToken].ToString(),
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
            };

            // Query String
            string queryString = Request.QueryString.HasValue ? Request.QueryString.Value : "";
            if (queryString.IsNotNullOrEmpty() && queryString.StartsWith("?"))
            {
                queryString = queryString.TrimStart('?');
            }

            NameValueCollection queryStringColl = HttpUtility.ParseQueryString(queryString);

            // Evaluate Access Token Partial Security
            PartialSecurity_AccessToken(ref securityReport);
            if (securityReport.Error != null)
            {
                return securityReport;
            }

            // Set Privileges
            securityReport.AuthendicationMethod = ApiAuthendicationMethod.AccessToken;

            // Return
            return securityReport;
        }

        // 1061-1069
        private void PartialSecurity_AccessToken(ref AccessSecurityReport securityReport)
        {
            // Variables
            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            string accessToken = Request.Headers[HeaderHolder_Request_AccessToken].ToString();

            // Get Server Time
            DateTime serverTime = AppStatic.Now;
            long serverTimeLong = serverTime.ToUnixTimeMilliSeconds();

            // Check Access Token
            if (accessToken.IsNullOrEmpty())
            {
                securityReport.Error = new ErrorResponse(1061, "Access Token eksik");
                return;
            }

            // Get Access Token From Database
            securityReport.ApiTokenPoco = AppCache.GetUserToken(accessToken);

            // Check Point
            if (securityReport.ApiTokenPoco == null || securityReport.ApiTokenPoco.LOGIN_ACCESSTOKEN != accessToken || securityReport.ApiTokenPoco.LOGIN_IPADDRESS != ipAddress)
            {
                securityReport.Error = new ErrorResponse(1062, "Access Token geçersiz");
                return;
            }

            // Expiration
            if (securityReport.ApiTokenPoco.LOGIN_EXPIRES < serverTimeLong)
            {
                securityReport.Error = new ErrorResponse(1063, "Access Token kullanım süresi dolmuş");
                return;
            }

            // Get User Poco
            securityReport.UserDataPoco = AppCache.GetUserData(securityReport.ApiTokenPoco.USER_ID);

            // Check Point
            if (securityReport.UserDataPoco == null)
            {
                securityReport.Error = new ErrorResponse(1064, "Hesap bulunamadı");
                return;
            }
        }

    }
}