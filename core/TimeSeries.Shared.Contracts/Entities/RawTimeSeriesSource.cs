using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeSeries.Shared.Contracts.Entities
{
    public class RawTimeSeriesSource
    {
        public RawTimeSeriesSource() { }

        public RawTimeSeriesSource(string sourceId)
        {
            SourceId = sourceId;
            RawData = new List<RawTimeSeries>(0);
        }

        public RawTimeSeriesSource(string sourceId, List<RawTimeSeries> rawData) : this(sourceId)
        {
            RawData = rawData;
        }

        [Key]
        [Required]
        public string SourceId { get; set; }

        [ForeignKey("SourceId")]
        public List<RawTimeSeries> RawData { get; set; }
    }
}