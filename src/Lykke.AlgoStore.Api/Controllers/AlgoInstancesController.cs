using AutoMapper;
using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Constants;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Domain.Errors;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Lykke.AlgoStore.Core.Validation;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
        [DescriptionAttribute("Gives users the ability to see all instances for a specific algo")]
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
        [DescriptionAttribute("Gives users the ability to see all available information about an instance")]
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

        [HttpGet("{instanceId}/status")]
        [SwaggerOperation("GetInstanceStatusAsync")]
        [DescriptionAttribute("Gives users the ability to see the current deployment status of an instance")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceStatus), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetInstanceStatusAsync(string instanceId)
        {
            var clientId = User.GetClientId();

            var data = await _algoInstancesService.GetAlgoInstanceDataAsync(clientId, instanceId);

            if (data.InstanceId == null)
                return NotFound();

            return Ok(data.AlgoInstanceStatus);
        }

        private ErrorResponse ValidateDateTimeParameters(AlgoMetaDataInformationModel algoMetaData)
        {
            var dtType = typeof(DateTime).FullName;

            var parameters = algoMetaData.Parameters.ToList();
            parameters.AddRange(algoMetaData.Functions.SelectMany(f => f.Parameters));
            parameters = parameters.Where(p => p.Type == dtType).Select(p => p).ToList();

            foreach (var param in parameters)
            {
                if (!DateTimeFormatValidator.IsDateTimeStringValid(param.Value))
                {
                    return new ErrorResponse()
                    {
                        ErrorCode = (int)AlgoStoreErrorCodes.ValidationError,
                        DisplayMessage = "Parameter DateTime format error",
                        ErrorMessage = $"Invalid DateTime format parameter. Name: {param.Key}, Value: {param.Value}",
                        ErrorDescription = $"Expected DateTime format is {AlgoStoreConstants.DateTimeFormat}"
                    };
                }
            }
            return null;
        }

        private void SetInstanceMetaDataProperties(AlgoClientInstanceData data, AlgoMetaDataInformationModel metaData)
        {
            data.AssetPairId = metaData.Parameters.SingleOrDefault(t => t.Key == "AssetPair")?.Value;
            data.Volume = Convert.ToDouble(metaData.Parameters.SingleOrDefault(t => t.Key == "Volume")?.Value);
            data.TradedAssetId = metaData.Parameters.SingleOrDefault(t => t.Key == "TradedAsset")?.Value;

            //When we create/edit algo instance and save it we call deploy process after that, that's why we set it's status to deploying.
            data.AlgoInstanceStatus = CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceStatus.Deploying;
        }

        [HttpPost("saveAlgoInstance")]
        [SwaggerOperation("SaveAlgoInstanceDataAsync")]
        [DescriptionAttribute("Gives users the ability to create Live instances")]
        [ProducesResponseType(typeof(AlgoClientInstanceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SaveAlgoInstanceDataAsync([FromBody]AlgoClientInstanceModel model)
        {
            var data = Mapper.Map<AlgoClientInstanceData>(model);
            data.ClientId = User.GetClientId();

            var validationError = ValidateDateTimeParameters(model.AlgoMetaDataInformation);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            await _algoInstancesService.ValidateAlgoInstancesDeploymentLimits(data.AlgoId, data.ClientId);

            SetInstanceMetaDataProperties(data, model.AlgoMetaDataInformation);

            var result = await _algoInstancesService.SaveAlgoInstanceDataAsync(data, model.AlgoClientId);

            var deployStatus = await DeployAlgoInstance(data, result.InstanceId);

            var response = Mapper.Map<AlgoClientInstanceModel>(result);
            response.IsAlgoInstanceDeployed = deployStatus;
            return Ok(response);
        }

        [HttpPut("{instanceId}/name")]
        [SwaggerOperation("SetInstanceNameAsync")]
        [DescriptionAttribute("Allows the changing of the name of an existing algo instance")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetInstanceNameAsync(string instanceId, [FromBody] NameModel model)
        {
            var clientId = User.GetClientId();

            if (!model.ValidateData(out var exception))
                throw exception;

            var data = await _algoInstancesService.GetAlgoInstanceDataAsync(clientId, instanceId);

            if (data.InstanceId == null)
                return NotFound();

            data.InstanceName = model.Name;

            if (data.AlgoInstanceType == CSharp.AlgoTemplate.Models.Enumerators.AlgoInstanceType.Live)
                await _algoInstancesService.SaveAlgoInstanceDataAsync(data, data.AlgoClientId);
            else
                await _algoInstancesService.SaveAlgoFakeTradingInstanceDataAsync(data, data.AlgoClientId);

            return Ok();
        }

        [HttpPost("fakeTradingInstanceData")]
        [SwaggerOperation("SaveAlgoFakeTradingInstanceDataAsync")]
        [DescriptionAttribute("Gives users the ability to create Demo / Backtest instances")]
        [ProducesResponseType(typeof(AlgoFakeTradingInstanceModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SaveAlgoFakeTradingInstanceDataAsync([FromBody]AlgoFakeTradingInstanceModel model)
        {
            var data = Mapper.Map<AlgoClientInstanceData>(model);
            data.ClientId = User.GetClientId();

            var validationError = ValidateDateTimeParameters(model.AlgoMetaDataInformation);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            await _algoInstancesService.ValidateAlgoInstancesDeploymentLimits(data.AlgoId, data.ClientId);

            SetInstanceMetaDataProperties(data, model.AlgoMetaDataInformation);

            var result = await _algoInstancesService.SaveAlgoFakeTradingInstanceDataAsync(data, model.AlgoClientId);

            var deployStatus = await DeployAlgoInstance(data, result.InstanceId);

            var response = Mapper.Map<AlgoFakeTradingInstanceModel>(result);
            response.IsAlgoInstanceDeployed = deployStatus;
            return Ok(response);
        }

        [HttpDelete]
        [SwaggerOperation("DeleteAlgoInstanceDataAsync")]
        [DescriptionAttribute("Allows the deletion of an instance and all it's data")]
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

        [HttpGet("userInstances")]
        [SwaggerOperation("GetInstancesForUser")]
        [DescriptionAttribute("Allows users to see a list of all of his instances")]
        [ProducesResponseType(typeof(List<UserInstanceModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInstancesForUser()
        {
            var clientId = User.GetClientId();

            var result = await _algoInstancesService.GetUserInstancesAsync(clientId);

            return Ok(Mapper.Map<List<UserInstanceModel>>(result));
        }

        private async Task<bool> DeployAlgoInstance(AlgoClientInstanceData data, string instanceId)
        {
            ManageImageData dataToDeploy = new ManageImageData()
            {
                AlgoId = data.AlgoId,
                ClientId = data.ClientId,
                AlgoClientId = data.AlgoClientId,
                InstanceId = instanceId
            };

            return await _service.DeployImageAsync(dataToDeploy);
        }
    }
}
