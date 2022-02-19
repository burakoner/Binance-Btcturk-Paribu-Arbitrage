using Arbitrage.CoreApi.Enums;
using Arbitrage.CoreApi.Models.Common;
using Arbitrage.CoreApi.StreamApi.Objects;
using Gizza.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arbitrage.CoreApi.StreamApi.Exchange
{
    public partial class SocketHandler
    {
        /* Singleton Objects */
        public AppCache AppCache { get; set; }
        public AppErrors AppErrors { get; set; }
        public AppConnections AppConn { get; set; }

        /* Public Fields */
        public Dictionary<string, SocketClient> Clients { get; set; }
        public Dictionary<string, SocketChannel> Channels { get; set; }

        /* Pulse Timer */
        public System.Timers.Timer PulseTimer { get; set; }
        public bool IsPulseTimerRunning { get; set; }

        /* Read-Only Fields */
        public readonly int requestBufferSize = 1 * 1024;
        public readonly int responseBufferSize = 1024 * 1024;
        public readonly string SymbolAll = "All";
        public readonly string SymbolNone = "None";

        public SocketHandler(AppConnections appConn, AppErrors appErrors)
        {
            /* Singleton Objects */
            AppConn = appConn;
            AppErrors = appErrors;
        }

        public void SetAppCache(AppCache appCache)
        {
            /* Set App Cache */
            AppCache = appCache;

            /* Pulse Timer */
            PulseTimer = new System.Timers.Timer();
            PulseTimer.Elapsed += new System.Timers.ElapsedEventHandler(PulseTimer_Action);
            PulseTimer.Interval = 1 * 1000;
            PulseTimer.Enabled = true;

            /* Properties */
            Clients = new Dictionary<string, SocketClient>();
            Channels = new Dictionary<string, SocketChannel>();

            /* Construct Public Channels */
            ConstructPublicChannels();

            /* Construct Order Book Timer */
            ConstructSocketTimers();
        }

        public void ConstructPublicChannels()
        {
            string cname = ChannelName(StreamChannelType.TraceAll);
            Channels.Add(cname, new SocketChannel
            {
                Name = cname,
                Symbol = "ALL",
                Type = StreamChannelType.TraceAll,
            });
        }

        public async Task OnConnect(HttpContext context)
        {
            try
            {
                // var host = this.GetClientHost(context);
                WebSocket ws = await context.WebSockets.AcceptWebSocketAsync();

                // Client Information
                SocketClient sc = new SocketClient(this, ws, context);

                // Add to Clients
                lock (Clients)
                {
                    Clients.Add(sc.ConnectionInfo.Id, sc);
                }

                // Say Hello
                // Selamlamıyorum. Bu gereksiz sorunlara sebep oluyor
                // await this.SendToClientAsync(sc, this.SystemChannel, $"Welcome to {AppConstants.SystemName} Web-Sockets API", true);

                // Check Query: Subscribe
                if (context.Request.Query.ContainsKey("subscribe"))
                {
                    string[] subscribes = context.Request.Query["subscribe"].ToString().Split(",");
                    foreach (string subscribe in subscribes)
                    {
                        await JoinChannel(sc, subscribe, 0);
                    }
                }

                // Prepare for Listening
                byte[] requestBuffer = new byte[requestBufferSize];
                WebSocketReceiveResult receiveResult = await ws.ReceiveAsync(new ArraySegment<byte>(requestBuffer), CancellationToken.None);

                // Listen
                while (!receiveResult.CloseStatus.HasValue)
                {
                    // Decode Request
                    string jsonRequest = Encoding.UTF8.GetString(requestBuffer).Trim('\0');
                    bool parseResult = StreamRequest.TryParse(jsonRequest, out StreamRequest streamRequest);

                    // Check Point
                    if (!parseResult || streamRequest == null || !streamRequest.IsValid)
                    {
                        break;
                    }

                    await sc.ProcessRequest(streamRequest);

                    /*
                    // Action
                    string jsonResponse = /*"Selam " +* / sr.OpCode.ToString();
                    //string jsonResponse = /*"Selam " +* / sr.Method;
                    var responseBuffer = new byte[1024 * 1024];
                    responseBuffer = Encoding.UTF8.GetBytes(jsonResponse);

                    // Send Response
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer, 0, jsonResponse.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    */

                    // GC
                    streamRequest = null;

                    // Wait for Next Request
                    requestBuffer = new byte[requestBufferSize];
                    receiveResult = await ws.ReceiveAsync(new ArraySegment<byte>(requestBuffer), CancellationToken.None);
                }
            }
            catch { }
        }

        public async Task JoinChannel(SocketClient sc, string channelName, long identifier)
        {
            try
            {
                // Check Socket Client
                if (sc == null || sc.Socket == null || sc.Socket.State != WebSocketState.Open)
                {
                    return;
                }

                // Check Channel Name
                if (channelName.IsNullOrEmpty())
                {
                    return;
                }

                // Check Socket Channel
                string dicName = Channels.Keys.Where(x => string.Equals(x, channelName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (dicName.IsNullOrEmpty())
                {
                    return;
                }

                // Get Socket Channel
                SocketChannel channel = Channels[dicName];
                if (channel == null)
                {
                    return;
                }

                // Check for if already submitted
                if (channel.Clients.ContainsKey(sc.ConnectionInfo.Id))
                {
                    return;
                }

                // Add to clients
                lock (channel.Clients)
                {
                    channel.Clients.Add(sc.ConnectionInfo.Id, sc);
                }

                // Send Feedback
                StreamResponse<KeyStatePair> response = new StreamResponse<KeyStatePair>(identifier, SystemChannel, new KeyStatePair { Key = channel.Name, State = true });
                await SendToClientAsync(sc, JsonConvert.SerializeObject(response));
            }
            catch { }
        }

        public async Task LeaveChannel(SocketClient sc, string channelName, long identifier)
        {
            try
            {
                // Check Socket Client
                if (sc == null || sc.Socket == null || sc.Socket.State != WebSocketState.Open)
                {
                    return;
                }

                // Check Channel Name
                if (channelName.IsNullOrEmpty())
                {
                    return;
                }

                // Check Socket Channel
                string dicName = Channels.Keys.Where(x => string.Equals(x, channelName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (dicName.IsNullOrEmpty())
                {
                    return;
                }

                // Get Socket Channel
                SocketChannel channel = Channels[dicName];
                if (channel == null)
                {
                    return;
                }

                // Check for if already submitted
                if (!channel.Clients.ContainsKey(sc.ConnectionInfo.Id))
                {
                    return;
                }

                // Add to clients
                lock (channel.Clients)
                {
                    channel.Clients.Remove(sc.ConnectionInfo.Id);
                }

                // Send Feedback
                StreamResponse<KeyStatePair> response = new StreamResponse<KeyStatePair>(identifier, SystemChannel, new KeyStatePair { Key = channel.Name, State = false });
                await SendToClientAsync(sc, JsonConvert.SerializeObject(response));
            }
            catch { }
        }

        public void PulseTimer_Action(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // Check Point
                if (IsPulseTimerRunning)
                {
                    return;
                }

                // Is Running
                IsPulseTimerRunning = true;

                // JOB-01 => Part 1 : Check for connection states
                List<string> removeList = new List<string>();
                lock (Clients)
                {
                    foreach (KeyValuePair<string, SocketClient> dict in Clients)
                    {
                        if (dict.Value.Socket == null || !dict.Value.Socket.State.IsIn(WebSocketState.Open, WebSocketState.Connecting))
                        {
                            removeList.Add(dict.Key);
                        }
                    }
                }

                // JOB-01 => Part 2 : Remove disconnected clients
                foreach (string removeMe in removeList)
                {
                    lock (Clients)
                    {
                        Clients.Remove(removeMe);
                    }
                    foreach (SocketChannel channel in Channels.Values)
                    {
                        lock (channel.Clients)
                        {
                            channel.Clients.Remove(removeMe);
                        }
                    }
                }

                // Is Running
                IsPulseTimerRunning = false;
            }
            catch { }
        }

        public async Task SendToClientAsync(SocketClient sc, string channelName, string message, bool constructStreamResponse = false, CancellationToken ct = default)
        {
            if (sc != null && sc.Socket != null)
            {
                await SendToClientAsync(sc.Socket, channelName, message, constructStreamResponse, ct);
            }
        }

        private async Task SendToClientAsync(WebSocket ws, string channelName, string message, bool constructStreamResponse = false, CancellationToken ct = default)
        {
            // Prepare Message
            byte[] bytes = constructStreamResponse
                ? Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new StreamResponse<string>(channelName, message)))
                : Encoding.UTF8.GetBytes(message);

            // Send Response
            await SendToClientAsync(ws, bytes, ct);
        }

        private async Task SendToClientAsync(SocketClient sc, string streamResponse, CancellationToken ct = default)
        {
            if (sc != null && sc.Socket != null)
            {
                await SendToClientAsync(sc.Socket, Encoding.UTF8.GetBytes(streamResponse), ct);
            }
        }

        private async Task SendToClientAsync(WebSocket ws, byte[] bytes, CancellationToken ct = default)
        {
            try
            {
                // Check Point
                if (ws == null)
                {
                    return;
                }

                // Check Point
                if (ws.State != WebSocketState.Open)
                {
                    return;
                }

                // Send Message
                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, ct);
            }
            catch { }
        }

        private async Task SendToChannelAsync(string channelName, string message, bool constructStreamResponse = false, CancellationToken ct = default)
        {
            // Prepare Message
            byte[] bytes = constructStreamResponse
                ? Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new StreamResponse<string>(channelName, message)))
                : Encoding.UTF8.GetBytes(message);

            // Send Response
            await SendToChannelAsync(channelName, bytes, ct);
        }

        private async Task SendToChannelAsync(string channelName, byte[] bytes, CancellationToken ct = default)
        {
            try
            {
                // Check Socket Channel
                string dicName = Channels.Keys.Where(x => string.Equals(x, channelName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (dicName.IsNullOrEmpty())
                {
                    return;
                }

                // Get Socket Channel
                SocketChannel channel = Channels[dicName];
                if (channel == null)
                {
                    return;
                }

                // Send All
                foreach (SocketClient client in channel.Clients.Values)
                {
                    if (client.Socket != null && client.Socket.State == WebSocketState.Open)
                    {
                        await SendToClientAsync(client.Socket, bytes, ct);
                    }
                }
            }
            catch { }
        }

        public async Task CloseConnectionAsync(WebSocket ws, WebSocketCloseStatus cs = WebSocketCloseStatus.NormalClosure, string cd = "", CancellationToken ct = default)
        {
            try
            {
                // Check Point
                if (ws == null)
                {
                    return;
                }

                // Check Point
                if (ws.State != WebSocketState.Open)
                {
                    return;
                }

                // Action
                await ws.CloseAsync(cs, cd, ct);
            }
            catch { }
        }

    }
}