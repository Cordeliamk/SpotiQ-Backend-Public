using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spotiq_backend.Domain.Entities
{
    public class PollSong
    {
        public int Id { get; set; }
        public int VotesCount { get; set; }

        // Two FKs and navigation props

        //[ForeignKey(nameof(Poll))]
        public int PollId { get; set; }
        public Poll? Poll { get; set; }

        //[ForeignKey(nameof(Songwish))]
        public int SongwishId { get; set; }
        public Songwish? Songwish { get; set; }

    }
}
