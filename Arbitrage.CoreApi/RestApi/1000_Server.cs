using Arbitrage.CoreApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Arbitrage.CoreApi.RestApi
{
    public partial class CoreApiController
    {
        [HttpGet("server/ping")]
        public ActionResult Server_Ping()
        {
            // Action
            return Ok(new ApiResponse("pong", null));
        }

        [HttpGet("server/time")]
        public ActionResult Server_Time()
        {
            // Action
            return Ok(new ApiResponse(AppStatic.Epoch, null));
        }
    }
}