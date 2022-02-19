using Arbitrage.CoreApi.Enums;
using System.Collections.Generic;

namespace Arbitrage.CoreApi.StreamApi.Objects
{
    public class SocketChannel
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public StreamChannelType Type { get; set; }

        public Dictionary<string, SocketClient> Clients { get; set; }

        public SocketChannel()
        {
            Clients = new Dictionary<string, SocketClient>();
        }

    }
}
