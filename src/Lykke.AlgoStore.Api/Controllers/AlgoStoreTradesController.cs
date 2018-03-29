using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
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
        [ProducesResponseType(typeof(List<Statistics>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllTradesForAlgoInstanceAsync(string instanceId)
        {
            var result = await _tradesService.GetAllTradesForAlgoInstanceAsync(instanceId);

            return Ok(result);
        }
    }
}
