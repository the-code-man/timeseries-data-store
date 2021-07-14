using System;

namespace TimeSeries.Shared.Contracts.Api
{
    public class AggrTimeSeriesData
    {
        public DateTime Time { get; set; }

        public double Value { get; set; }
    }
}
