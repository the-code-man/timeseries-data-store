using System.Collections.Concurrent;
using System.Threading.Tasks;

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

                await dataSubscription.Add(subscription.AggregationType);
            }
        }

        public async Task Unsubscribe(Subscription[] subscriptions)
        {
            foreach (var subscription in subscriptions)
            {
                var remainingSubs = await _subscriptions[subscription.Source].Remove(subscription.AggregationType);

                if (remainingSubs <= 0)
                {
                    _subscriptions.TryRemove(subscription.Source, out _);
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