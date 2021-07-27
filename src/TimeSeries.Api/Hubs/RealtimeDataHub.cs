using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TimeSeries.Realtime.DataStream.Hub;
using TimeSeries.Shared.Contracts.DataStream;

namespace TimeSeries.Api.Hubs
{
    public class RealtimeDataHub : Hub<IRealtimeDataClient>
    {
        private readonly ILogger<RealtimeDataHub> _logger;
        private readonly ClientConnectionFactory _connectionFactory;
        private static readonly ConcurrentDictionary<string, ClientConnection> _subscriptions = new();

        public RealtimeDataHub(ILogger<RealtimeDataHub> logger, ClientConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
        }

        public void Subscribe(string source, string aggregationType)
        {
            _subscriptions[Context.ConnectionId].Subscribe(source, aggregationType);
            _logger.LogInformation($"Subscription successfully created for Connection --> {Context.ConnectionId}, Subscription --> {source} ({aggregationType})");
        }

        public void Unsubscribe(string source, string aggregationType)
        {
            _subscriptions[Context.ConnectionId].Unsubscribe(source, aggregationType);
            _logger.LogInformation($"Subscription successfully removed for Connection --> {Context.ConnectionId}, Subscription --> {source} ({aggregationType})");
        }

        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var client = Clients.Client(connectionId);

            _logger.LogDebug($"Client with id {Context.ConnectionId} connected to the hub");

            if (_subscriptions.TryAdd(connectionId, _connectionFactory.Create(connectionId, client)))
            {
                _logger.LogDebug($"Subscription successfully created for client with id {Context.ConnectionId}");
            }
            else
            {
                _logger.LogError($"Multiple connections from same client. User name -> {Context.User.Identity.Name}, ConnectionId -> {Context.ConnectionId}");
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (_subscriptions.TryRemove(Context.ConnectionId, out ClientConnection clientConnection))
            {
                clientConnection.UnsubscribeAll();
                _logger.LogDebug($"Subscription successfully removed for client with id {Context.ConnectionId}");
            }
            else
            {
                _logger.LogError($"Unable to remove subscription for client with User name -> {Context.User.Identity.Name}, ConnectionId -> {Context.ConnectionId}");
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}