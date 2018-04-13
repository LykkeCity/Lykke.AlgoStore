using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [Route("api/v1/statistics")]
    public class AlgoStoreStatisticsController : Controller
    {
        private readonly IAlgoStoreStatisticsService _statisticsService;
        private readonly IAlgoStoreClientDataService _algoStoreClientDataService;

        public AlgoStoreStatisticsController(IAlgoStoreStatisticsService statisticsService, IAlgoStoreClientDataService algoStoreClientDataService)
        {
            _statisticsService = statisticsService;
            _algoStoreClientDataService = algoStoreClientDataService;
        }

        [HttpGet]
        [SwaggerOperation("GetAlgoInstanceStatisticsAsync")]
        [ProducesResponseType(typeof(StatisticsSummary), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAlgoInstanceStatisticsAsync(string instanceId)
        {
            StatisticsSummary result;
            var algoInstance = await _algoStoreClientDataService.GetAlgoInstanceDataAsync(User.GetClientId(), instanceId);
            
            if (algoInstance.AlgoInstanceStatus == CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceStatus.Stopped)
                result = await _statisticsService.GetStatisticsSummaryAsync(User.GetClientId(), instanceId);
            else
                result = await _statisticsService.UpdateStatisticsSummaryAsync(User.GetClientId(), instanceId);

            return Ok(result);
        }
    }
}
