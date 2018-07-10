using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Service.AlgoTrades.Client.AutorestClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [RequirePermissionAttribute]
    [Route("api/v1/trades")]
    public class AlgoStoreTradesController : Controller
    {
        private readonly IAlgoStoreTradesService _tradesService;

        public AlgoStoreTradesController(IAlgoStoreTradesService tradesService)
        {
            _tradesService = tradesService;
        }

        [HttpGet]
        [SwaggerOperation("GetAllTradesForAlgoInstanceAsync")]
        [DescriptionAttribute("Gives users the ability to see the trades of the instance")]
        [ProducesResponseType(typeof(IEnumerable<AlgoInstanceTradeResponseModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllTradesForAlgoInstanceAsync(string instanceId)
        {
            var result = await _tradesService.GetAllTradesForAlgoInstanceAsync(User.GetClientId(), instanceId);
            return Ok(result);
        }
    }
}
