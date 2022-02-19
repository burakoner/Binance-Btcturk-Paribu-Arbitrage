using Arbitrage.CoreApi.Converters;
using Newtonsoft.Json;
using System;

namespace Arbitrage.CoreApi.StreamApi.Objects
{
    public class StreamResponse<T>
    {
        /// <summary>
        /// Request Identifier
        /// </summary>
        [JsonProperty("i", NullValueHandling = NullValueHandling.Ignore)]
        public long? Identifier { get; set; }

        /// <summary>
        /// Channel Name
        /// </summary>
        [JsonProperty("c")]
        public string Channel { get; set; }

        /// <summary>
        /// Event Time
        /// </summary>
        [JsonProperty(propertyName: "t", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(TimestampMilliSecondsConverter))]
        public DateTime Time { get; set; }

        /*
        /// <summary>
        /// DataModel
        /// </summary>
        [JsonProperty(propertyName: "m", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(EnumLabelConverter<StreamDataModel>))]
        // [JsonProperty(propertyName: "dm", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(StreamDataModelConverter))]
        public StreamDataModel DataModel { get; set; }
        */

        /// <summary>
        /// Data
        /// </summary>
        [JsonProperty(propertyName: "d", NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        [JsonProperty(propertyName: "e", NullValueHandling = NullValueHandling.Ignore)]
        public StreamError Error { get; set; }

        public StreamResponse()
        {
            Time = AppStatic.Now;
        }

        public StreamResponse(T data)
        {
            Time = AppStatic.Now;
            Data = data;
        }

        public StreamResponse(StreamError error)
        {
            Time = AppStatic.Now;
            Error = error;
        }

        public StreamResponse(string channel)
        {
            Channel = channel;
            Time = AppStatic.Now;
        }

        public StreamResponse(string channel, T data)
        {
            Channel = channel;
            Time = AppStatic.Now;
            Data = data;
        }

        public StreamResponse(long identifier, string channel, T data)
        {
            Identifier = identifier;
            Channel = channel;
            Time = AppStatic.Now;
            Data = data;
        }

        public StreamResponse(string channel, StreamError error)
        {
            Channel = channel;
            Time = AppStatic.Now;
            Error = error;
        }
    }
}
