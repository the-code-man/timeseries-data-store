using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TimeSeries.Shared.Contracts.Entities
{
    public sealed class RawTimeSeries
    {
        [Key]
        public long TimeseriesId { get; set; }

        [Required]
        public double Time { get; set; }

        public List<double> Values { get; set; }
    }
}
