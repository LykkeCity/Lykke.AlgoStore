using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
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
        private readonly ILog _log;
        private readonly IAlgoStoreService _service;

        public AlgoStoreManagementController(ILog log, IAlgoStoreService service)
        {
            _log = log;
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
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartTest([FromBody]ManageImageModel model)
        {
            var data = Mapper.Map<ManageImageData>(model);
            data.ClientId = User.GetClientId();

            var result = await _service.StartTestImage(data);

            return Ok(result);
        }

        [HttpPost("test/stop")]
        [SwaggerOperation("StopTest")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StopTest([FromBody]ManageImageModel model)
        {
            var data = Mapper.Map<ManageImageData>(model);
            data.ClientId = User.GetClientId();

            var result = await _service.StopTestImage(data);

            return Ok(result);
        }

        [HttpGet("test/log")]
        [SwaggerOperation("GetTestLog")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetTestLog(ManageImageModel model)
        {
            var data = Mapper.Map<ManageImageData>(model);
            data.ClientId = User.GetClientId();

            var result = await _service.GetTestLog(data);

            return Ok(result);
        }

        [HttpGet("test/tailLog")]
        [SwaggerOperation("GetTestTailLog")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetTestTailLog(TailLogModel model)
        {
            var data = Mapper.Map<TailLogData>(model);
            data.ClientId = User.GetClientId();

            var result = await _service.GetTestTailLog(data);

            return Ok(result);
        }
    }
}
