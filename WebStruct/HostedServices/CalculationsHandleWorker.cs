using CompModels.ModelsAlghoritms.Handler;

namespace WebStruct.HostedServices
{
    public class CalculationsHandleWorker : BackgroundService
    {
        private readonly ILogger<CalculationsHandleWorker> _logger;
        private readonly ICalculateRequestsHandler calculateRequestsHandler;

        public CalculationsHandleWorker(ILogger<CalculationsHandleWorker> logger, ICalculateRequestsHandler calculateRequestsHandler)
        {
            _logger = logger;
            this.calculateRequestsHandler = calculateRequestsHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CalculationsHandleWorker is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("CalculationsHandleWorker is doing work at: {time}", DateTimeOffset.Now);

                    await calculateRequestsHandler.HandleAsync();

                    await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            _logger.LogInformation("CalculationsHandleWorker is stopping.");
        }
    }
}
