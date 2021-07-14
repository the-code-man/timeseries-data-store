using Autofac;
using AutoMapper;
using TimeSeries.gRPC.Server.Profiles;

namespace TimeSeries.gRPC.Server.Modules
{
    public class gRPCServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(_ => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DefaultMappingsProfile>();
            })).AsSelf().SingleInstance();

            builder.Register(c =>
            {
                //This resolves a new context that can be used later.
                var context = c.Resolve<IComponentContext>();
                var config = context.Resolve<MapperConfiguration>();
                return config.CreateMapper(context.Resolve);
            })
                .As<IMapper>()
                .InstancePerLifetimeScope();

        }
    }
}
