using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.DataStore;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.Calculator.Max.Services
{
    public class MaxCalculatorService : BackgroundService, IDataProcessor<ProcessedTimeSeries>
    {
        private readonly ILogger<MaxCalculatorService> _logger;
        private readonly IBusControl _messageBus;
        private readonly IWriteData<AggregatedTimeSeries> _dataStore;
        private readonly CancellationTokenSource _tokenSource;

        public MaxCalculatorService(ILogger<MaxCalculatorService> logger,
            IBusControl messageBus,
            IWriteData<AggregatedTimeSeries> dataStore)
        {
            _logger = logger;
            _messageBus = messageBus;
            _dataStore = dataStore;
            _tokenSource = new CancellationTokenSource();
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting message bus");
            return _messageBus.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            _logger.LogInformation("Stopping message bus");
            return _messageBus.StopAsync(cancellationToken);
        }

        public Task Process(ProcessedTimeSeries processedTimeSeries)
        {
            _logger.LogInformation($"Received processed timeseries data. Source: {processedTimeSeries.SourceId}");

            var maxData = processedTimeSeries.RawData
                .Select(d => new AggregatedTimeSeries
                {
                    Time = d.Time,
                    Value = d.Values.Max()
                }).ToArray();

            return _dataStore.AddTimeSeriesData(processedTimeSeries.SourceId, maxData, _tokenSource.Token);
        }
    }
}