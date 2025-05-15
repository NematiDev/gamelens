using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameLens.Models.Domain
{
    public class ReviewComment
    {
        public int Id { get; set; }
        [Required]
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set;} = DateTime.UtcNow;
        [ForeignKey("Review")]
        public int ReviewId { get; set; }
        public Review? Review { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
    }
}
