using Arbitrage.CoreApi.Database.Poco;
using Arbitrage.CoreApi.Enums;
using Arbitrage.CoreApi.StreamApi.Exchange;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Arbitrage.CoreApi.StreamApi.Objects
{
    public enum SocketClientAuthType
    {
        None,
        ApiKey,
        ApiToken,
    }

    public class SocketClient
    {
        public WebSocket Socket { get; set; }
        public SocketHandler SocketHandler { get; set; }
        public SocketConnection ConnectionInfo { get; set; }

        public DateTime ConnectedAt { get; set; }
        public DateTime LastPingTime { get; set; }
        public DateTime LastPongTime { get; set; }

        public bool IsAuthorized { get; private set; }
        public SocketClientAuthType AuthType { get; private set; }
        public USR_DATA UserDataPoco { get; private set; }

        public Dictionary<string, SocketSubscription> PublicSubscriptions { get; private set; }
        public Dictionary<string, SocketSubscription> PrivateSubscriptions { get; private set; }

        public SocketClient(SocketHandler handler, WebSocket socket, HttpContext context)
        {
            Socket = socket;
            SocketHandler = handler;
            ConnectionInfo = new SocketConnection(context.Connection, context.TraceIdentifier);
            ConnectedAt = AppStatic.Now;
            LastPingTime = AppStatic.Now;
            LastPongTime = AppStatic.Now;
            PublicSubscriptions = new Dictionary<string, SocketSubscription>();
            PrivateSubscriptions = new Dictionary<string, SocketSubscription>();
            AuthType = SocketClientAuthType.None;
        }

        public SocketClient(SocketHandler handler, WebSocket socket, SocketConnection connectionInfo)
        {
            Socket = socket;
            SocketHandler = handler;
            ConnectionInfo = connectionInfo;
            ConnectedAt = AppStatic.Now;
            LastPingTime = AppStatic.Now;
            LastPongTime = AppStatic.Now;
            PublicSubscriptions = new Dictionary<string, SocketSubscription>();
            PrivateSubscriptions = new Dictionary<string, SocketSubscription>();
            AuthType = SocketClientAuthType.None;
        }

        public async Task ProcessRequest(StreamRequest streamRequest)
        {
            StreamOperationCode opcode = streamRequest.OperationCode;
            if (opcode == StreamOperationCode.Subscribe)
            {
                await ProcessSubscribeRequest(streamRequest);
            }

            if (opcode == StreamOperationCode.Unsubscribe)
            {
                await ProcessUnsubscribeRequest(streamRequest);
            }
        }

        public async Task ProcessSubscribeRequest(StreamRequest streamRequest)
        {
            if (/*streamRequest.OperationCode.ToLowerInvariant() == "subscribe"*/ streamRequest.OperationCode == StreamOperationCode.Subscribe && streamRequest.Params != null && streamRequest.Params.Length > 0)
            {
                foreach (string subscribe in streamRequest.Params)
                {
                    await SocketHandler.JoinChannel(this, subscribe, streamRequest.Identifier);
                }
            }
        }

        public async Task ProcessUnsubscribeRequest(StreamRequest streamRequest)
        {
            if (/*streamRequest.OperationCode.ToLowerInvariant() == "unsubscribe"*/ streamRequest.OperationCode == StreamOperationCode.Unsubscribe && streamRequest.Params != null && streamRequest.Params.Length > 0)
            {
                foreach (string subscribe in streamRequest.Params)
                {
                    await SocketHandler.LeaveChannel(this, subscribe, streamRequest.Identifier);
                }
            }
        }
    }
}
