using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.Api.Infrastructure.ContentFilters;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Linq;
using System;
using Lykke.AlgoStore.Api.Infrastructure.Attributes;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [RequirePermissionAttribute]
    [Route("api/v1/algoInstances")]
    public class AlgoInstancesController : Controller
    {
        private readonly IAlgoInstancesService _algoInstancesService;
        private readonly IAlgoStoreService _service;

        public AlgoInstancesController(IAlgoInstancesService algoInstancesService,
            IAlgoStoreService service)
        {
            _algoInstancesService = algoInstancesService;
            _service = service;
        }

        [HttpGet("getAllByAlgoIdAndClientId")]
        [SwaggerOperation("GetAllAlgoInstanceDataAsync")]
        [ProducesResponseType(typeof(List<AlgoClientInstanceModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllAlgoInstanceDataAsync(string algoId)
        {
            var data = new CSharp.AlgoTemplate.Models.Models.BaseAlgoData
            {
                ClientId = User.GetClientId(),
                AlgoId = algoId
            };

            var result = await _algoInstancesService.GetAllAlgoInstanceDataByAlgoIdAndClientIdAsync(data);
            var response = Mapper.Map<List<AlgoClientInstanceModel>>(result);

            return Ok(response);
        }

        [HttpGet("getAlgoInstance")]
        [SwaggerOperation("GetAllAlgoInstanceDataAsync")]
        [ProducesResponseType(typeof(AlgoClientInstanceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAlgoInstanceDataAsync(string algoId, string instanceId)
        {
            var data = new BaseAlgoInstance
            {
                ClientId = User.GetClientId(),
                AlgoId = algoId,
                InstanceId = instanceId
            };

            var result = await _algoInstancesService.GetAlgoInstanceDataAsync(data);

            if (result == null || result.AlgoId == null)
                return NotFound();

            var response = Mapper.Map<AlgoClientInstanceModel>(result);
            return Ok(response);
        }

        [HttpPost("saveAlgoInstance")]
        [SwaggerOperation("SaveAlgoInstanceDataAsync")]
        [ProducesResponseType(typeof(AlgoClientInstanceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SaveAlgoInstanceDataAsync([FromBody]AlgoClientInstanceModel model)
        {
            var data = Mapper.Map<AlgoClientInstanceData>(model);
            data.ClientId = User.GetClientId();

            data.AssetPair = model.AlgoMetaDataInformation.Parameters.SingleOrDefault(t => t.Key == "AssetPair")?.Value;
            data.Volume = Convert.ToDouble(model.AlgoMetaDataInformation.Parameters.SingleOrDefault(t => t.Key == "Volume")?.Value);
            data.TradedAsset = model.AlgoMetaDataInformation.Parameters.SingleOrDefault(t => t.Key == "TradedAsset")?.Value;

            //When we create/edit algo instance and save it we call deploy process after that, that's why we set it's status to deploying.
            data.AlgoInstanceStatus = CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceStatus.Deploying;

            var result = await _algoInstancesService.SaveAlgoInstanceDataAsync(data, model.AlgoClientId);
            var response = Mapper.Map<AlgoClientInstanceModel>(result);

            return Ok(response);
        }

        [HttpPost("backTestInstanceData")]
        [SwaggerOperation("SaveAlgoBackTestInstanceDataAsync")]
        [ProducesResponseType(typeof(AlgoBackTestInstanceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SaveAlgoBackTestInstanceDataAsync([FromBody]AlgoBackTestInstanceModel model)
        {
            var data = Mapper.Map<AlgoClientInstanceData>(model);
            data.ClientId = User.GetClientId();

            data.AssetPair = model.AlgoMetaDataInformation.Parameters.SingleOrDefault(t => t.Key == "AssetPair")?.Value;
            data.Volume = Convert.ToDouble(model.AlgoMetaDataInformation.Parameters.SingleOrDefault(t => t.Key == "Volume")?.Value);
            data.TradedAsset = model.AlgoMetaDataInformation.Parameters.SingleOrDefault(t => t.Key == "TradedAsset")?.Value;

            //When we create/edit algo instance and save it we call deploy process after that, that's why we set it's status to deploying.
            data.AlgoInstanceStatus = CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceStatus.Deploying;

            var result = await _algoInstancesService.SaveAlgoBackTestInstanceDataAsync(data, model.AlgoClientId);
            var response = Mapper.Map<AlgoBackTestInstanceModel>(result);

            return Ok(response);
        }

        [HttpDelete]
        [SwaggerOperation("DeleteAlgoInstanceDataAsync")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteAlgoInstanceDataAsync([FromBody] ManageImageModel model)
        {
            var clientId = User.GetClientId();

            var data = Mapper.Map<ManageImageData>(model);
            data.ClientId = clientId;

            var clientInstanceData = await _algoInstancesService.ValidateCascadeDeleteClientMetadataRequestAsync(data);

            await _service.DeleteInstanceAsync(clientInstanceData);

            return NoContent();
        }
    }
}
