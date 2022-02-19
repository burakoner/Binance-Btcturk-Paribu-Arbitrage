using Arbitrage.CoreApi.Database.Poco;
using Arbitrage.CoreApi.Enums;
using Gizza.Extensions;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Arbitrage.CoreApi
{
    public class AppErrors
    {
        public AppConnections AppConn { get; set; }

        public AppErrors(AppConnections appConn)
        {
            AppConn = appConn;
        }

        // Sample
        // AppErrors.SaveToDatabase(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
        public void SaveToDatabase(string className, string methodName, Exception ex, string json = "")
        {
            try
            {
                // Get Version
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                int intVersion =
                    version.Major * 1000000 +
                    version.Minor * 10000 +
                    version.Build * 100 +
                    version.Revision;

                // Variable
                APP_ERROR appLog = new APP_ERROR
                {
                    CAT = AppStatic.Epoch,
                    TYPE = AppErrorType.Exception,
                    STATUS = AppErrorStatus.New,
                    LOGLEVEL = AppErrorLevel.Error,
                    CLASS = className,
                    METHOD = methodName,
                    VERSION = version.ToString(),
                    EXCEPTION_DATA = ex.Data.ToString(),
                    EXCEPTION_HELPLINK = ex.HelpLink,
                    EXCEPTION_HRESULT = ex.HResult.ToString(),
                    EXCEPTION_MESSAGE = ex.Message,
                    EXCEPTION_SOURCE = ex.Source,
                    EXCEPTION_STACKTRACE = ex.StackTrace,
                    EXCEPTION_TARGETSITE = ex.TargetSite.ToString(),
                    EXCEPTION_JSONDATA = json.IsNotNullOrEmpty() ? json : JsonConvert.SerializeObject(ex)
                };

                // Save
                appLog.SubmitChanges(AppConn.dbConn);
            }
            catch { }
        }

    }
}