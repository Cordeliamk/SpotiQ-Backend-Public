using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spotiq_backend.Domain.Entities
{
    public class Songwish
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        [Required]
        public string SpotifyId { get; set; } = string.Empty;

        [MaxLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        [Required]
        public string ArtistName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? UserSession { get; set; }

        [Required]
        public DateTime EnteredTime { get; set; }

        public DateTime? QueuedTime { get; set; }

        public DateTime? SelectedForVoteTime { get; set; }

        public DateTime? ArchivedTime { get; set; }

        // FK and navigation prop
        public int SpotifyHostId { get; set; }
        public SpotifyHost? SpotifyHost { get; set; }

        public ICollection<PollSong>? PollSongs { get; set; }

    }
}
