using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Api.Hubs
{
    public class ClientConnection
    {
        // TimeSeries Source Subscription
        private readonly ConcurrentDictionary<string, RealtimeDataSubscription> _subscriptions;
        private readonly IRealtimeDataClient _client;

        public ClientConnection(IRealtimeDataClient client)
        {
            _subscriptions = new ConcurrentDictionary<string, RealtimeDataSubscription>();
            _client = client;
        }

        public async Task Subscribe(Subscription[] subscriptions)
        {
            foreach (var subscription in subscriptions)
            {
                if (!_subscriptions.TryGetValue(subscription.Source, out RealtimeDataSubscription dataSubscription))
                {
                    dataSubscription = new RealtimeDataSubscription(_client);
                    _subscriptions.TryAdd(subscription.Source, dataSubscription);
                }

                if (Enum.TryParse(subscription.AggregationType, out AggregationType aggrType)) 
                {
                    await dataSubscription.Add(aggrType);
                }
            }
        }

        public async Task Unsubscribe(Subscription[] subscriptions)
        {
            foreach (var subscription in subscriptions)
            {
                if (Enum.TryParse(subscription.AggregationType, out AggregationType aggrType))
                {
                    var remainingSubs = await _subscriptions[subscription.Source].Remove(aggrType);

                    if (remainingSubs <= 0)
                    {
                        _subscriptions.TryRemove(subscription.Source, out _);
                    }
                }
            }
        }

        public async Task UnsubscribeAll()
        {
            foreach (var key in _subscriptions.Keys)
            {
                await _subscriptions[key].RemoveAll();
            }

            _subscriptions.Clear();
        }
    }
}