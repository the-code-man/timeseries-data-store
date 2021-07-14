using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.ServiceBus.Common
{
    public class ProcessedTimeSeriesConsumer : IConsumer<ProcessedTimeSeries>
    {
        private readonly ILogger<ProcessedTimeSeriesConsumer> _logger;
        private readonly IDataProcessor<ProcessedTimeSeries> _dataProcessor;

        public ProcessedTimeSeriesConsumer(ILogger<ProcessedTimeSeriesConsumer> logger, IDataProcessor<ProcessedTimeSeries> dataProcessor)
        {
            _logger = logger;
            _dataProcessor = dataProcessor;
        }

        public Task Consume(ConsumeContext<ProcessedTimeSeries> context)
        {
            var data = context.Message;

            _logger.LogInformation($"Received processed data from '{data.SourceId}' source");

            return _dataProcessor.Process(data);
        }
    }
}