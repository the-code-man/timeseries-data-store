using System.Collections.Generic;
using TimeSeries.Realtime.DataStream.Hub.Contracts;
using TimeSeries.Shared.Contracts.DataStream;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Realtime.DataStream.Hub
{
    public class Subscription : ISubscription
    {
        private readonly IRealtimeDataClient _client;
        private readonly IRealtimeDataHandler _realtimeDataHandler;
        private readonly List<AggregationType> _aggregationTypes;
        private readonly string _connectionId;
        private readonly string _timeSeriesSource;

        public Subscription(IRealtimeDataClient client,
            IRealtimeDataHandler realtimeDataHandler,
            string connectionId,
            string timeSeriesSource)
        {
            _client = client;
            _realtimeDataHandler = realtimeDataHandler;
            _connectionId = connectionId;
            _timeSeriesSource = timeSeriesSource;
            _aggregationTypes = new List<AggregationType>(0);
        }

        private string UniqueSubscriptionIdPrefix
        {
            get
            {
                return $"{_connectionId}_{_timeSeriesSource}";
            }
        }

        public void Add(AggregationType aggregationType)
        {
            if (!_aggregationTypes.Contains(aggregationType))
            {
                _aggregationTypes.Add(aggregationType);
                _realtimeDataHandler.Subscribe($"{UniqueSubscriptionIdPrefix}_{aggregationType}", OnRealtimeDataReceived);
            }
            else
            {
                // Log subscription already exists
            }
        }

        public int Remove(AggregationType aggregationType)
        {
            _aggregationTypes.Remove(aggregationType);
            _realtimeDataHandler.Unsubscribe($"{UniqueSubscriptionIdPrefix}_{aggregationType}");

            return _aggregationTypes.Count;
        }

        public void RemoveAll()
        {
            foreach (var aggrType in _aggregationTypes)
            {
                _realtimeDataHandler.Unsubscribe($"{UniqueSubscriptionIdPrefix}_{aggrType}");
            }

            _aggregationTypes.Clear();
        }

        private void OnRealtimeDataReceived(RealtimeDataEvent realtimeData)
        {
            if (realtimeData.Source == _timeSeriesSource &&
                _aggregationTypes.Contains(realtimeData.AggregationType))
            {
                _client.OnRealtimeDataReceived(realtimeData.Source,
                    realtimeData.AggregationType,
                    realtimeData.Data);
            }
        }
    }
}