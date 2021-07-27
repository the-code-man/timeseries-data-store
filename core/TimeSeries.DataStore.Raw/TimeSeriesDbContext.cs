using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.DataStore.Raw
{
    public class TimeSeriesDbContext : DbContext
    {
        public TimeSeriesDbContext(DbContextOptions<TimeSeriesDbContext> options) : base(options) { }

        public DbSet<MultiValueTimeSeriesSource> TimeSeriesSource { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MultiValueTimeSeries>().Property(e => e.Values).HasConversion(
            v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
            v => JsonConvert.DeserializeObject<List<double>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}