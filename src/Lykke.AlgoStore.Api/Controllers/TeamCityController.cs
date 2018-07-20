using System.Threading.Tasks;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.AlgoStore.Api.Controllers
{
    //[Authorize]
    [Route("api/v1/algoInstances")]
    public class TeamCityController : Controller
    {
        private readonly IAlgoStoreService _service;

        public TeamCityController(IAlgoStoreService service)
        {
            _service = service;
        }

        [HttpPost("updateBuildStatus")]
        public async Task<IActionResult> UpdateAlgoInstanceStatus([FromBody] TeamCityWebHookResponseModel payload)
        {
            if (payload == default(TeamCityWebHookResponseModel))
                return NotFound();

            var webHook = AutoMapper.Mapper.Map<TeamCityWebHookResponse>(payload);

            if (await _service.UpdateAlgoInstanceStatusAsync(webHook))
                return Ok();
            else
                return BadRequest();
        }
    }
}
