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

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
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

        [HttpGet("metadata")]
        [SwaggerOperation("GetAlgoMetadata")]
        [ProducesResponseType(typeof(List<AlgoMetaDataModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAlgoMetadata()
        {
            string clientId = User.GetClientId();

            var result = await _clientDataService.GetClientMetadataAsync(clientId);

            if (result == null || result.AlgoMetaData.IsNullOrEmptyCollection())
                return NotFound();

            var response = Mapper.Map<List<AlgoMetaDataModel>>(result.AlgoMetaData);

            return Ok(response);
        }

        [HttpPost("metadata")]
        [SwaggerOperation("SaveAlgoMetadata")]
        [ProducesResponseType(typeof(AlgoMetaDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SaveAlgoMetadata([FromBody]AlgoMetaDataModel model)
        {
            string clientId = User.GetClientId();
            string clientName = User.Identity.Name;

            var data = Mapper.Map<AlgoMetaData>(model);

            var result = await _clientDataService.SaveClientMetadataAsync(clientId, clientName, data);

            var response = Mapper.Map<AlgoMetaDataModel>(result.AlgoMetaData[0]);

            return Ok(response);
        }

        [HttpPost("metadata/cascadeDelete")]
        [SwaggerOperation("CascadeDeleteAlgoMetadata")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteAlgoMetadata([FromBody]AlgoMetaDataModel model)
        {
            var clientId = User.GetClientId();

            var data = Mapper.Map<AlgoMetaData>(model);

            var runtimeData = await _clientDataService.ValidateCascadeDeleteClientMetadataRequestAsync(clientId, data);

            await _service.DeleteImageAsync(runtimeData);

            await _clientDataService.DeleteMetadataAsync(clientId, data);

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
            string clientId = User.GetClientId();

            var data = Mapper.Map<UploadAlgoBinaryData>(model);

            await _clientDataService.SaveAlgoAsBinaryAsync(clientId, data);

            return NoContent();
        }

        [HttpPost("imageData/upload/string")]
        [SwaggerOperation("UploadString")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UploadSting([FromBody] UploadAlgoStringModel model)
        {
            string clientId = User.GetClientId();

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
        public async Task<IActionResult> GetUploadString(string algoId)
        {
            string clientId = User.GetClientId();

            var data = await _clientDataService.GetAlgoAsStringAsync(clientId, algoId);

            return Ok(new DataStringModel
            {
                Data = data
            });
        }

        [HttpGet("instanceData/all")]
        [SwaggerOperation("GetAllAlgoInstanceDataAsync")]
        [ProducesResponseType(typeof(List<AlgoClientInstanceModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllAlgoInstanceDataAsync(string algoId)
        {
            var data = new BaseAlgoData
            {
                ClientId = User.GetClientId(),
                AlgoId = algoId
            };

            var result = await _clientDataService.GetAllAlgoInstanceDataAsync(data);

            if (result.IsNullOrEmptyCollection())
                return NotFound();

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

            if (result == null)
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

            var result = await _clientDataService.SaveAlgoInstanceDataAsync(data);

            var response = Mapper.Map<AlgoClientInstanceModel>(result);

            return Ok(response);
        }
    }
}
