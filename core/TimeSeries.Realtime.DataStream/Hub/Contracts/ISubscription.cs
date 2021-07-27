using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Realtime.DataStream.Hub.Contracts
{
    public interface ISubscription
    {
        void Add(AggregationType aggregationType);

        int Remove(AggregationType aggregationType);

        void RemoveAll();
    }
}