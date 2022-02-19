using System.Net;

namespace Arbitrage.CoreApi.StreamApi.Objects
{
    public class SocketConnection
    {
        public string Id { get; set; }
        public IPAddress LocalIpAddress { get; set; } // Server
        public int LocalPort { get; set; } // Server
        public IPAddress RemoteIpAddress { get; set; } // Client
        public int RemotePort { get; set; } // Client
        public string TraceIdentifier { get; set; }

        public SocketConnection()
        {
        }

        public SocketConnection(Microsoft.AspNetCore.Http.ConnectionInfo cinfo, string traceIdentifier)
        {
            Id = cinfo.Id;
            LocalIpAddress = cinfo.LocalIpAddress;
            LocalPort = cinfo.LocalPort;
            RemoteIpAddress = cinfo.RemoteIpAddress;
            RemotePort = cinfo.RemotePort;
            TraceIdentifier = traceIdentifier;
        }
    }
}
