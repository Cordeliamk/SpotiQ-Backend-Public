using spotiq_backend.DataAccess;
using spotiq_backend.Domain.Entities;
using System.Globalization;

namespace spotiq_backend.Workers
{
    internal interface IPollService
    {
        Task ExecuteAsync(CancellationToken stoppingToken);
        Task RunStuff(CancellationToken stoppingToken);

    }

    public class PollService : IPollService
    {
        private const int createPollDelay = 1 * 10 * 1000;
        private readonly SpotiqContext _context;

        public PollService(SpotiqContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int spotifyHostId = 1;
            string message;

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(createPollDelay, stoppingToken);
                try
                {
                    EfMethods efMethods = new(_context);
                    
                        int pollId = await efMethods.CreatePoll(spotifyHostId);
                        switch (pollId)
                        {
                            case -1:
                                message = "No host registered with id " + spotifyHostId;
                                break;
                            case 0:
                                message = "Not enough songwishes for host to create poll";
                                break;
                            default:
                                message = "New poll: " + pollId;
                                break;
                        };
                    Console.WriteLine();
                    Console.WriteLine(DateTime.Now.ToString("HH:mm:ss", CultureInfo.CurrentCulture));
                    Console.WriteLine(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("********** PollWorker reports error ***********");
                    Console.WriteLine(e);
                    Console.WriteLine("****************** End of error ***************");
                }
                //await RunStuff(stoppingToken);

            }
        }

        public Task RunStuff(CancellationToken stoppingToken)
        {

            Console.WriteLine("Fra PollService Delay 1: " +
                DateTime.Now.ToString("HH:mm:ss", CultureInfo.CurrentCulture));

            return Task.FromResult("OK");
        }
    }

    public class PollServiceConsumer : BackgroundService
    {
        public IServiceProvider Services { get; }

        public PollServiceConsumer(IServiceProvider services)
        {
            Services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Fra PollServiceConsumer : Service is running");
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var scopedProccesingService = scope.ServiceProvider.GetRequiredService<IPollService>();
                await scopedProccesingService.ExecuteAsync(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Fra PollServiceConsumer : Service is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}
