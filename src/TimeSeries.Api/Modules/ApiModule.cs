using Autofac;
using AutoMapper;
using TimeSeries.Api.Profiles;
using TimeSeries.Api.Services;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Api.Modules
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(_ => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DefaultMappingsProfile>();
            }
            )).AsSelf().SingleInstance();

            builder.Register(c =>
            {
                //This resolves a new context that can be used later.
                var context = c.Resolve<IComponentContext>();
                var config = context.Resolve<MapperConfiguration>();
                return config.CreateMapper(context.Resolve);
            })
                .As<IMapper>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CalculatorService>()
                .As<ICalculatorService>()
                .InstancePerLifetimeScope();
        }
    }
}
