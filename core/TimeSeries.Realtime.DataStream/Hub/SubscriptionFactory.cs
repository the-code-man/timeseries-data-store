using TimeSeries.Realtime.DataStream.Hub.Contracts;
using TimeSeries.Shared.Contracts.DataStream;

namespace TimeSeries.Realtime.DataStream.Hub
{
    public class SubscriptionFactory : ISubscriptionFactory
    {
        private readonly IRealtimeDataHandler _realtimeDataHandler;

        public SubscriptionFactory(IRealtimeDataHandler realtimeDataHandler)
        {
            _realtimeDataHandler = realtimeDataHandler;
        }

        public ISubscription Create(string connectionId, string timeSeriesSource, IRealtimeDataClient hubClient)
        {
            return new Subscription(hubClient, _realtimeDataHandler, connectionId, timeSeriesSource);
        }
    }
}