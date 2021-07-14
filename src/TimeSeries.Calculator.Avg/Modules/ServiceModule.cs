using Autofac;
using Microsoft.Extensions.Hosting;
using TimeSeries.Calculator.Avg.Services;
using TimeSeries.gRPC.Server.Modules;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.Calculator.Avg.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AvgCalculatorService>()
                .As<IHostedService>()
                .As<IDataProcessor<ProcessedTimeSeries>>()
                .SingleInstance();

            builder.RegisterModule(new gRPCServerModule());
        }
    }
}
