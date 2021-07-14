using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.Shared.Contracts.DataStore
{
    public interface IWriteData<T>
    {
        Task<WriteResponse> AddTimeSeriesData(string source, T[] timeSeries, CancellationToken ct);
    }
}
