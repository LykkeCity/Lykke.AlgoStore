﻿using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Controllers
{
    //[Authorize]
    [Route("api/v001/[controller]")]
    public class AlgoStoreManagementController : Controller
    {
        private readonly ILog _log;
        private readonly IAlgoStoreService _service;

        public AlgoStoreManagementController(ILog log, IAlgoStoreService service)
        {
            _log = log;
            _service = service;
        }

        [HttpPost("deploy")]
        [SwaggerOperation("DeployAlgo")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeployImage([FromBody]DeployImageModel model)
        {
            var data = Mapper.Map<DeployImageData>(model);
            data.ClientId = User.GetClientId();

            var result = await _service.DeployImage(data);

            return Ok(result);
        }
    }
}
