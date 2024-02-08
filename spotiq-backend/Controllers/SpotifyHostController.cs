using Microsoft.AspNetCore.Mvc;
using spotiq_backend.DataAccess;
using spotiq_backend.Domain;
using spotiq_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace spotiq_backend.Controllers
{
    [ApiController]
    [Route("api/spotifyHosts")]

    public class SpotifyHostController : ControllerBase
    {
        private readonly SpotiqContext _context;

        public SpotifyHostController(SpotiqContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrives alle spotify host
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetAllHosts()
        {
            EfMethods efMethods = new(_context);
            List<SpotifyHost> spotifyHosts = await efMethods.GetAllHosts();
            if(spotifyHosts.Count == 0)
            {
                return NotFound("No spotify hosts found");
            }
            return Ok(spotifyHosts);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get(int id)
        {
            EfMethods efMethods = new(_context);
            SpotifyHost? spotifyHost = await efMethods.GetHost(id);
            if (spotifyHost == null)
            {
                return NotFound("Host id not found");
            }

            return Ok(spotifyHost);
        }

        /// <summary>
        /// Creates a new spotify host
        /// </summary>
        /// <param name="spotifyHost">Json object</param>
        /// <returns>The created spotifyHost, including Identity value from the database</returns>
        /// <remarks>
        /// Example request:
        /// 
        /// <pre>
        /// {
        ///     "deviceId": "device_id_til_Martin",
        ///     "name": "Martins mobil",
        ///     "clientId": "Martins-Client-Id",
        ///     "clientSecret": "Martins-Client-Seecret",
        ///     "url": "url til Martins mobil",
        ///     "accessToken" : "Martins-access-token",
        ///     "refreshToken" : "Martins-refresh-token"
        /// }
        /// </pre>
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Create(SpotifyHost spotifyHost)
        {
            SpotifyHost newSpotifyHost = new SpotifyHost
            {
                DeviceId = spotifyHost.DeviceId,
                ClientId = spotifyHost.ClientId,
                ClientSecret = spotifyHost.ClientSecret,
                Name = spotifyHost.Name,
                Url = spotifyHost.Url,
                AccessToken = spotifyHost.AccessToken,
                RefreshToken = spotifyHost.RefreshToken
            };

            EfMethods efMethods = new(_context);
            await efMethods.CreateSpotifyHost(newSpotifyHost);

            return Ok(newSpotifyHost);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteSpotifyHost(int id)
        {
            EfMethods efMethods = new(_context);
            SpotifyHost? spotifyHost = await efMethods.GetHost(id);
            if (spotifyHost == null)
            {
                return NotFound("Host id not found");
            }

            bool deleted = await efMethods.DeleteHost(id);
            
            return Ok(deleted);
        }
    }
    
}