using System.Threading.Tasks;
using Lykke.AlgoStore.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Route("api/v1/algoInstances")]
    public class TeamCityController : Controller
    {
        [HttpPost("updateBuildStatus")]
        public async Task<IActionResult> UpdateAlgoInstanceStatus([FromBody] TeamCityWebHookResponseModel payload)
        {
            if (payload == default(TeamCityWebHookResponseModel))
                return NotFound();

            return Ok();
        }
    }
}
