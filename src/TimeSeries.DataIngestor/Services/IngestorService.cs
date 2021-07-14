using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.DataStore;
using TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.DataIngestor.Services
{
    public class IngestorService : BackgroundService, IDataProcessor<RawTimeSeriesSource>
    {
        private readonly ILogger<IngestorService> _logger;
        private readonly IBusControl _messageBus;
        private readonly IWriteData<RawTimeSeries> _dataStore;
        private readonly CancellationTokenSource _tokenSource;

        public IngestorService(ILogger<IngestorService> logger,
            IBusControl messageBus,
            IWriteData<RawTimeSeries> dataStore)
        {
            _logger = logger;
            _messageBus = messageBus;
            _dataStore = dataStore;
            _tokenSource = new CancellationTokenSource();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting message bus");
            return _messageBus.StartAsync(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            _logger.LogInformation("Stopping message bus");
            return _messageBus.StopAsync(cancellationToken);
        }

        public Task Process(RawTimeSeriesSource rawTimeSeries)
        {
            return _dataStore.AddTimeSeriesData(rawTimeSeries.SourceId, rawTimeSeries.RawData.ToArray(), _tokenSource.Token);
        }
    }
}