using Autofac;
using Microsoft.EntityFrameworkCore;
using TimeSeries.Shared.Contracts.DataStore;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Settings;

namespace TimeSeries.DataStore.Raw
{
    public class DataStoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(bContext =>
            {
                var dataStoreSettings = bContext.Resolve<DataStoreSettings>();
                
                var context = new TimeSeriesDbContext(
                    new DbContextOptionsBuilder<TimeSeriesDbContext>()
                    .UseSqlite($"Data Source={dataStoreSettings.ConnectionString}").Options);

                context.Database.EnsureCreated();

                return context;
            });

            builder.RegisterType<ReadDataStore>()
                .As<IReadSource<string>>()
                .As<IReadData<RawTimeSeries>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<WriteDataStore>()
                .As<IWriteSource<string>>()
                .As<IWriteData<RawTimeSeries>>()
                .InstancePerLifetimeScope();
        }
    }
}
