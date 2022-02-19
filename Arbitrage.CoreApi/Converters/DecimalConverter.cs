using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Arbitrage.CoreApi.Converters
{
    public class DecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Bu noktadan sonra otomatik olarak hane sayısı ekliyor
            // 0.1000000000m => 0.1
            // 0.0100000000m => 0.01
            // 0.0010000000m => 0.001
            // 0.0001000000m => 0.0001
            // 0.0000100000m => 0.00001
            // 0.0000010000m => 0.000001
            // 0.0000001000m => 0.0000001
            // 0.0000000100m => 0.00000001
            // 0.0000000010m => 0.000000001
            // 0.0000000001m => 0.0000000001
            // writer.WriteRawValue(((decimal)value).Normalize().ToString(CultureInfo.InvariantCulture));

            // Bu ise noktadan sonra standart 8 basamak ekliyor
            // 0.1000000000m => 0.10000000
            // 0.0100000000m => 0.01000000
            // 0.0010000000m => 0.00100000
            // 0.0001000000m => 0.00010000
            // 0.0000100000m => 0.00001000
            // 0.0000010000m => 0.00000100
            // 0.0000001000m => 0.00000010
            // 0.0000000100m => 0.00000001
            // 0.0000000010m => 0.00000000
            // 0.0000000001m => 0.00000000
            //writer.WriteRawValue(((decimal)value).ToString("F"+AppConstants.PRICE_MINIMUM_DECIMAL_PLACES, CultureInfo.InvariantCulture));
            writer.WriteRawValue(((decimal)value).ToString("F8", CultureInfo.InvariantCulture));

            // Hangisi daha mantıklı karar veremedim.
            // Şimdilik ikincisi olsun
        }
    }
}