using Microsoft.EntityFrameworkCore;
using TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.DataStore.Aggr
{
    public class TimeSeriesDbContext : DbContext
    {
        public TimeSeriesDbContext(DbContextOptions<TimeSeriesDbContext> options) : base(options) { }

        public DbSet<AggregatedTimeSeries> AggregatedTimeSeries { get; set; }
    }
}
