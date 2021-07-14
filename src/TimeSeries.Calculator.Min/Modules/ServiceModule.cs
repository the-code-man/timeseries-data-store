using Autofac;
using Microsoft.Extensions.Hosting;
using TimeSeries.Calculator.Min.Services;
using TimeSeries.gRPC.Server.Modules;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.Calculator.Min.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MinCalculatorService>()
                .As<IHostedService>()
                .As<IDataProcessor<ProcessedTimeSeries>>()
                .SingleInstance();

            builder.RegisterModule(new gRPCServerModule());
        }
    }
}
