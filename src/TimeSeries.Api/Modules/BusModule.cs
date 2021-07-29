using Autofac;
using GreenPipes;
using MassTransit;
using TimeSeries.Realtime.DataStream;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.Settings;

namespace TimeSeries.Api.Modules
{
    public class BusModule : Module
    {
        private readonly MessageBusSettings _busSettings;

        public BusModule(MessageBusSettings busSettings)
        {
            _busSettings = busSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TimeSeriesProducer>().As<IProducer>().SingleInstance();

            builder.AddMassTransit(v =>
            {
                v.AddConsumer<NotificationService>();

                v.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(_busSettings.Host, h =>
                    {
                        h.Username(_busSettings.UserName);
                        h.Password(_busSettings.Password);
                    });

                    cfg.ReceiveEndpoint(MessageBusQueue.PROCESSED_DATA, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(2, 100));
                        ep.ConfigureConsumer<NotificationService>(provider);     // Link endpoint to consumer
                    });

                }));
            });
        }
    }
}