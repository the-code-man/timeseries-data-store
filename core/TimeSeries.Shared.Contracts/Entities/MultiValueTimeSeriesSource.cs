using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeSeries.Shared.Contracts.Entities
{
    public class MultiValueTimeSeriesSource
    {
        public MultiValueTimeSeriesSource() { }

        public MultiValueTimeSeriesSource(string sourceId)
        {
            SourceId = sourceId;
            RawData = new List<MultiValueTimeSeries>(0);
        }

        public MultiValueTimeSeriesSource(string sourceId, List<MultiValueTimeSeries> rawData) : this(sourceId)
        {
            RawData = rawData;
        }

        [Key]
        [Required]
        public string SourceId { get; set; }

        [ForeignKey("SourceId")]
        public List<MultiValueTimeSeries> RawData { get; set; }
    }
}