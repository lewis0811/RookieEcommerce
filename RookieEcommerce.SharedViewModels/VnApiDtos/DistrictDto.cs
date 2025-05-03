using System.Text.Json.Serialization;

namespace RookieEcommerce.SharedViewModels.VnApiDtos
{
    public class DistrictDto
    {
        [JsonPropertyName("_id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("name_with_type")]
        public string? NameWithType { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("path_with_type")]
        public string? PathWithType { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("parent_code")]
        public string? ParentCode { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}