using Arbitrage.CoreApi.StreamApi.Exchange;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;

namespace Arbitrage.CoreApi.RestApi
{
    [ApiController]
    [Route("api/v1")]
    public partial class CoreApiController : ControllerBase
    {
        /* Singleton Objects */
        public AppCache AppCache { get; set; }
        public AppErrors AppErrors { get; set; }
        public AppSettings AppSettings { get; set; }
        public AppConnections AppConn { get; set; }
        public SocketHandler SocketHandler { get; set; }

        /* Public Fields */
        public IWebHostEnvironment Environment { get; private set; }
        public ILogger<CoreApiController> Logger { get; private set; }

        public CoreApiController(
            IMemoryCache memoryCache,
            IWebHostEnvironment environment,
            ILogger<CoreApiController> logger,
            AppCache appCache,
            AppErrors appErrors,
            AppSettings appSettings,
            AppConnections appConnections,
            SocketHandler socketHandler)
        {
            try
            {
                /* Singleton Objects */
                AppCache = appCache;
                AppErrors = appErrors;
                AppSettings = appSettings;
                AppConn = appConnections;
                SocketHandler = socketHandler;

                /* Public Fields */
                Environment = environment;
                Logger = logger;
            }
            catch (Exception ex)
            {
                AppErrors.SaveToDatabase(GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
    }
}