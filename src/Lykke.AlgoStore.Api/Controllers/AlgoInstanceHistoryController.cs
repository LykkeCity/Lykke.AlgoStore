using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [RequirePermissionAttribute]
    [Route("api/v1/history")]
    public class AlgoInstanceHistoryController : Controller
    {
        private readonly IAlgoInstanceHistoryService _service;
        private readonly ILog _log;

        public AlgoInstanceHistoryController(IAlgoInstanceHistoryService service, ILog log)
        {
            this._service = service;
            this._log = log;
        }

        /// <summary>
        /// Get history function values for instance
        /// </summary>
        /// <param name="instanceId">Instance ID</param>
        /// <param name="fromMoment">From moment in ISO 8601</param>
        /// <param name="toMoment">To moment in ISO 8601</param>
        [HttpGet("functions")]
        [SwaggerOperation("GetHistoryFunctions")]
        [Description("Get history function values")]
        [ProducesResponseType(typeof(IEnumerable<FunctionChartingUpdate>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetHistoryFunctions([FromQuery][Required]string instanceId, [FromQuery]DateTime fromMoment, [FromQuery]DateTime toMoment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ErrorResponse.Create(ModelState));
                }

                var functions = await _service.GetFunctionsAsync(instanceId, fromMoment.ToUniversalTime(), toMoment.ToUniversalTime(), User.GetClientId(), ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ErrorResponse.Create(ModelState));
                }

                if (functions == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }

                return Ok(functions);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(AlgoInstanceHistoryController), nameof(GetHistoryFunctions), ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Get history trades for instance
        /// </summary>
        /// <param name="instanceId">Instance ID</param>
        /// <param name="tradedAssetId">Traded asset</param>
        /// <param name="fromMoment">From moment in ISO 8601</param>
        /// <param name="toMoment">To moment in ISO 8601</param>
        [HttpGet("trades")]
        [SwaggerOperation("GetHistoryTrades")]
        [Description("Get history trades")]
        [ProducesResponseType(typeof(IEnumerable<TradeChartingUpdate>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetHistoryTrades([FromQuery][Required]string instanceId, [FromQuery][Required]string tradedAssetId, [FromQuery]DateTime fromMoment, [FromQuery]DateTime toMoment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ErrorResponse.Create(ModelState));
                }
                
                var trades = await _service.GetTradesAsync(instanceId, tradedAssetId, fromMoment, toMoment, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ErrorResponse.Create(ModelState));
                }

                if (trades == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                }
                var result = trades.Select(AutoMapper.Mapper.Map<TradeChartingUpdate>).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(AlgoInstanceHistoryController), nameof(GetHistoryTrades), ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        /// <summary>
        /// Get history candles
        /// </summary>
        /// <param name="assetPairId">Asset pair ID</param>
        /// <param name="priceType">Price type</param>
        /// <param name="timeInterval">Time interval</param>
        /// <param name="fromMoment">From moment in ISO 8601 (inclusive)</param>
        /// <param name="toMoment">To moment in ISO 8601 (exclusive)</param>
        [HttpGet("candles/{assetPairId}/{priceType}/{timeInterval}/{fromMoment:datetime}/{toMoment:datetime}")]
        [SwaggerOperation("GetHistoryCandles")]
        [Description("Get history candles")]
        [ProducesResponseType(typeof(IEnumerable<CandleChartingUpdate>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetHistoryCandles(string assetPairId, CandlePriceType priceType, CandleTimeInterval timeInterval, DateTime fromMoment, DateTime toMoment)
        {
            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(90));
                var candles = await _service.GetCandlesAsync(assetPairId, priceType, timeInterval, fromMoment, toMoment, ModelState, cts.Token);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ErrorResponse.Create(ModelState));
                }

                if (candles == null)
                {
                    return StatusCode((int) HttpStatusCode.InternalServerError);
                }
                var result = candles.Select(AutoMapper.Mapper.Map<CandleChartingUpdate>).ToList();
                return Ok(result);
            }
            catch (OperationCanceledException ex)
            {
                Response.Headers.Add("Retry-After", "60");
                await _log.WriteErrorAsync(nameof(AlgoInstanceHistoryController), nameof(GetHistoryCandles), "Call to Lykke Candles service timed out. Requested data was too big or the service is unavailable", ex);
                return StatusCode((int) HttpStatusCode.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(AlgoInstanceHistoryController),nameof(GetHistoryCandles),ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
