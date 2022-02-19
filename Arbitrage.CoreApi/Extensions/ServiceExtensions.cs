using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    IExceptionHandlerFeature contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        System.Exception exception = contextFeature.Error;
                        GlobalErrorResponse globalError = new GlobalErrorResponse
                        {
                            Code = context.Response.StatusCode,
#if DEBUG
                            Message = contextFeature.Error.Message,
#else
                            Message = "Internal Server Error",
#endif
                        };
                        await context.Response.WriteAsync(globalError.ToString());
                    }
                });
            });
        }
    }

    public class GlobalErrorResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        public GlobalErrorResponse()
        {
        }

        public GlobalErrorResponse(int errorCode, string errorMessage)
        {
            Code = errorCode;
            Message = errorMessage;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
