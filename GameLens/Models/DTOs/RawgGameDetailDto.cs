using System.Text.Json.Serialization;

namespace GameLens.Models.DTOs
{
    // DTO for RAWG API Game Details
    public class RawgGameDetailDto : RawgGameSearchResultDto
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("genres")]
        public List<RawgNamedEntityDto> Genres { get; set; }

        [JsonPropertyName("platforms")]
        public List<RawgPlatformWrapperDto> Platforms { get; set; }

        [JsonPropertyName("developers")]
        public List<RawgNamedEntityDto> Developers { get; set; }

        [JsonPropertyName("publishers")]
        public List<RawgNamedEntityDto> Publishers { get; set; }
    }
}
