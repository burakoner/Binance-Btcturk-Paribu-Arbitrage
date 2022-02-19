using Newtonsoft.Json;

namespace Arbitrage.CoreApi.Models
{
    public class ApiResponse
    {
        [JsonProperty("success")]
        public bool Success => Error == null;

        [JsonProperty(propertyName: "data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }

        [JsonProperty(propertyName: "error", NullValueHandling = NullValueHandling.Ignore)]
        public ErrorResponse Error { get; set; }

        public ApiResponse()
        {
            Data = null;
            Error = null;
        }

        public ApiResponse(object data)
        {
            Data = data;
            Error = null;
        }

        public ApiResponse(ErrorResponse error)
        {
            Data = null;
            Error = error;
        }

        public ApiResponse(object data, ErrorResponse error)
        {
            Data = data;
            Error = error;
        }
    }

    /// <summary>
    /// Bu Class'ı yalnızca Swagger dokümantasyonu için SwaggerResponse Attributeleri Kullanımı için tasarladım.
    /// Aslında diğer alanlarda da kullanılabilir ama karışıklık yaratmaya gerek yok. Normal API yanıtlarında üsttekini kullanmak daha doğru.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        [JsonProperty("success")]
        public bool Success => Error == null;

        [JsonProperty(propertyName: "data", NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }

        [JsonProperty(propertyName: "error", NullValueHandling = NullValueHandling.Ignore)]
        public ErrorResponse Error { get; set; }

        public ApiResponse()
        {
            Error = null;
        }

        public ApiResponse(T data)
        {
            Data = data;
            Error = null;
        }

        public ApiResponse(ErrorResponse error)
        {
            Error = error;
        }

        public ApiResponse(T data, ErrorResponse error)
        {
            Data = data;
            Error = error;
        }
    }
}
