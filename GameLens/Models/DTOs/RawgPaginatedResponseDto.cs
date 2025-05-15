using System.Text.Json.Serialization;

namespace GameLens.Models.DTOs
{
    // DTO for paginated RAWG API responses
    public class RawgPaginatedResponseDto<T>
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("previous")]
        public string? Previous { get; set; }

        [JsonPropertyName("results")]
        public List<T>? Results { get; set; }
    }
}
