using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Api.Hubs;
using TimeSeries.ServiceBus.Common;
using TimeSeries.Shared.Contracts.DataStore;
using TimeSeries.Shared.Contracts.Settings;
using ApiContracts = TimeSeries.Shared.Contracts.Api;
using Entities = TimeSeries.Shared.Contracts.Entities;

namespace TimeSeries.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    [Route("api/timeseries/v{version:apiVersion}/[controller]")]
    public class WriteDataController : ControllerBase
    {
        private readonly ILogger<WriteDataController> _logger;
        private readonly IWriteData<Entities.MultiValueTimeSeries> _dataWriter;
        private readonly IWriteSource<string> _sourceWriter;
        private readonly IMapper _mapper;
        private readonly IProducer _messageBus;
        private readonly IHubContext<RealtimeDataHub> _hubContext;

        public WriteDataController(ILogger<WriteDataController> logger,
            IWriteData<Entities.MultiValueTimeSeries> dataWriter,
            IWriteSource<string> sourceWriter,
            IMapper mapper,
            IProducer messageBus,
            IHubContext<RealtimeDataHub> hubContext)
        {
            _dataWriter = dataWriter;
            _sourceWriter = sourceWriter;
            _mapper = mapper;
            _logger = logger;
            _messageBus = messageBus;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody][Required] string[] sources,
            CancellationToken token)
        {
            _logger.LogDebug($"Received {sources.Length} new sources");

            var result = await _sourceWriter.AddSources(sources, token);
            return result.IsSuccess ?
                Ok(new ApiContracts.WriteResponse
                {
                    IsSuccess = true
                }) :
                StatusCode(500, new ApiContracts.WriteResponse
                {
                    IsSuccess = false,
                    ErrorMessage = result.ErrorMessage
                });
        }

        [HttpPost("{sourceId}")]
        public async Task<IActionResult> Post(string sourceId,
            [FromBody] ApiContracts.MultiValueTimeSeries[] data,
            CancellationToken token)
        {
            _logger.LogDebug($"Received {data.Length} data points for {sourceId}");

            try
            {
                var svcData = _mapper.Map<Entities.MultiValueTimeSeries[]>(data);
                var result = await _dataWriter.AddTimeSeriesData(sourceId, svcData, token);
                return result.IsSuccess ?
                    Ok(new ApiContracts.WriteResponse
                    {
                        IsSuccess = true
                    }) :
                    StatusCode(500, new ApiContracts.WriteResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = result.ErrorMessage
                    });
            }
            catch (AutoMapperMappingException ex)
            {
                var errorMessage = "Error occured while trying to map DTO to internal Model.";
                _logger.LogError(errorMessage, ex);
                return BadRequest(errorMessage);
            }
        }

        [MapToApiVersion("2.0")]
        [HttpPost("{sourceId}")]
        public async Task<IActionResult> PostToServiceBus(string sourceId,
            [FromBody] ApiContracts.MultiValueTimeSeries[] data,
            CancellationToken token)
        {
            _logger.LogDebug($"Received {data.Length} data points for {sourceId}");

            try
            {
                var svcData = _mapper.Map<Entities.MultiValueTimeSeries[]>(data);
                await _messageBus.Send(MessageBusQueue.RAW_DATA, sourceId, svcData, token);

                return Ok(new ApiContracts.WriteResponse
                {
                    IsSuccess = true
                });
            }
            catch (AutoMapperMappingException ex)
            {
                var errorMessage = "Error occured while trying to map DTO to internal Model.";
                _logger.LogError(errorMessage, ex);
                return BadRequest(errorMessage);
            }
            catch (Exception ex)
            {
                var errorMessage = "Error occured while sending message broker.";
                _logger.LogError(errorMessage, ex);
                return StatusCode(500, errorMessage);
            }
        }
    }
}