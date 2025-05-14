using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GameLens.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        [Url]
        public string? ProfilePictureUrl { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
