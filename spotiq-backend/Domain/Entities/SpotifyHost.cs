using System.ComponentModel.DataAnnotations;


namespace spotiq_backend.Domain.Entities
{
    public class SpotifyHost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DeviceId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ClientSecret { get; set; } = string.Empty;


        [StringLength(256)]
        public string? AccessToken { get; set; }
        [StringLength(256)]
        public string? RefreshToken { get; set; }

        [StringLength(100)]
        public string? Url { get; set; }

        // Navigation props (linked from FK entities)
        public ICollection<Songwish>? Songwishes { get; set; }
        public ICollection<Poll>? Polls { get; set; }

    }
}
