using Gizza.Extensions;
using System;

namespace Arbitrage.CoreApi
{
    public static class AppStatic
    {
        public static long Epoch => Now.ToUnixTimeMilliSeconds();

        public static long EpochSeconds => Epoch / 1000;

        public static DateTime Now => DateTime.UtcNow;

        public static DateTime ValidUntil => new DateTime(2099, 12, 31, 23, 59, 59, 999);
    }
}
