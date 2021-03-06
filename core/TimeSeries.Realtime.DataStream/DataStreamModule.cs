using Autofac;
using TimeSeries.Realtime.DataStream.Hub;
using TimeSeries.Realtime.DataStream.Hub.Contracts;
using TimeSeries.Shared.Contracts.DataStream;

namespace TimeSeries.Realtime.DataStream
{
    public class DataStreamModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ClientConnectionFactory>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<RealtimeDataHandler>()
                .As<IRealtimeDataHandler>()
                .SingleInstance();

            builder.RegisterType<SubscriptionFactory>()
                .As<ISubscriptionFactory>()
                .SingleInstance();

            builder.RegisterType<Subscription>()
                .As<ISubscription>()
                .InstancePerLifetimeScope();

            builder.RegisterType<NotificationService>()
                .AsSelf()
                .SingleInstance();
        }
    }
}