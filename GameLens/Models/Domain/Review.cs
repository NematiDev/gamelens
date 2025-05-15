using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public virtual ICollection<ReviewLike> Likes { get; set; } = new List<ReviewLike>();
        public ICollection<ReviewComment> Comments { get; set; } = new List<ReviewComment>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("Game")]
        public int GameId { get; set; }
        public Game? Game { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
    }
}
