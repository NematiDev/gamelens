using GameLens.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameLens.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewLike> ReviewsLike { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // a user can like a review only once
            builder.Entity<ReviewLike>()
                .HasIndex(rl => new { rl.ReviewId, rl.UserId })
                .IsUnique();

            // Configure primary keys (no auto-increment, set by RAWG API)
            builder.Entity<Game>()
                .Property(g => g.Id)
                .ValueGeneratedNever();

            builder.Entity<Genre>()
                .Property(g => g.Id)
                .ValueGeneratedNever();

            builder.Entity<Platform>()
                .Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Entity<Developer>()
                .Property(d => d.Id)
                .ValueGeneratedNever();

            builder.Entity<Publisher>()
                .Property(p => p.Id)
                .ValueGeneratedNever();

            // Configure indexes
            builder.Entity<Game>()
                .HasIndex(g => g.Id)
                .IsUnique();

            builder.Entity<Game>()
                .HasIndex(g => g.Name);

            builder.Entity<Genre>()
                .HasIndex(g => g.Id)
                .IsUnique();

            builder.Entity<Platform>()
                .HasIndex(p => p.Id)
                .IsUnique();

            builder.Entity<Developer>()
                .HasIndex(d => d.Id)
                .IsUnique();

            builder.Entity<Publisher>()
                .HasIndex(p => p.Id)
                .IsUnique();
        }
    }
}
