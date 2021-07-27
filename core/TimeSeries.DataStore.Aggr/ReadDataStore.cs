using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.DataStore;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.DataStore.Aggr
{
    public class ReadDataStore : IReadData<SingleValueTimeSeries>
    {
        private readonly TimeSeriesDbContext _timeSeriesDataContext;

        public ReadDataStore(TimeSeriesDbContext timeSeriesDataContext)
        {
            _timeSeriesDataContext = timeSeriesDataContext;
        }

        public Task<ReadResponse<List<SingleValueTimeSeries>>> GetHistoric(string sourceId,
            DateTime from,
            DateTime to,
            CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return Task.FromResult(new ReadResponse<List<SingleValueTimeSeries>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Cancellation requested"
                });
            }

            var fromDate = from.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var toDate = to.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            return Task.FromResult(new ReadResponse<List<SingleValueTimeSeries>>
            {
                Data = _timeSeriesDataContext.AggregatedTimeSeries.Where(v => v.Source == sourceId &&
                v.Time >= fromDate &&
                v.Time <= toDate).ToList(),

                IsSuccess = true
            });
        }

        public Task<ReadResponse<SingleValueTimeSeries>> GetLatest(string sourceId, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return Task.FromResult(new ReadResponse<SingleValueTimeSeries>
                {
                    IsSuccess = false,
                    ErrorMessage = "Cancellation requested"
                });
            }

            return Task.FromResult(new ReadResponse<SingleValueTimeSeries>
            {
                Data = _timeSeriesDataContext.AggregatedTimeSeries.Where(v => v.Source == sourceId)
                .OrderByDescending(v => v.Time).First(),

                IsSuccess = true
            });
        }
    }
}