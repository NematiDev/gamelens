namespace GameLens.Models.Domain
{
    public class Game
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? AverageRating { get; set; }
        public int? Metacritic { get; set; }
        public DateTime? Released { get; set; }
        public string? LocalBackgroundImage { get; set; }
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<Platform> Platforms { get; set; } = new List<Platform>();
        public ICollection<Developer> Developers { get; set; } = new List<Developer>();
        public ICollection<Publisher> Publishers { get; set; } = new List<Publisher>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
