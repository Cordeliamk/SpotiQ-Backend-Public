namespace spotiq_backend.Domain.Entities
{
    public class Poll
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; } 
        public DateTime EndTime { get; set; }
        public DateTime? ArchivedTime { get; set; }
        public int WinnerId { get; set; }
        public string? TrackSpotifyId { get; set; } = string.Empty;

        // FK and navigation prop
        public int SpotifyHostId { get; set; }

        public SpotifyHost? SpotifyHost { get; set; }
        public ICollection<PollSong>? PollSongs { get; set; }

    }
}
