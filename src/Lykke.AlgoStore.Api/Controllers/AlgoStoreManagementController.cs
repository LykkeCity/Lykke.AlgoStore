using AutoMapper;
using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [RequirePermissionAttribute]
    [Route("api/v1/management")]
    public class AlgoStoreManagementController : Controller
    {
        private readonly IAlgoStoreService _service;
        private readonly IAlgoStoreStatisticsService _statisticsService;
        private readonly IAlgoInstancesService _algoInstancesService;

        public AlgoStoreManagementController(IAlgoStoreService service,
            IAlgoStoreStatisticsService algoStoreStatisticsService, IAlgoInstancesService algoInstancesService)
        {
            _service = service;
            _statisticsService = algoStoreStatisticsService;
            _algoInstancesService = algoInstancesService;
        }

        [HttpPost("stop")]
        [SwaggerOperation("StopTest")]
        [DescriptionAttribute("Allows users to stop a running instance")]
        [ProducesResponseType(typeof(StatusModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StopTest([FromBody]ManageImageModel model)
        {
            var data = Mapper.Map<ManageImageData>(model);
            data.ClientId = User.GetClientId();

            var result = new StatusModel();
            result.Status = await _service.StopTestImageAsync(data);

            if (result.Status == AlgoInstanceStatus.Stopped.ToString())
            {
                await _statisticsService.UpdateStatisticsSummaryAsync(data.ClientId, data.InstanceId);
            }

            return Ok(result);
        }

        [HttpGet("tailLog")]
        [SwaggerOperation("GetTestTailLog")]
        [DescriptionAttribute("Gives you the ability to see the logs of the instance")]
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
