using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.DataStore;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.DataStore.Raw
{
    public class ReadDataStore : IReadSource<string>, IReadData<RawTimeSeries>
    {
        private readonly TimeSeriesDbContext _timeSeriesDataContext;

        public ReadDataStore(TimeSeriesDbContext timeSeriesDataContext)
        {
            _timeSeriesDataContext = timeSeriesDataContext;
        }

        public Task<ReadResponse<List<string>>> GetAllSources(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return Task.FromResult(new ReadResponse<List<string>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Cancellation requested"
                });
            }

            return Task.FromResult(new ReadResponse<List<string>>
            {
                IsSuccess = true,
                Data = _timeSeriesDataContext.TimeSeriesSource.Select(v => v.SourceId).ToList()
            });
        }

        public Task<ReadResponse<List<RawTimeSeries>>> GetHistoric(string sourceId, DateTime from, DateTime to, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return Task.FromResult(new ReadResponse<List<RawTimeSeries>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Cancellation requested"
                });
            }

            var fromDate = from.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            var toDate = to.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            try
            {
                return Task.FromResult(new ReadResponse<List<RawTimeSeries>>
                {
                    Data = _timeSeriesDataContext.TimeSeriesSource
                                             .Where(v => v.SourceId == sourceId)
                                             .Include(e => e.RawData)
                                             .Single()
                                             .RawData
                                             .Where(vd => vd.Time >= fromDate && vd.Time <= toDate)
                                             .ToList(),
                    IsSuccess = true
                });
            }
            catch (InvalidOperationException)
            {
                return Task.FromResult(new ReadResponse<List<RawTimeSeries>>
                {
                    IsSuccess = false,
                    ErrorMessage = $"No data available for '{sourceId}' source"
                });
            }
        }

        public Task<ReadResponse<RawTimeSeries>> GetLatest(string sourceId, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return Task.FromResult(new ReadResponse<RawTimeSeries>
                {
                    IsSuccess = false,
                    ErrorMessage = "Cancellation requested"
                });
            }

            try
            {
                return Task.FromResult(new ReadResponse<RawTimeSeries>
                {
                    Data = _timeSeriesDataContext.TimeSeriesSource
                                             .Where(v => v.SourceId == sourceId)
                                             .Include(v => v.RawData)
                                             .Single()
                                             .RawData
                                             .OrderByDescending(v => v.Time).First(),

                    IsSuccess = true
                });
            }
            catch (InvalidOperationException)
            {
                return Task.FromResult(new ReadResponse<RawTimeSeries>
                {
                    IsSuccess = false,
                    ErrorMessage = $"No data available for '{sourceId}' source"
                });
            }
        }
    }
}