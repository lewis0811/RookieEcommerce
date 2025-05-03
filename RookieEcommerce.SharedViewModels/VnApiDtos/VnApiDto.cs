using System.Text.Json.Serialization;

namespace RookieEcommerce.SharedViewModels.VnApiDtos
{
    public class VnApiDto<T>
    {
        [JsonPropertyName("exitcode")]
        public int ExitCode { get; set; }

        [JsonPropertyName("data")]
        public VnApiDataContainer<T>? Data { get; set; } // Dùng kiểu generic T

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class VnApiDataContainer<T>
    {
        [JsonPropertyName("nItems")]
        public int NItems { get; set; }

        [JsonPropertyName("nPages")]
        public int NPages { get; set; }

        [JsonPropertyName("data")]
        public List<T>? Data { get; set; }
    }
}