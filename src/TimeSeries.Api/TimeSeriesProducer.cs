using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.Api
{
    public class TimeSeriesProducer : IProducer
    {
        private readonly ILogger<TimeSeriesProducer> _logger;
        private readonly IBus _bus;

        public TimeSeriesProducer(ILogger<TimeSeriesProducer> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task Send(string queue, string sourceId, RawTimeSeries[] data, CancellationToken ct)
        {
            // TODO :  Publisher Confirms and Consumer Acknowledgements, Production Checklist and Monitoring

            try
            {
                var message = new RawTimeSeriesSource(sourceId, new List<RawTimeSeries>(data));

                Uri uri = new($"queue:{queue}");
                var endpoint = await _bus.GetSendEndpoint(uri);
                await endpoint.Send(message, ct);

                _logger.LogDebug("Message published!");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while sending message to bus", ex);
            }
        }
    }
}
/* implementation directly RabbitMQ libary
 * 
 private readonly ConnectionFactory _factory;
 private readonly IModel _channel;
 const string queueName = "vec:raw";

_logger.LogInformation("Connecting to message broker...");

_factory = new ConnectionFactory()
{
    HostName = _settings.MessageBroker.Host,
    Port = _settings.MessageBroker.Port,
    UserName = _settings.MessageBroker.UserName,
    Password = _settings.MessageBroker.Password
};

var conn = _factory.CreateConnection();
_channel = conn.CreateModel();

_channel.QueueDeclare(queue: queueName,
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

_logger.LogInformation("Connected to message broker...");

_channel.BasicPublish(exchange: "",
    routingKey: queueName,
    basicProperties: null,
    body: JsonSerializer.SerializeToUtf8Bytes(new
    {
        Source = sourceId,
        Data = data
    }));
*/