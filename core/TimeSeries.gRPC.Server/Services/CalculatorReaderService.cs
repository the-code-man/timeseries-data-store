using AutoMapper;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.DataStore;

namespace TimeSeries.gRPC.Server.Services
{
    public class CalculatorReaderService : AggregatedTimeSeriesService.AggregatedTimeSeriesServiceBase
    {
        private readonly ILogger<CalculatorReaderService> _logger;
        private readonly IReadData<Shared.Contracts.Entities.SingleValueTimeSeries> _dataReader;
        private readonly IMapper _mapper;

        public CalculatorReaderService(ILogger<CalculatorReaderService> logger, IReadData<Shared.Contracts.Entities.SingleValueTimeSeries> dataReader, IMapper mapper)
        {
            _logger = logger;
            _dataReader = dataReader;
            _mapper = mapper;
        }

        public override async Task<GetHistoricResponse> GetHistoric(GetHistoricRequest request, ServerCallContext context)
        {
            var response = await _dataReader.GetHistoric(request.TimeSeriesSourceId, request.From.ToDateTime(), request.To.ToDateTime(), context.CancellationToken);

            return new GetHistoricResponse
            {
                TimeSeriesData = { _mapper.Map<List<AggregatedTimeSeries>>(response.Data) },
                ErrorMessage = response.ErrorMessage,
                Success = response.IsSuccess
            };
        }

        public override async Task<GetLatestResponse> GetLatest(GetLatestRequest request, ServerCallContext context)
        {
            var response = await _dataReader.GetLatest(request.TimeSeriesSourceId, context.CancellationToken);

            return new GetLatestResponse
            {
                TimeSeriesData = _mapper.Map<AggregatedTimeSeries>(response.Data),
                ErrorMessage = response.ErrorMessage,
                Success = response.IsSuccess
            };
        }
    }
}
