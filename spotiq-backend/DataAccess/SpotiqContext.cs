using spotiq_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace spotiq_backend.DataAccess
{
    public class SpotiqContext : DbContext
    {
        public SpotiqContext(DbContextOptions<SpotiqContext> options)
           : base(options)
        {
        }
       
        public DbSet<Songwish> Songwish { get; set; } = default!;

        public DbSet<SpotifyHost> SpotifyHost { get; set; } = default!;
        public DbSet<Poll> Poll { get; set; } = default!;
        public DbSet<PollSong> PollSong { get; set; } = default!;
    }
}
