﻿using Autofac;
using GreenPipes;
using MassTransit;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.Settings;

namespace TimeSeries.Calculator.Min.Modules
{
    public class ConsumerBusModule : Module
    {
        private readonly MessageBusSettings _busSettings;

        public ConsumerBusModule(MessageBusSettings busSettings)
        {
            _busSettings = busSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.AddMassTransit(v =>
            {
                v.AddConsumer<ProcessedTimeSeriesConsumer>();            //  Add one or more consumers to the transit

                v.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(_busSettings.Host, h =>
                    {
                        h.Username(_busSettings.UserName);
                        h.Password(_busSettings.Password);
                    });

                    cfg.ReceiveEndpoint(MessageBusQueue.PROCESSED_DATA_MIN, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(2, 100));
                        ep.ConfigureConsumer<ProcessedTimeSeriesConsumer>(provider);     // Link endpoint to consumer
                    });
                }));
            });
        }
    }
}