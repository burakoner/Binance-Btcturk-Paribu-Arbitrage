using Arbitrage.CoreApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Arbitrage.CoreApi.RestApi
{
    public partial class CoreApiController
    {
        [HttpGet("opps/markets")]
        // public async Task<ActionResult> Opps_Markets_Get()
        public ActionResult Opps_Markets_Get()
        {
            /*
            // Api Security
            var bodyString = await this.Request.GetRawBodyStringAsync(Encoding.UTF8);
            var securityReport = this.AccessTokenSecurity(bodyString);
            if (securityReport.Error != null)
                return this.Ok(new ApiResponse(null, securityReport.Error));
            */

            // Api Response
            var apiResponse = new ApiResponse();

            // Return
            apiResponse.Data = AppCache.GetExchangeMarkets();
            return this.Ok(apiResponse);
        }


        [HttpGet("opps/all")]
        // public async Task<ActionResult> Opps_All_Get()
        public ActionResult Opps_All_Get()
        {
            /*
            // Api Security
            var bodyString = await this.Request.GetRawBodyStringAsync(Encoding.UTF8);
            var securityReport = this.AccessTokenSecurity(bodyString);
            if (securityReport.Error != null)
                return this.Ok(new ApiResponse(null, securityReport.Error));
            */

            // Api Response
            var apiResponse = new ApiResponse();

            // Return
            apiResponse.Data = AppCache.MarketTracker.Values;
            return this.Ok(apiResponse);

        }
    }
}