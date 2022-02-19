using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Arbitrage.CoreApi.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// Retrieves the raw body as a byte array from the Request.Body stream
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<byte[]> GetRawBodyBytesAsync(this HttpRequest request)
        {
            using (MemoryStream ms = new MemoryStream(2048))
            {
                await request.Body.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
