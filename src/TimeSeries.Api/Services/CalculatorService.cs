using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.gRPC.Server;
using TimeSeries.Shared.Contracts.Api;
using TimeSeries.Shared.Contracts.Services;
using TimeSeries.Shared.Contracts.Settings;

namespace TimeSeries.Api.Services
{
    public class CalculatorService : ICalculatorService
    {
        private readonly ServiceSettings _serviceSettings;
        private readonly IMapper _mapper;

        public CalculatorService(ServiceSettings serviceSettings, IMapper mapper)
        {
            _serviceSettings = serviceSettings;
            _mapper = mapper;
        }

        public async Task<ReadResponse<List<AggrTimeSeriesData>>> GetHistoric(AggregationType aggregateType,
            string timeSeriesSourceId,
            DateTime from,
            DateTime to,
            CancellationToken cancellationToken)
        {
            var channel = GrpcChannel.ForAddress(GetAggregateServiceUri(aggregateType));
            var client = new AggregatedTimeSeriesService.AggregatedTimeSeriesServiceClient(channel);

            var response = await client.GetHistoricAsync(new GetHistoricRequest
            {
                TimeSeriesSourceId = timeSeriesSourceId,
                From = Timestamp.FromDateTime(from.ToUniversalTime()),
                To = Timestamp.FromDateTime(to.ToUniversalTime())
            }, cancellationToken: cancellationToken);

            return _mapper.Map<ReadResponse<List<AggrTimeSeriesData>>>(response);
        }

        public async Task<ReadResponse<AggrTimeSeriesData>> GetLatest(AggregationType aggregateType, 
            string timeSeriesSourceId, 
            CancellationToken cancellationToken)
        {
            var channel = GrpcChannel.ForAddress(GetAggregateServiceUri(aggregateType));
            var client = new AggregatedTimeSeriesService.AggregatedTimeSeriesServiceClient(channel);

            var response = await client.GetLatestAsync(new GetLatestRequest
            {
                TimeSeriesSourceId = timeSeriesSourceId
            }, cancellationToken: cancellationToken);

            return _mapper.Map<ReadResponse<AggrTimeSeriesData>>(response);
        }

        private string GetAggregateServiceUri(AggregationType aggregateType) => aggregateType switch
        {
            AggregationType.Max => _serviceSettings.MaxCalculatorSvcUri,
            AggregationType.Min => _serviceSettings.MinCalculatorSvcUri,
            AggregationType.Avg => _serviceSettings.AvgCalculatorSvcUri,
            _ => _serviceSettings.AvgCalculatorSvcUri
        };
    }
}