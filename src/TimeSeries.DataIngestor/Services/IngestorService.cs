using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.DataStore;
using TimeSeries.Shared.Contracts.DataStream;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Services;
using ApiContracts = TimeSeries.Shared.Contracts.Api;

namespace TimeSeries.DataIngestor.Services
{
    public class IngestorService : BackgroundService, IDataProcessor<MultiValueTimeSeriesSource>
    {
        private readonly ILogger<IngestorService> _logger;
        private readonly IBusControl _messageBus;
        private readonly IWriteData<MultiValueTimeSeries> _dataStore;
        private readonly IMapper _mapper;
        private readonly CancellationTokenSource _tokenSource;

        public IngestorService(ILogger<IngestorService> logger,
            IBusControl messageBus,
            IWriteData<MultiValueTimeSeries> dataStore,
            IMapper mapper)
        {
            _logger = logger;
            _messageBus = messageBus;
            _dataStore = dataStore;
            _mapper = mapper;
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

        public async Task Process(MultiValueTimeSeriesSource rawTimeSeries)
        {
            var response = await _dataStore.AddTimeSeriesData(rawTimeSeries.SourceId,
                rawTimeSeries.RawData.ToArray(),
                _tokenSource.Token);

            if (response.IsSuccess)
            {
                await _messageBus.Publish(new RealtimeDataEvent
                {
                    AggregationType = AggregationType.Raw,
                    Data = _mapper.Map<ApiContracts.MultiValueTimeSeries[]>(rawTimeSeries.RawData),
                    Source = rawTimeSeries.SourceId
                }, _tokenSource.Token);
            }
        }
    }
}