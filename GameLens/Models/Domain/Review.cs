using System.ComponentModel.DataAnnotations;

namespace GameLens.Models.Domain
{
    public class Review
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120, ErrorMessage = "Review title must be under 100 characters.")]
        public string? Title { get; set; }
        [Required]
        [StringLength(1000, ErrorMessage = "Review content must be under 1000 characters.")]
        public string? Content { get; set; }
        [Range(1, 10, ErrorMessage = "Rating must be between 1 and 10.")]
        public int Rating { get; set; }
        public int LikeCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int GameId { get; set; }
        public Game? Game { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
    }
}
