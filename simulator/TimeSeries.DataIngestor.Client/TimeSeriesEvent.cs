using System;

namespace DataIngestor.Client
{
    public class TimeSeriesEvent
    {
        public TimeSeriesEvent(DateTime time, double[] values)
        {
            Time = time;
            Values = values;
        }

        public DateTime Time { get; set; }

        public double[] Values { get; set; }
    }
}
