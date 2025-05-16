using System.Text.Json.Serialization;

namespace GameLens.Models.DTOs
{
    // DTO for RAWG API Game Search Result
    public class RawgGameSearchResultDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("background_image")]
        public string? BackgroundImage { get; set; }

        [JsonPropertyName("metacritic")]
        public int? Metacritic { get; set; }

        [JsonPropertyName("released")]
        public DateTime? Released { get; set; }
    }
}
