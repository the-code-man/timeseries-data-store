using TimeSeries.Shared.Contracts.DataStream;

namespace TimeSeries.Realtime.DataStream.Hub.Contracts
{
    public interface ISubscriptionFactory
    {
        ISubscription Create(string connectionId, string timeSeriesSource, IRealtimeDataClient hubClient);
    }
}