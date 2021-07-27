using System;

namespace TimeSeries.Shared.Contracts.Api
{
    public class SingleValueTimeSeries
    {
        public DateTime Time { get; set; }

        public double Value { get; set; }
    }
}
