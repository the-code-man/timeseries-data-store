using Autofac;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using TimeSeries.DataIngestor.Profiles;
using TimeSeries.DataIngestor.Services;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.DataIngestor.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(_ => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DefaultMappingsProfile>();
            }))
                .AsSelf()
                .SingleInstance();

            builder.Register(c =>
            {
                //This resolves a new context that can be used later.
                var context = c.Resolve<IComponentContext>();
                var config = context.Resolve<MapperConfiguration>();
                return config.CreateMapper(context.Resolve);
            })
                .As<IMapper>()
                .InstancePerLifetimeScope();

            builder.RegisterType<IngestorService>()
                   .As<IHostedService>()
                   .As<IDataProcessor<MultiValueTimeSeriesSource>>()
                   .SingleInstance();
        }
    }
}