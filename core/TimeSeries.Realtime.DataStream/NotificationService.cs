using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Api;
using TimeSeries.Shared.Contracts.DataStream;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Realtime.DataStream
{
    public class NotificationService : BackgroundService
    {
        private readonly IRealtimeDataHandler _realtimeDataHandler;
        private IObservable<long> _tempBus;

        public NotificationService(IRealtimeDataHandler realtimeDataHandler)
        {
            _realtimeDataHandler = realtimeDataHandler;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _tempBus = Observable.Interval(TimeSpan.FromSeconds(5));

            _tempBus.Subscribe(n =>
            {
                var time = DateTime.UtcNow;

                _realtimeDataHandler.Publish(new RealtimeDataEvent
                {
                    Source = "FQREW",
                    AggregationType = AggregationType.Raw,
                    Data = new MultiValueTimeSeries[]
                    {
                        new MultiValueTimeSeries
                        {
                            Time = time,
                            Values = new List<double> { n, n + 1 }
                        }
                    }
                });

                _realtimeDataHandler.Publish(new RealtimeDataEvent
                {
                    Source = "FQREW",
                    AggregationType = AggregationType.Avg,
                    Data = new MultiValueTimeSeries[]
                    {
                        new MultiValueTimeSeries
                        {
                            Time = time,
                            Values = new List<double> { n, n + 1 }
                        }
                    }
                });
            }, stoppingToken);

            return Task.CompletedTask;
        }
    }
}
