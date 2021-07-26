using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Api;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Api.Hubs
{
    public class RealtimeDataSubscription
    {
        private readonly IRealtimeDataClient _client;

        public RealtimeDataSubscription(IRealtimeDataClient client)
        {
            AggregationTypes = new List<AggregationType>(0);
            _client = client;
        }

        public string TimeSeriesSource { get; init; }

        public List<AggregationType> AggregationTypes { get; }

        public Task Add(AggregationType aggregationType)
        {
            if (!AggregationTypes.Contains(aggregationType))
            {
                AggregationTypes.Add(aggregationType);
            }
            else
            {
                // Log subscription already exists
            }

            _client.OnRawProcessed(new RawTimeSeriesData[] { new RawTimeSeriesData { Time = DateTime.UtcNow, Values = new List<double> { 1, 2, 3 } } });

            _client.OnAggrProcessed(new AggrTimeSeriesData[] { new AggrTimeSeriesData { Time = DateTime.UtcNow, Value = 2 } }, AggregationType.Min);

            return Task.CompletedTask;
        }

        public Task<int> Remove(AggregationType aggregationType)
        {
            AggregationTypes.Remove(aggregationType);

            return Task.FromResult(AggregationTypes.Count);
        }

        public Task RemoveAll()
        {
            foreach (var aggrType in AggregationTypes)
            {
                //TODO
            }

            AggregationTypes.Clear();

            return Task.CompletedTask;
        }
    }
}