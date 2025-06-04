using Microsoft.EntityFrameworkCore;
using ProfileService.Models;

namespace ProfileService.Data
{
    public class ProfileDbContext : DbContext
    {
        public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options) { }

        public DbSet<Profile> Profile { get; set; }
    }
}
