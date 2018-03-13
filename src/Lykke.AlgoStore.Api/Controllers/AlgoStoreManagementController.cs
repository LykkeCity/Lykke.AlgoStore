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
        private readonly IAlgoStoreClientDataService _clientDataService;
        private readonly IAlgoStoreService _service;

        public AlgoStoreManagementController(IAlgoStoreClientDataService clientDataService, IAlgoStoreService service)
        {
            _clientDataService = clientDataService;
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

            var result = await _service.DeployImageAsync(data);

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
            result.Status = await _service.StartTestImageAsync(data);

            return Ok(result);
        }

        [HttpPost("test/stop")]
        [SwaggerOperation("StopTest")]
        [ProducesResponseType(typeof(StatusModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StopTest([FromBody]ManageImageModel model)
        {
            var data = Mapper.Map<ManageImageData>(model);
            data.ClientId = User.GetClientId();

            var result = new StatusModel();
            result.Status = await _service.StopTestImageAsync(data);

            var clientInstanceData = await _clientDataService.ValidateCascadeDeleteClientMetadataRequestAsync(data);

            await _service.DeleteImageAsync(clientInstanceData);

            return Ok(result);
        }

        [HttpGet("test/tailLog")]
        [SwaggerOperation("GetTestTailLog")]
        [ProducesResponseType(typeof(LogModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetTestTailLog(TailLogModel model)
        {
            var data = Mapper.Map<TailLogData>(model);
            data.ClientId = User.GetClientId();

            var result = new LogModel();
            result.Log = await _service.GetTestTailLogAsync(data);

            return Ok(result);
        }
    }
}
