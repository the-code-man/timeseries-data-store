using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Api;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Shared.Contracts.DataStream
{
    public interface IRealtimeDataClient
    {
        Task OnRealtimeDataReceived(string source,
            AggregationType aggregationType,
            MultiValueTimeSeries[] aggregatedTimeSeries);
    }
}