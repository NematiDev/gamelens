using System.Text.Json.Serialization;

namespace GameLens.Models.DTOs
{
    // DTO for named entities (Genres, Developers, Publishers)
    public class RawgNamedEntityDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
