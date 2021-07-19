using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Api.Hubs
{
    public interface IRealtimeDataClient
    {
        Task OnProcessed(RawTimeSeries[] rawTimeSeries);

        Task OnProcessed(AggregatedTimeSeries[] aggregatedTimeSeries, AggregationType aggregationType);
    }
}
