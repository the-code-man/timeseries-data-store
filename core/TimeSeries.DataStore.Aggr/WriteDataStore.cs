using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.DataStore;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Internal;

namespace TimeSeries.DataStore.Aggr
{
    public class WriteDataStore : IWriteData<AggregatedTimeSeries>
    {
        private readonly TimeSeriesDbContext _timeSeriesDataContext;
        private readonly ILogger<WriteDataStore> _logger;

        public WriteDataStore(TimeSeriesDbContext timeSeriesDataContext, ILogger<WriteDataStore> logger)
        {
            _timeSeriesDataContext = timeSeriesDataContext;
            _logger = logger;
        }

        public async Task<WriteResponse> AddTimeSeriesData(string source, AggregatedTimeSeries[] aggregatedTimeSeries, CancellationToken cancellationToken)
        {
            var opResult = false;
            string errorMessage = string.Empty;

            try
            {
                foreach (var timeSeries in aggregatedTimeSeries)
                {
                    timeSeries.Source = source;
                }

                _timeSeriesDataContext.AggregatedTimeSeries.AddRange(aggregatedTimeSeries);

                var affRows = await _timeSeriesDataContext.SaveChangesAsync(cancellationToken);
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
