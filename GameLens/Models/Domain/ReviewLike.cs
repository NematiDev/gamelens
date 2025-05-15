namespace GameLens.Models.Domain
{
    public class ReviewLike
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int ReviewId { get; set; }
        public Review? Review { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
