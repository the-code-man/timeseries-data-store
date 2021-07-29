using Autofac;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using TimeSeries.Calculator.Min.Profiles;
using TimeSeries.Calculator.Min.Services;
using TimeSeries.gRPC.Server.Profiles;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.Calculator.Min.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(_ => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<gRPCServerMappingProfile>();
                cfg.AddProfile<DefaultMappingsProfile>();

            })).AsSelf().SingleInstance();

            builder.Register(c =>
            {
                //This resolves a new context that can be used later.
                var context = c.Resolve<IComponentContext>();
                var config = context.Resolve<MapperConfiguration>();
                return config.CreateMapper(context.Resolve);
            }).As<IMapper>().InstancePerLifetimeScope();

            builder.RegisterType<MinCalculatorService>()
                .As<IHostedService>()
                .As<IDataProcessor<ProcessedTimeSeries>>()
                .SingleInstance();
        }
    }
}
