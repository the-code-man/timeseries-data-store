using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.DataStore;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.DataStore.Raw
{
    public class WriteDataStore : IWriteSource<string>, IWriteData<MultiValueTimeSeries>
    {
        private readonly TimeSeriesDbContext _timeSeriesDataContext;
        private readonly ILogger<WriteDataStore> _logger;

        public WriteDataStore(TimeSeriesDbContext timeSeriesDataContext, ILogger<WriteDataStore> logger)
        {
            _timeSeriesDataContext = timeSeriesDataContext;
            _logger = logger;
        }

        public async Task<WriteResponse> AddSources(string[] sources, CancellationToken ct)
        {
            var opResult = false;
            string errorMessage = string.Empty;

            try
            {
                await _timeSeriesDataContext.TimeSeriesSource.AddRangeAsync(sources.Select(s => new MultiValueTimeSeriesSource(s)));
                var affRows = await _timeSeriesDataContext.SaveChangesAsync(ct);

                opResult = affRows > 0;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //TODO :  retry

                errorMessage = "Concurrency error while saving sources to database.";
                _logger.LogError(errorMessage, ex);
            }
            catch (DbUpdateException ex)
            {
                errorMessage = "Error occurred while saving sources to database";
                _logger.LogError(errorMessage, ex);
            }
            catch (Exception ex)
            {
                errorMessage = "Unknown error occurred while adding sources to database";
                _logger.LogError(errorMessage, ex);
            }

            return new WriteResponse
            {
                IsSuccess = opResult,
                ErrorMessage = errorMessage
            };
        }

        public async Task<WriteResponse> AddTimeSeriesData(string source, MultiValueTimeSeries[] rawTimeSeries, CancellationToken ct)
        {
            var opResult = false;
            string errorMessage = string.Empty;

            try
            {
                var timeSeries = await _timeSeriesDataContext.TimeSeriesSource
                    .Where(v => v.SourceId == source)
                    .Include(e => e.RawData)
                    .SingleOrDefaultAsync();

                timeSeries.RawData.AddRange(rawTimeSeries);
                var affRows = await _timeSeriesDataContext.SaveChangesAsync(ct);
                opResult = affRows > 0;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //TODO :  retry

                errorMessage = "Concurrency error while saving sources to database.";
                _logger.LogError(errorMessage, ex);
            }
            catch (DbUpdateException ex)
            {
                errorMessage = "Error occurred while saving sources to database";
                _logger.LogError(errorMessage, ex);
            }
            catch (Exception ex)
            {
                errorMessage = "Unknown error occurred while adding sources to database";
                _logger.LogError(errorMessage, ex);
            }

            return new WriteResponse
            {
                IsSuccess = opResult,
                ErrorMessage = errorMessage
            };
        }
    }
}
