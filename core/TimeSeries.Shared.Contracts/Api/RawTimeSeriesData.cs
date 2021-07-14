using System;
using System.Collections.Generic;

namespace TimeSeries.Shared.Contracts.Api
{
    public class RawTimeSeriesData
    {
        public DateTime Time { get; set; }

        public List<double> Values { get; set; }
    }
}
