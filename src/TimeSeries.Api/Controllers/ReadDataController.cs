using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.DataStore;
using TimeSeries.Shared.Contracts.Entities;
using TimeSeries.Shared.Contracts.Services;
using ApiContracts = TimeSeries.Shared.Contracts.Api;
using System.Linq;

namespace TimeSeries.Api.Controllers
{
    [ApiController]
    [Route("api/timeseries/[controller]")]
    public class ReadDataController : ControllerBase
    {
        private readonly IReadSource<string> _sourceReader;
        private readonly IReadData<MultiValueTimeSeries> _dataReader;
        private readonly IMapper _mapper;

        public ReadDataController(IReadSource<string> sourceReader, IReadData<MultiValueTimeSeries> dataReader, IMapper mapper)
        {
            _sourceReader = sourceReader;
            _dataReader = dataReader;
            _mapper = mapper;
        }

        [HttpGet("sources")]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var svcResponse = await _sourceReader.GetAllSources(token);
            var apiResponse = _mapper.Map<ApiContracts.ReadResponse<List<string>>>(svcResponse);

            return Ok(apiResponse);
        }

        [HttpGet("aggregationtypes")]
        public async Task<IActionResult> GetAggregationTypes(CancellationToken token)
        {
            var response = await Task.FromResult(new ApiContracts.ReadResponse<string[]>
            {
                Data = Enum.GetValues<AggregationType>().Select(a => a.ToString()).ToArray(),
                IsSuccess = true,
                ErrorMessage = string.Empty
            });

            return Ok(response);
        }

        [HttpGet("{sourceId}/historic")]
        public async Task<IActionResult> Get([FromRoute][Required] string sourceId,
            [FromQuery][Required] DateTime from,
            [FromQuery][Required] DateTime to,
            CancellationToken token)
        {
            var svcResponse = await _dataReader.GetHistoric(sourceId, from.ToUniversalTime(), to.ToUniversalTime(), token);
            var apiResponse = _mapper.Map<ApiContracts.ReadResponse<List<ApiContracts.MultiValueTimeSeries>>>(svcResponse);

            return Ok(apiResponse);
        }

        [HttpGet("{sourceId}/getlatest")]
        public async Task<IActionResult> Get(string sourceId, CancellationToken token)
        {
            var svcResponse = await _dataReader.GetLatest(sourceId, token);
            var apiResponse = _mapper.Map<ApiContracts.ReadResponse<ApiContracts.MultiValueTimeSeries>>(svcResponse);

            return Ok(apiResponse);
        }
    }
}