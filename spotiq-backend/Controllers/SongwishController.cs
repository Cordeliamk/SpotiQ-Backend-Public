using Microsoft.AspNetCore.Mvc;
using spotiq_backend.DataAccess;
using spotiq_backend.Domain;
using spotiq_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace spotiq_backend.Controllers
{
    [ApiController]
    [Route("api/songwishes")]

    public class SongwishController : ControllerBase
    {
        private readonly SpotiqContext _context;

        public SongwishController(SpotiqContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            EfMethods efMethods = new(_context);
            return Ok(await efMethods.GetAllSongwishes());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get(int id)
        {
            EfMethods efMethods = new(_context);
            Songwish? songwish = await efMethods.GetSongwish(id);
            if (songwish == null)
            {
                return NotFound();
            }

            return Ok(songwish);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Songwish songwish)
        {
            Songwish newSongwish = new()
            {
                SpotifyId = songwish.SpotifyId,
                SpotifyHostId = songwish.SpotifyHostId,
                Name = songwish.Name,
                ArtistName = songwish.ArtistName,
                EnteredTime = DateTime.Now
            };

            EfMethods efMethods = new(_context);
            int songWishId = await efMethods.CreateSongwish(newSongwish);

            return Ok(newSongwish);
        }
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(int id)
        {
            EfMethods efMethods = new(_context);
            bool songwishDeleted = await efMethods.DeleteSongwish(id);
            if (!songwishDeleted)
                return NotFound("Songwish not found - nothing deleted");

            return Ok("Songwish with id " + id + " was deleted");
        }
    }
}