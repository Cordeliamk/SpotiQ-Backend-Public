using Microsoft.AspNetCore.Mvc;
using spotiq_backend.DataAccess;
using spotiq_backend.Domain;
using spotiq_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;

namespace spotiq_backend.Controllers
{
    [ApiController]
    [Route("api/polls")]

    public class PollController : ControllerBase
    {
        private readonly SpotiqContext _context;

        public PollController(SpotiqContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all polls for device, both active and past ones
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns>List of Polls</returns>
        [HttpGet()]
        public async Task<IActionResult> GetAll(string deviceId)
        {
            EfMethods efMethods = new(_context);
            return Ok(await efMethods.GetPollsByDevice(deviceId));
        }

        /// <summary>
        /// Gets the current/running poll for the given spotifyHost
        /// </summary>
        /// <param name="spotifyHostId"></param>
        /// <returns>PollInfo json-object</returns>
        [HttpGet("current/{spotifyHostId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetRunning(int spotifyHostId)
        {
            EfMethods efMethods = new(_context);
            PollInfo? pollInfo = await efMethods.GetRunningPoll(spotifyHostId);
            if (pollInfo == null)
            {
                return NotFound("No running poll found");
            }

            return Ok(pollInfo);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Get(int id)
        {
            EfMethods efMethods = new(_context);
            PollInfo? poll = await efMethods.GetPollInfoMinimal (id);
            if (poll == null)
            {
                return NotFound();
            }

            return Ok(poll);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int spotifyHostId)
        {
            EfMethods efMethods = new(_context);

            int pollId = await efMethods.CreatePoll(spotifyHostId);
            return pollId switch
            {
                -1 => NotFound("No host registered with id " + spotifyHostId),
                0 => UnprocessableEntity("Not enough songwishes for host to create poll"),
                _ => Ok(pollId),
            };
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeletePoll(int id)
        {
            EfMethods efMethods = new(_context);
            
            bool deleted = await efMethods.DeletePoll(id);
            if (!deleted) return NotFound("Bad poll id - no poll deleted");

            return Ok(deleted);
        }

        /// <summary>
        /// Increments the count of votes for a given track in a poll
        /// </summary>
        /// <param name="pollSongId"></param>
        /// <returns>True if votes count was incremented</returns>
        [HttpPost("votes/{pollSongId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateVote(int pollSongId)
        {
            EfMethods efMethods = new(_context);

            int votesCount = await efMethods.IncrementVoteCount(pollSongId);
            return votesCount switch
            {
                -1 => NotFound("No pollSong registered with id " + pollSongId),
                0 => StatusCode(StatusCodes.Status500InternalServerError),
                _ => Ok(votesCount),
            };
        }
        /// <summary>
        /// Return winner of a given poll
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns>PollSongInfo if winner found</returns>
        [HttpGet("winner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetWinner(int pollId)
        {
            EfMethods efMethods = new(_context);
            Console.WriteLine("\n--------------Før api/polls/GetWinner()");
            PollSongInfo? psi = await efMethods.GetWinner(pollId);
            Console.WriteLine("\n--------------Etter api/polls/GetWinner()");
            if (psi == null) return NotFound("No poll or winner???");

            return Ok(psi);
        }
        //[HttpGet("winner/addtoq/")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        //[ProducesDefaultResponseType]
        ////public async Task<IActionResult> AddToQ()
        ////{
        ////    EfMethods efMethods = new(_context);
        ////    await efMethods.AddToQ("", "", "", "");
        ////    //if (psi == null) return NotFound("No poll or winner???");

        ////    return Ok();
        ////}
    }
}