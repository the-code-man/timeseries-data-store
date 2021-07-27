using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TimeSeries.Shared.Contracts.Services
{
    public interface ICalculatorService
    {
        Task<Api.ReadResponse<List<Api.SingleValueTimeSeries>>> GetHistoric(AggregationType aggregateType, string source, DateTime from, DateTime to, CancellationToken cancellationToken);

        Task<Api.ReadResponse<Api.SingleValueTimeSeries>> GetLatest(AggregationType aggregateType, string source, CancellationToken cancellationToken);
    }
}
