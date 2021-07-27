using Autofac;
using Microsoft.Extensions.Hosting;
using TimeSeries.DataIngestor.Services;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.DataIngestor.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IngestorService>()
                   .As<IHostedService>()
                   .As<IDataProcessor<MultiValueTimeSeriesSource>>()
                   .SingleInstance();
        }
    }
}