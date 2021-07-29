using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.ServiceBus.Common
{
    public class RawTimeSeriesConsumer : IConsumer<ProcessedTimeSeries>
    {
        private readonly ILogger<RawTimeSeriesConsumer> _logger;
        private readonly IDataProcessor<ProcessedTimeSeries> _dataProcessor;

        public RawTimeSeriesConsumer(ILogger<RawTimeSeriesConsumer> logger, IDataProcessor<ProcessedTimeSeries> dataProcessor)
        {
            _logger = logger;
            _dataProcessor = dataProcessor;
        }

        public Task Consume(ConsumeContext<ProcessedTimeSeries> context)
        {
            var data = context.Message;

            _logger.LogInformation($"Received raw timeseries data from '{data.SourceId}' source");

            return _dataProcessor.Process(data);
        }
    }
}