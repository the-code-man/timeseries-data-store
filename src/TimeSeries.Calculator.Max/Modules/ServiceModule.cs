using Autofac;
using Microsoft.Extensions.Hosting;
using TimeSeries.Calculator.Max.Services;
using TimeSeries.gRPC.Server.Modules;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.Calculator.Max.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MaxCalculatorService>()
                .As<IHostedService>()
                .As<IDataProcessor<ProcessedTimeSeries>>()
                .SingleInstance();

            builder.RegisterModule(new gRPCServerModule());
        }
    }
}
