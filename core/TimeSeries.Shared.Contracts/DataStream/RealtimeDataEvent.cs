using TimeSeries.Shared.Contracts.Api;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Shared.Contracts.DataStream
{
    public class RealtimeDataEvent
    {
        public string Source { get; set; }

        public AggregationType AggregationType { get; set; }

        public MultiValueTimeSeries[] Data { get; set; }
    }
}
