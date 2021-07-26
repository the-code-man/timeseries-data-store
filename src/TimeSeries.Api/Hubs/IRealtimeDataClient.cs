using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Api;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Api.Hubs
{
    public interface IRealtimeDataClient
    {
        Task OnAggrProcessed(AggrTimeSeriesData[] aggregatedTimeSeries, AggregationType aggregationType);

        Task OnRawProcessed(RawTimeSeriesData[] rawTimeSeries);
    }
}
