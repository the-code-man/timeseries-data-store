using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Api.Hubs
{
    public class RealtimeDataHub : Hub<IRealtimeDataClient>
    {
        private readonly ILogger<RealtimeDataHub> _logger;
        private readonly ConcurrentDictionary<string, ClientConnection> _subscriptions;

        public RealtimeDataHub(ILogger<RealtimeDataHub> logger)
        {
            _logger = logger;
            _subscriptions = new ConcurrentDictionary<string, ClientConnection>();
        }

        public async Task Subscribe(Subscription[] subscriptions)
        {
            await _subscriptions[Context.ConnectionId].Subscribe(subscriptions);
        }

        public async Task Unsubscribe(Subscription[] subscriptions)
        {
            await _subscriptions[Context.ConnectionId].Unsubscribe(subscriptions);
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogDebug($"Client with id {Context.ConnectionId} connected to the hub");

            var connectionId = Context.ConnectionId;
            var client = Clients.Client(connectionId);

            if (_subscriptions.TryAdd(connectionId, new ClientConnection(client)))
            {
                _logger.LogDebug($"Subscription successfully created for client with id {Context.ConnectionId}");
            }
            else
            {
                _logger.LogError($"Multiple connections from same client. User name -> {Context.User.Identity.Name}, ConnectionId -> {Context.ConnectionId}");
            }

            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_subscriptions.TryRemove(Context.ConnectionId, out ClientConnection clientConnection))
            {
                await clientConnection.UnsubscribeAll();
                _logger.LogDebug($"Subscription successfully removed for client with id {Context.ConnectionId}");
            }
            else
            {
                _logger.LogError($"Unable to remove subscription for client with User name -> {Context.User.Identity.Name}, ConnectionId -> {Context.ConnectionId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}