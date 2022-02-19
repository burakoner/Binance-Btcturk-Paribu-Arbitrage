namespace Arbitrage.CoreApi.Helpers
{
    public static class BitHelpers
    {
        public static string ByteToString(byte[] buff)
        {
            string result = "";
            foreach (byte t in buff)
            {
                result += t.ToString("X2"); /* hex format */
            }

            return result;
        }
    }
}
