using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Api.Hubs
{
    public class Subscription
    {
        public string Source { get; set; }

        public AggregationType AggregationType { get; set; }
    }
}