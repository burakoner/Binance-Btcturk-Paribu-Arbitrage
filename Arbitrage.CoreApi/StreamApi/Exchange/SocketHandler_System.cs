using Arbitrage.CoreApi.Models.Common;
using Arbitrage.CoreApi.StreamApi.Objects;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.CoreApi.StreamApi.Exchange
{
    public partial class SocketHandler
    {
        public readonly string SystemChannel = "System";

        public async Task SendSystemMessageAsync(SocketClient sc, string message)
        {
            if (sc != null && sc.Socket != null)
            {
                StreamResponse<string> streamResponse = new StreamResponse<string>()
                {
                    Channel = SystemChannel,
                    Data = message
                };
                await SendSystemMessageAsync(sc, streamResponse);
            }
        }

        public async Task SendSystemMessageAsync(SocketClient sc, StreamResponse<string> streamResponse)
        {
            if (sc != null && sc.Socket != null)
            {
                streamResponse.Channel = SystemChannel;
                await SendToClientAsync(sc.Socket, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(streamResponse)));
            }
        }

        public async Task SendSystemMessageAsync(SocketClient sc, StreamResponse<KeyStatePair> streamResponse)
        {
            if (sc != null && sc.Socket != null)
            {
                streamResponse.Channel = SystemChannel;
                await SendToClientAsync(sc.Socket, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(streamResponse)));
            }
        }

    }
}