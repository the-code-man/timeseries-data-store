using System.ComponentModel.DataAnnotations;

namespace TimeSeries.Shared.Contracts.Entities
{
    public class SingleValueTimeSeries
    {
        [Key]
        public int DataId { get; set; }

        public string Source { get; set; }

        public double Time { get; set; }

        public double Value { get; set; }
    }
}
