using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.ServiceBus.Consumer
{
    public class TimeSeriesConsumer : IConsumer<MultiValueTimeSeriesSource>
    {
        private readonly ILogger<TimeSeriesConsumer> _logger;
        private readonly IDataProcessor<MultiValueTimeSeriesSource> _dataProcessor;

        public TimeSeriesConsumer(ILogger<TimeSeriesConsumer> logger, IDataProcessor<MultiValueTimeSeriesSource> dataProcessor)
        {
            _logger = logger;
            _dataProcessor = dataProcessor;
        }

        public async Task Consume(ConsumeContext<MultiValueTimeSeriesSource> context)
        {
            var data = context.Message;

            _logger.LogInformation($"Received raw data to process from '{data.SourceId}' source");
            await _dataProcessor.Process(data);

            await context.Publish<ProcessedTimeSeries>(new
            {
                data.RawData,
                data.SourceId
            });
        }
    }
}