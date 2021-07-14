using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.Shared.Contracts.DataStore
{
    public interface IWriteSource<T>
    {
        Task<WriteResponse> AddSources(T[] sources, CancellationToken ct);
    }
}
