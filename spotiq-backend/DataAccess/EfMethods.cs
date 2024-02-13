using spotiq_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Collections;


namespace spotiq_backend.DataAccess

{
    public class EfMethods
    {
        private readonly SpotiqContext _spotiqContext;
       
        public EfMethods(SpotiqContext spotiqContext)
        {
            _spotiqContext = spotiqContext;
        }
        #region Songwish methods
        public async Task<int> CreateSongwish(Songwish songwish)
        {
            _spotiqContext.Songwish.Add(songwish);
            await _spotiqContext.SaveChangesAsync();
            return songwish.Id;
        }

        public async Task<List<Songwish>> GetAllSongwishes()
        {
            return await _spotiqContext.Songwish.ToListAsync();
        }

        public async Task<Songwish?> GetSongwish(int id)
        {
            return await _spotiqContext.Songwish.FindAsync(id);
        }

        public async Task<bool> DeleteSongwish(int id)
        {
            Songwish? songwish = await _spotiqContext.Songwish.FindAsync(id);
            if (songwish == null) { return false; }

            _spotiqContext.Remove(songwish);
            await _spotiqContext.SaveChangesAsync();

            return true;
        }
        #endregion

        #region SpotifyHost methods
        public async Task<SpotifyHost> CreateSpotifyHost(SpotifyHost spotifyHost)
        {
            _spotiqContext.SpotifyHost.Add(spotifyHost);
            await _spotiqContext.SaveChangesAsync();
            return spotifyHost;
        }
        public async Task<SpotifyHost?> GetDefaultHost()
        {
            int defaultHostId = 1;
            return await GetHost(defaultHostId);
        }

        public async Task<SpotifyHost?> GetHost(int id)
        {
            return await _spotiqContext.SpotifyHost.FindAsync(id);
        }

        public async Task<List<SpotifyHost>> GetAllHosts()
        {
            return await _spotiqContext.SpotifyHost.ToListAsync();
        }

        public async Task<bool> DeleteHost(int id)
        {
            SpotifyHost? host = await _spotiqContext.SpotifyHost.Where(h => h.Id == id).SingleOrDefaultAsync();

            if (host == null)
            {
                return false;
            }
            else
            {
                _spotiqContext.Remove(host);
                await _spotiqContext.SaveChangesAsync();
                return true;
            }
        }
        #endregion

        #region Poll methods

        // GetPoll ikke i bruk 16.04 - slettes?
        public async Task<Poll?> GetPoll(int id)
        {
            Poll? poll = await _spotiqContext.Poll
                .Where(p => p.Id == id)
                .Include(p => p.PollSongs)!
                .ThenInclude(sw => sw.Songwish)
                .SingleOrDefaultAsync()
                ;

            return poll;
        }
        public async Task<Poll?> GetLatestPoll(int spotifyHostId)
        {
            // TODO: Vi må ta med spotifyHostId i søket!
            Poll? poll = await _spotiqContext.Poll
                        .Where(p => p.SpotifyHostId == spotifyHostId)
                        .Where(p => p.EndTime == _spotiqContext.Poll.Max(m => m.EndTime))
                        .FirstOrDefaultAsync();

            return poll;
        }

        public DateTime GetNextPollStartTime(Poll latestPoll)
        {
            return latestPoll.EndTime > DateTime.Now ? latestPoll.EndTime : DateTime.Now;
        }

        public async Task<PollInfo?> GetRunningPoll(int spotifyHostId)
        {
            SpotifyHost? spotifyHost = await GetHost(spotifyHostId);
            if (spotifyHost == null)
                return null;

            DateTime now = DateTime.Now;
            Poll? runningPoll = await _spotiqContext.Poll
                .Where(p => p.StartTime < now && p.EndTime > now
                    && p.SpotifyHostId == spotifyHostId)
                .SingleOrDefaultAsync();

            if (runningPoll == null)
                return null;

            PollInfo? pollInfo = await GetPollInfoMinimal(runningPoll.Id);

            return pollInfo;
        }

        public async Task AddToQ(string trackId, SpotifyHost spotifyHost)
        {
            SpotifyApi spotifyApi = new(spotifyHost);

            await spotifyApi.AddToQueue(trackId, SpotifyHost spotifyHost );

            // hent spotifYHost
            //SpotifyHost? spotifyHost = await GetDefaultHost();
            //if (spotifyHost == null) return;
            //await _spotifyApi.AddToQueue(trackId, accessToken, refreshToken, deviceId, 0);

        }

        public async Task<PollSongInfo?> GetWinner(int pollId)
        {
            Poll? poll = await _spotiqContext.Poll.FindAsync(pollId);
            if (poll == null)
            {
                Console.WriteLine("\nFra toppen EFm.GetWinner(): Poll er IKKE funnet ");
                return null;
            }
            Console.WriteLine("\nFra toppen EFm.GetWinner(): Poll er funnet ");
            if (poll.WinnerId > 0)
            {
                string trackId = poll.TrackSpotifyId!;
                PollSongInfo returnTrack; 
                // returner vinner
                Console.WriteLine("\nFra EFm.GetWinner(): Vinner allerede kåret - bare hent vinneren");
                
                PollInfo? pollInfo = await GetPollInfoMinimal(pollId);
                foreach(PollSongInfo track in pollInfo!.Tracks!)
                {
                    if (track.SpotifyId == trackId)
                        return track;
                    
                }
                return null;
            }

            PollSongInfo? psi = await CreateWinner(pollId);
            Console.WriteLine("\nFra Ny GetWinner(): ");
            Console.WriteLine("****" + psi!.SongName + "****");

            SpotifyHost? host = await GetHost(poll.SpotifyHostId);
            if (host == null) return null;
            Console.WriteLine("\n***Spotify id fra GetWinner(): " + poll.TrackSpotifyId);

            await AddToQ(poll.TrackSpotifyId!, !);

            return psi;
        }
        public async Task<PollSongInfo?> CreateWinner(int pollId)
        {
            PollInfo? pollInfo = await GetPollInfoMinimal(pollId);
            if (pollInfo == null || pollInfo.Tracks == null) return null;

            //List<PollSongInfo> pollWinners = new(); // 
            PollSongInfo? singlePollWinner = null;
            pollInfo.Tracks = (List<PollSongInfo>)pollInfo.Tracks;
            int winnerId = 0;
            int maxVotes = 0;

            foreach (PollSongInfo psi in pollInfo.Tracks)
            {
                if (psi.VotesCount > maxVotes) // Single winner. TODO: accept ties with >=
                {
                    //pollWinners.Add(psi); // For ties
                    singlePollWinner = psi;
                    winnerId = psi.PollSongId;
                    maxVotes = psi.VotesCount;
                }
            }
            // If no votes, draw random winner
            Random rnd = new();
            if (maxVotes == 0)
            {
                int index = rnd.Next(0, pollInfo.Tracks.Count);
                singlePollWinner = pollInfo.Tracks[index];
            }

            Poll? poll = await _spotiqContext.Poll.FindAsync(pollId);
            if (poll == null) { return null; }

            poll.WinnerId = 1;
            poll.TrackSpotifyId = singlePollWinner!.SpotifyId;
            poll.ArchivedTime = DateTime.Now;
            _spotiqContext.Update(poll);
            await _spotiqContext.SaveChangesAsync();
            //pollInfo.Tracks[0].
             //Set winner in Poll
            //Poll? poll = _spotiqContext

            return singlePollWinner;
        }

        public async Task<PollInfo?> GetPollInfoMinimal(int id)
        {
            var pollSongData = await
                (from ps in _spotiqContext.PollSong
                 join p in _spotiqContext.Poll
                 on ps.PollId equals p.Id
                 join sw in _spotiqContext.Songwish
                 on ps.SongwishId equals sw.Id
                 where ps.PollId == id
                 select new
                 {
                     ps.PollId,
                     ps.VotesCount,
                     p.StartTime,
                     p.EndTime,
                     PollSongId = ps.Id,
                     SongName = sw.Name,
                     sw.SpotifyId,
                     sw.ArtistName,
                     SpotifyHostId = p.SpotifyHost!.Id,
                     SpotifyHostName = p.SpotifyHost!.Name
                 }
                ).ToListAsync();

            if (pollSongData.Count == 0) { return null; }
            List<PollSongInfo> pollSongs = new();

            Console.WriteLine($"\n{DateTime.Now.ToString("HH:mm:ss", CultureInfo.CurrentCulture)} - Debug fra GetPollMinimal: ----------");
            Console.Write($"\nPollId: {pollSongData[0].PollId}");
            Console.WriteLine($"\tHost name: {pollSongData[0].SpotifyHostName}");
            foreach (var ps in pollSongData)
            {
                pollSongs.Add(new PollSongInfo
                {
                    PollSongId = ps.PollSongId,
                    PollId = ps.PollId,
                    SongName = ps.SongName,
                    ArtistName = ps.ArtistName,
                    VotesCount = ps.VotesCount,
                    SpotifyId = ps.SpotifyId,
                });

                Console.Write($"\nPollSongId: {ps.PollSongId}");
                Console.Write($"\tSong name: {ps.SongName}");
            }
            PollInfo poll = new()
            {
                PollId = pollSongData[0].PollId,
                StartTime = pollSongData[0].StartTime,
                EndTime = pollSongData[0].EndTime,
                SpotifyHostId = pollSongData[0].SpotifyHostId,
                SpotifyHostName = pollSongData[0].SpotifyHostName,
                Tracks = pollSongs
            };

            return poll;
        }


        public async Task<int> CreatePoll(int spotifyHostId)
        {
            DateTime startTime = DateTime.Now;
            int delayBeforeNextPoll = 10;
            int pollDurationInMinutes = 1;
            int pollDurationInSeconds = 30;

            Poll? latestPoll = await GetLatestPoll(spotifyHostId);
            if (latestPoll != null)
            {
                startTime = GetNextPollStartTime(latestPoll).AddSeconds(delayBeforeNextPoll);
            }
            SpotifyHost? spotifyHost = await GetHost(spotifyHostId);
            if (spotifyHost == null) { return -1; }

            // Hent ut alle aktuelle Songwish for spotifyHost
            List<Songwish>? selectedSongwishes = await SelectSongsToPoll(spotifyHostId);
            if (selectedSongwishes is null) { return 0; }

            Poll poll = new()
            {
                SpotifyHostId = spotifyHostId,
                StartTime = startTime,
                //EndTime = startTime.AddMinutes(pollDurationInMinutes)
                EndTime = startTime.AddSeconds(pollDurationInSeconds),
            };

            _spotiqContext.Poll.Add(poll);
            await _spotiqContext.SaveChangesAsync();
            //return poll.Id;
            // Lag liste for PollSongs som skal inserteres
            List<PollSong> pollSongs = new();

            // Kjør løkke for å:
            // -    Oppdatere SelectedForVoteTime og ArchivedTime
            // -    Legg selected Songwises som entries i PollSong

            foreach (Songwish songwish in selectedSongwishes)
            {
                songwish.ArchivedTime = DateTime.Now;
                songwish.SelectedForVoteTime = DateTime.Now;
                _spotiqContext.Songwish.Update(songwish);
                // Insert PollSongs
                _spotiqContext.PollSong.Add(new PollSong { PollId = poll.Id, SongwishId = songwish.Id });
            }
            await _spotiqContext.SaveChangesAsync();
            return poll.Id;
        }

        public async Task<bool> DeletePoll(int id)
        {
            Poll? poll = await _spotiqContext.Poll.Where(p => p.Id == id).SingleOrDefaultAsync();

            if (poll == null)
            {
                return false;
            }
            else
            {
                _spotiqContext.Remove(poll);
                await _spotiqContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<List<Poll>> GetPollsByDevice(string deviceId)
        {
            List<Poll> polls = await _spotiqContext.Poll
                        .Include(p => p.SpotifyHost)
                        .Where(sh => sh.SpotifyHost!.DeviceId == deviceId)
                        .ToListAsync();

            return polls;
        }
        public async Task<List<Poll>> GetAllPolls(int? spotifyHostId = 0)
        {
            List<Poll> polls = spotifyHostId > 0
                ? await _spotiqContext.Poll.ToListAsync()
                : await _spotiqContext.Poll.Where(p => p.SpotifyHost!.Id == spotifyHostId).ToListAsync();

            return polls;
        }
        public async Task<List<Songwish>?> SelectSongsToPoll(int spotifyHostId, int selectCount = 3)
        {
            // Get all songs where archivedTime and SelectedForVote is null. Migth remove one of them later
            List<Songwish> activeSongwishes = await _spotiqContext.Songwish
                .Where(sw => sw.SelectedForVoteTime == null && sw.ArchivedTime == null && sw.SpotifyHost!.Id == spotifyHostId)
                .ToListAsync();


            // Returner tom liste hvis antall tilgjengelig songwish er mindre enn selectCount
            if (activeSongwishes.Count < selectCount) { return null; }

            // Trekk ut spor tilfeldig i forhold til antall elementer i lista
            Random rnd = new Random();
            List<Songwish> selectedSongs = new();

            Console.WriteLine(string.Format("\nTilfeldig uttrekk - {0}", DateTime.Now.ToString("HH:mm:ss", new CultureInfo("nb-NO"))));

            for (int i = 0; i < selectCount; i++)
            {
                // Unngå evig loop når duplikater fjernes
                if (activeSongwishes.Count + selectedSongs.Count < selectCount)
                {
                    Console.WriteLine("Not enough song for new poll after duplicate removal");
                    return null;
                }

                bool addSong = true;
                int index = rnd.Next(0, activeSongwishes.Count);
                foreach (Songwish uniqueSong in selectedSongs)
                {
                    if (uniqueSong.SpotifyId == activeSongwishes[index].SpotifyId)
                    {
                        addSong = false;
                    }
                }
                if (!addSong)
                {
                    Console.WriteLine($"spotifyId {activeSongwishes[index].SpotifyId} - " +
                                $"{activeSongwishes[index].Name} already exists. Remove from source list and skip to next draw to avoid duplicate tracks");

                    activeSongwishes.Remove(activeSongwishes[index]);
                    i--;
                    continue;
                }
                selectedSongs.Add(activeSongwishes[index]);
                Console.WriteLine($"Random Index song: id {activeSongwishes[index].Id} - {activeSongwishes[index].Name}");
                // Fjern den songwish som akkurat er lagt til i utvalgte slik at det blir tre ulike i pollen
                activeSongwishes.RemoveAt(index);
            }

            return selectedSongs;
        }
        #endregion
        #region Votes methods
        public async Task<int> IncrementVoteCount(int pollSongId)
        {
            PollSong? pollSong = await _spotiqContext.PollSong.FindAsync(pollSongId);
            if (pollSong == null) { return -1; }

            try
            {
                pollSong.VotesCount++;
                _spotiqContext.Update(pollSong);
                await _spotiqContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error from IncrementVoteCount: \n" + ex);
                return 0;
            }
            return pollSong.VotesCount;
        }
        #endregion


    }

    // Helper class for returning only neccessary data to client
    public class PollInfo
    {
        public int PollId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int SpotifyHostId { get; set; }
        public string SpotifyHostName { get; set; } = string.Empty;
        public IList<PollSongInfo>? Tracks { get; set; }
    }

    public class PollSongInfo
    {
        public int PollSongId { get; set; }
        public int PollId { get; set; }
        public string SongName { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string SpotifyId { get; set; } = string.Empty;
        public int VotesCount { get; set; }
    }


}