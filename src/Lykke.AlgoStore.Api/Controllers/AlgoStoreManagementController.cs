using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [Route("api/v1/management")]
    public class AlgoStoreManagementController : Controller
    {
        private readonly IAlgoStoreService _service;

        public AlgoStoreManagementController(IAlgoStoreService service)
        {
            _service = service;
        }

        [HttpPost("deploy/binary")]
        [SwaggerOperation("DeployBinaryImage")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeployBinaryImage([FromBody]ManageImageModel model)
        {
            var data = Mapper.Map<ManageImageData>(model);
            data.ClientId = User.GetClientId();

            var result = await _service.DeployImage(data);

            return Ok(result);
        }

        [HttpPost("test/start")]
        [SwaggerOperation("StartTest")]
        [ProducesResponseType(typeof(StatusModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartTest([FromBody]ManageImageModel model)
        {
            var data = Mapper.Map<ManageImageData>(model);
            data.ClientId = User.GetClientId();

            var result = new StatusModel();
            result.Status = await _service.StartTestImage(data);

            return Ok(result);
        }
    }
}
