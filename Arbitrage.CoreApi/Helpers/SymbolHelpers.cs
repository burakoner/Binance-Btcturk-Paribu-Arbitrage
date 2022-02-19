using Gizza.Extensions;

namespace Arbitrage.CoreApi.Helpers
{
    public static class SymbolHelpers
    {
        public static string FixName(string symbol)
        {
            return symbol.ToStringSafe()
            .Trim().ToUpperInvariant()
            .Replace(" ", "-")
            .Replace(".", "-")
            .Replace(",", "-")
            //.Replace("-", "")
            .Replace("_", "-")
            .Replace("/", "-")
            .Replace("\\", "-");
        }
    }
}
