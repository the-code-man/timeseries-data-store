using TimeSeries.Realtime.DataStream.Hub.Contracts;
using TimeSeries.Shared.Contracts.DataStream;

namespace TimeSeries.Realtime.DataStream.Hub
{
    public class ClientConnectionFactory
    {
        private readonly ISubscriptionFactory _subscriptionFactory;

        public ClientConnectionFactory(ISubscriptionFactory subscriptionFactory)
        {
            _subscriptionFactory = subscriptionFactory;
        }

        public ClientConnection Create(string connectionId, IRealtimeDataClient hubClient)
        {
            return new ClientConnection(_subscriptionFactory, hubClient, connectionId);
        }
    }
}