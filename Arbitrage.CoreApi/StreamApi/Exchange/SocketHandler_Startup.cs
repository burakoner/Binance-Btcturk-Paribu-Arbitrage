using Microsoft.AspNetCore.Builder;
using System;

namespace Arbitrage.CoreApi.StreamApi.Exchange
{
    public partial class SocketHandler
    {
        public void RegisterSocketHandler(IApplicationBuilder app)
        {
            try
            {
                /* WebSocketOptions */
                WebSocketOptions webSocketOptions = new WebSocketOptions()
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(120),
                    // ReceiveBufferSize = this.requestBufferSize, // Deprecated
                };
                // webSocketOptions.AllowedOrigins.Add("https://client.com");
                // webSocketOptions.AllowedOrigins.Add("https://www.client.com");

                /* Enable WebSockets */
                app.UseWebSockets(webSocketOptions);

                /* AcceptWebSocket */
                app.Use(async (context, next) =>
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        if (context.Request.Path == "/stream/v1" || context.Request.Path == "/stream/v1/")
                        {
                            await OnConnect(context);
                        }
                        else
                        {
                            await next();
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }

                });
            }
            catch //(Exception ex)
            {
                //AppErrors.SaveToDatabase(this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

    }
}