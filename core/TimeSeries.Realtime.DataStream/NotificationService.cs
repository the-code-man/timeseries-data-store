using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.DataStream;

namespace TimeSeries.Realtime.DataStream
{
    public class NotificationService : IConsumer<RealtimeDataEvent>
    {
        private readonly IRealtimeDataHandler _realtimeDataHandler;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IRealtimeDataHandler realtimeDataHandler,
            ILogger<NotificationService> logger)
        {
            _realtimeDataHandler = realtimeDataHandler;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<RealtimeDataEvent> context)
        {
            _realtimeDataHandler.Publish(context.Message);
            return Task.CompletedTask;
        }
    }
}