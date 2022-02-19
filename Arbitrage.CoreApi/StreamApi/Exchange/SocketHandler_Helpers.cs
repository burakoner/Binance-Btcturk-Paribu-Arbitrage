using Arbitrage.CoreApi.Enums;
using Gizza.Data.Attributes;
using System.Collections.Generic;

namespace Arbitrage.CoreApi.StreamApi.Exchange
{
    public partial class SocketHandler
    {
        private string ChannelName(StreamChannelType section, params string[] args)
        {
            List<string> nameParts = new List<string>
            {
                section.GetLabel()
            };
            if (args != null)
            {
                foreach (string arg in args)
                {
                    nameParts.Add(arg);
                }
            }

            return string.Join(".", nameParts);
        }
    }
}