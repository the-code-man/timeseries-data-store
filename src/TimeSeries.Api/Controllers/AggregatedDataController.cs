using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TimeSeries.Shared.Contracts.Services;

namespace TimeSeries.Api.Controllers
{
    [ApiController]
    [Route("api/timeseries/[controller]")]
    public class AggregatedDataController : ControllerBase
    {
        private readonly ILogger<AggregatedDataController> _logger;
        private readonly ICalculatorService _calculatorService;

        public AggregatedDataController(ILogger<AggregatedDataController> logger, ICalculatorService calculatorService)
        {
            _logger = logger;
            _calculatorService = calculatorService;
        }

        [HttpGet("{sourceId}/historic")]
        public async Task<IActionResult> Get([FromRoute][Required] string sourceId,
            [FromQuery][Required] DateTime from,
            [FromQuery][Required] DateTime to,
            [FromQuery] string aggregationType,
            CancellationToken token)
        {
            if (!Enum.TryParse(aggregationType, true, out AggregationType aggrType))
            {
                aggrType = AggregationType.Avg;
            }

            var response = await _calculatorService.GetHistoric(aggrType, sourceId, from.ToUniversalTime(), to.ToUniversalTime(), token);
            return Ok(response);
        }

        [HttpGet("{sourceId}/getlatest")]
        public async Task<IActionResult> Get(string sourceId,
            [FromQuery] string aggregationType,
            CancellationToken token)
        {
            if (!Enum.TryParse(aggregationType, true, out AggregationType aggrType))
            {
                aggrType = AggregationType.Avg;
            }

            var response = await _calculatorService.GetLatest(aggrType, sourceId, token);
            return Ok(response);
        }
    }
}