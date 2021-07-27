using System;
using System.Collections.Concurrent;
using TimeSeries.Realtime.DataStream.Hub.Contracts;
using TimeSeries.Shared.Contracts.DataStream;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Realtime.DataStream.Hub
{
    public class ClientConnection
    {
        private readonly ConcurrentDictionary<string, ISubscription> _subscriptions;
        private readonly ISubscriptionFactory _subscriptionFactory;
        private readonly string _connectionId; 
        private readonly IRealtimeDataClient _hubClient;

        public ClientConnection(ISubscriptionFactory subscriptionFactory, IRealtimeDataClient hubClient, string connectionId)
        {
            _subscriptionFactory = subscriptionFactory;
            _subscriptions = new ConcurrentDictionary<string, ISubscription>();
            _hubClient = hubClient;
            _connectionId = connectionId;
        }

        public void Subscribe(string source, string aggregationType)
        {
            if (!_subscriptions.TryGetValue(source, out ISubscription subscription))
            {
                subscription = _subscriptionFactory.Create(_connectionId, source, _hubClient);
                _subscriptions.TryAdd(source, subscription);
            }

            if (Enum.TryParse(aggregationType, out AggregationType aggrType))
            {
                subscription.Add(aggrType);
            }
        }

        public void Unsubscribe(string source, string aggregationType)
        {
            if (Enum.TryParse(aggregationType, out AggregationType aggrType))
            {
                var remainingSubs = _subscriptions[source].Remove(aggrType);

                if (remainingSubs <= 0)
                {
                    _subscriptions.TryRemove(source, out _);
                }
            }
        }

        public void UnsubscribeAll()
        {
            foreach (var key in _subscriptions.Keys)
            {
                _subscriptions[key].RemoveAll();
            }

            _subscriptions.Clear();
        }
    }
}