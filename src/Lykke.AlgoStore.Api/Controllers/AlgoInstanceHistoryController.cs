using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.Services.Utils;
using Lykke.Common.Log;
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
                var result = candles.Select(c => c.ToCandleChartingUpdate()).ToList();
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
