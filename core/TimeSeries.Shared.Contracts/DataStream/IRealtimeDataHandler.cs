using System;

namespace TimeSeries.Shared.Contracts.DataStream
{
    public interface IRealtimeDataHandler
    {
        void Publish(RealtimeDataEvent @event);

        void Subscribe(string subIdentifier, Action<RealtimeDataEvent> action);

        void Unsubscribe(string subIdentifier);
    }
}