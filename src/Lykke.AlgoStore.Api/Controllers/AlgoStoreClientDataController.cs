﻿using System.Collections.Generic;
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
    [Route("api/v1/clientData")]
    public class AlgoClientDataController : Controller
    {
        private readonly IAlgoStoreClientDataService _clientDataService;
        private readonly IAlgoStoreService _service;

        public AlgoClientDataController(
            IAlgoStoreClientDataService clientDataService,
            IAlgoStoreService service)
        {
            _clientDataService = clientDataService;
            _service = service;
        }

        [HttpGet("getAllAlgos")]
        [SwaggerOperation("GetAllAlgos")]
        [ProducesResponseType(typeof(List<AlgoRatingMetaDataModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllAlgos()
        {
            var result = await _clientDataService.GetAllAlgosWithRatingAsync();

            if (result == null || result.IsNullOrEmptyCollection())
                return NotFound();

            var response = Mapper.Map<List<AlgoRatingMetaDataModel>>(result);

            return Ok(response);
        }

        [HttpGet("getCurrentUserAlgos")]
        [SwaggerOperation("GetCurrentUserAlgos")]
        [ProducesResponseType(typeof(List<AlgoMetaDataModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCurrentUserAlgos()
        {
            var clientId = User.GetClientId();
            var result = await _clientDataService.GetClientAlgosAsync(clientId);

            if (result == null)
                return NotFound();

            var response = Mapper.Map<List<AlgoMetaDataModel>>(result);

            return Ok(response);
        }

        [HttpPost("algoRating")]
        [SwaggerOperation("algoRating")]
        [ProducesResponseType(typeof(AlgoRatingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RateAlgo([FromBody] AlgoRatingModel model)
        {
            var data = Mapper.Map<AlgoRatingData>(model);

            data.ClientId = User.GetClientId();

            var result = await _clientDataService.SaveAlgoRatingAsync(data);

            var response = Mapper.Map<AlgoRatingModel>(result);

            return Ok(response);
        }

        [HttpGet("algoRating")]
        [SwaggerOperation("algoRating")]
        [ProducesResponseType(typeof(AlgoRatingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAlgoRating(string algoId)
        {
            var clientId = User.GetClientId();

            var result = await _clientDataService.GetAlgoRatingAsync(algoId, clientId);

            if (result == null)
                return NotFound();

            var response = Mapper.Map<AlgoRatingModel>(result);

            return Ok(response);
        }

        [HttpGet("userAlgoRating")]
        [SwaggerOperation("userAlgoRating")]
        [ProducesResponseType(typeof(AlgoRatingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUserAlgoRating(string algoId, string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                clientId = User.GetClientId();

            var result = await _clientDataService.GetAlgoRatingForClientAsync(algoId, clientId);

            if (result == null)
                return NotFound();

            var response = Mapper.Map<AlgoRatingModel>(result);

            return Ok(response);
        }

        [HttpPost("addToPublic")]
        [SwaggerOperation("addToPublic")]
        [ProducesResponseType(typeof(PublicAlgoDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddToPublic([FromBody] PublicAlgoDataModel model)
        {
            var clientId = User.GetClientId();
            var data = Mapper.Map<PublicAlgoData>(model);

            var result = await _clientDataService.AddToPublicAsync(data, clientId);

            var response = Mapper.Map<PublicAlgoDataModel>(result);

            return Ok(response);

        }

        [HttpPost("removeFromPublic")]
        [SwaggerOperation("removeFromPublic")]
        [ProducesResponseType(typeof(PublicAlgoDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemoveFromPublic([FromBody] PublicAlgoDataModel model)
        {
            var clientId = User.GetClientId();
            var data = Mapper.Map<PublicAlgoData>(model);

            var result = await _clientDataService.RemoveFromPublicAsync(data, clientId);

            var response = Mapper.Map<PublicAlgoDataModel>(result);

            return Ok(response);

        }

        [HttpGet("algoMetadata")]
        [SwaggerOperation("GetAlgoMetadata")]
        [ProducesResponseType(typeof(AlgoDataInformationModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAlgoMetadata(string clientId, string algoId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                clientId = User.GetClientId();

            var result = await _clientDataService.GetAlgoDataInformationAsync(clientId, algoId);

            if (result == null)
                return NotFound();

            var response = Mapper.Map<AlgoDataInformationModel>(result);

            return Ok(response);
        }

        [HttpPost("metadata/cascadeDelete")]
        [SwaggerOperation("CascadeDeleteAlgoMetadata")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteAlgoMetadata([FromBody]ManageImageModel model)
        {
            var clientId = User.GetClientId();

            var data = Mapper.Map<ManageImageData>(model);
            data.ClientId = clientId;

            var clientInstanceData = await _clientDataService.ValidateCascadeDeleteClientMetadataRequestAsync(data);

            await _service.DeleteImageAsync(clientInstanceData);

            await _clientDataService.DeleteMetadataAsync(data);

            return NoContent();
        }

        [HttpPost("imageData/upload/binary")]
        [SwaggerOperation("UploadBinaryFile")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ServiceFilter(typeof(ValidateMimeMultipartContentFilter))]
        public async Task<IActionResult> UploadBinaryFile(UploadAlgoBinaryModel model)
        {
            var clientId = User.GetClientId();

            var data = Mapper.Map<UploadAlgoBinaryData>(model);

            await _clientDataService.SaveAlgoAsBinaryAsync(clientId, data);

            return NoContent();
        }

        [HttpPost("imageData/upload/string")]
        [SwaggerOperation("UploadString")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UploadSting([FromBody]UploadAlgoStringModel model)
        {
            var clientId = User.GetClientId();

            var data = Mapper.Map<UploadAlgoStringData>(model);

            await _clientDataService.SaveAlgoAsStringAsync(clientId, data);

            return NoContent();
        }

        [HttpGet("imageData/upload/string")]
        [SwaggerOperation("GetUploadString")]
        [ProducesResponseType(typeof(DataStringModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUploadString(string clientId, string algoId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                clientId = User.GetClientId();

            var data = await _clientDataService.GetAlgoAsStringAsync(clientId, algoId);

            return Ok(new DataStringModel
            {
                Data = data
            });
        }

        [HttpGet("instanceData/allByAlgoIdAndClientId")]
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

            var result = await _clientDataService.GetAllAlgoInstanceDataByAlgoIdAndClientIdAsync(data);
            var response = Mapper.Map<List<AlgoClientInstanceModel>>(result);

            return Ok(response);
        }

        [HttpGet("instanceData")]
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

            var result = await _clientDataService.GetAlgoInstanceDataAsync(data);

            if (result == null || result.AlgoId == null)
                return NotFound();

            var response = Mapper.Map<AlgoClientInstanceModel>(result);
            return Ok(response);
        }

        [HttpPost("instanceData")]
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

            var result = await _clientDataService.SaveAlgoInstanceDataAsync(data, model.AlgoClientId);
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

            var result = await _clientDataService.SaveAlgoBackTestInstanceDataAsync(data, model.AlgoClientId);
            var response = Mapper.Map<AlgoBackTestInstanceModel>(result);

            return Ok(response);
        }

        [HttpDelete("instanceData")]
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

            var clientInstanceData = await _clientDataService.ValidateCascadeDeleteClientMetadataRequestAsync(data);

            await _service.DeleteInstanceAsync(clientInstanceData);

            return NoContent();
        }
    }
}
