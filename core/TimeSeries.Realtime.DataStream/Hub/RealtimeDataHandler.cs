using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using TimeSeries.Shared.Contracts.DataStream;

namespace TimeSeries.Realtime.DataStream.Hub
{
    public class RealtimeDataHandler : IRealtimeDataHandler, IDisposable
    {
        private readonly Subject<RealtimeDataEvent> _subject;
        private readonly Dictionary<string, IDisposable> _subscribers;

        public RealtimeDataHandler()
        {
            _subject = new Subject<RealtimeDataEvent>();
            _subscribers = new Dictionary<string, IDisposable>();
        }

        public void Publish(RealtimeDataEvent @event)
        {
            _subject.OnNext(@event);
        }

        public void Subscribe(string subIdentifier, Action<RealtimeDataEvent> action)
        {
            if (!_subscribers.ContainsKey(subIdentifier))
            {
                _subscribers.Add(subIdentifier, _subject.Subscribe(action));
            }
        }

        public void Unsubscribe(string subIdentifier)
        {
            _subscribers[subIdentifier].Dispose();
            _subscribers.Remove(subIdentifier);
        }

        public void Dispose()
        {
            if (_subject != null)
            {
                _subject.Dispose();
            }

            foreach (var subscriber in _subscribers)
            {
                subscriber.Value.Dispose();
            }
        }
    }
}
