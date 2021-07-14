using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.Shared.Contracts.DataStore
{
    public interface IReadData<T>
    {
        Task<ReadResponse<List<T>>> GetHistoric(string source, DateTime from, DateTime to, CancellationToken token);
        
        Task<ReadResponse<T>> GetLatest(string source, CancellationToken token);
    }
}