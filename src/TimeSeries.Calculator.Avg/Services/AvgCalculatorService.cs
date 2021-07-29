using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.DataStore;
using TimeSeries.Shared.Contracts.DataStream;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Internal;
using TimeSeries.Shared.Contracts.Services;
using ApiContracts = TimeSeries.Shared.Contracts.Api;


namespace TimeSeries.Calculator.Avg.Services
{
    public class AvgCalculatorService : BackgroundService, IDataProcessor<ProcessedTimeSeries>
    {
        private readonly ILogger<AvgCalculatorService> _logger;
        private readonly IBusControl _messageBus;
        private readonly IWriteData<SingleValueTimeSeries> _dataStore;
        private readonly IMapper _mapper;
        private readonly CancellationTokenSource _tokenSource;

        public AvgCalculatorService(ILogger<AvgCalculatorService> logger,
            IBusControl messageBus,
            IMapper mapper,
            IWriteData<SingleValueTimeSeries> dataStore)
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

        public async Task Process(ProcessedTimeSeries processedTimeSeries)
        {
            _logger.LogInformation($"Received processed timeseries data. Source: {processedTimeSeries.SourceId}");

            var avgData = processedTimeSeries.RawData
                .Select(d => new SingleValueTimeSeries
                {
                    Source = processedTimeSeries.SourceId,
                    Time = d.Time,
                    Value = Math.Round(d.Values.Average(), 2)
                }).ToArray();

            var response = await _dataStore.AddTimeSeriesData(processedTimeSeries.SourceId, avgData, _tokenSource.Token);

            if (response.IsSuccess)
            {
                await _messageBus.Publish(new RealtimeDataEvent
                {
                    AggregationType = AggregationType.Avg,
                    Data = _mapper.Map<ApiContracts.MultiValueTimeSeries[]>(avgData),
                    Source = processedTimeSeries.SourceId
                }, _tokenSource.Token);
            }
        }
    }
}