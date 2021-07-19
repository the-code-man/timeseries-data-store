using System.Collections.Generic;
using System.Threading.Tasks;
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