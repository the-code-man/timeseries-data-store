using System.Collections.Generic;
using TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.Shared.Contracts.Internal
{
    public sealed class ProcessedTimeSeries 
    {
        public string SourceId { get; set; }

        public List<RawTimeSeries> RawData { get; set; }
    }
}