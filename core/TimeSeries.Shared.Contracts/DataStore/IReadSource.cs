using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.Shared.Contracts.DataStore
{
    public interface IReadSource<T>
    {
        Task<ReadResponse<List<T>>> GetAllSources(CancellationToken token);
    }
}
