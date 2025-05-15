using System.Text.Json.Serialization;

namespace GameLens.Models.DTOs
{
    // DTO for platform wrapper
    public class RawgPlatformWrapperDto
    {
        [JsonPropertyName("platform")]
        public RawgNamedEntityDto? Platform { get; set; }
    }
}
