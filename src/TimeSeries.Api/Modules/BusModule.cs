using Autofac;
using MassTransit;
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
                v.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(_busSettings.Host, h =>
                    {
                        h.Username(_busSettings.UserName);
                        h.Password(_busSettings.Password);
                    });
                }));
            });
        }
    }
}