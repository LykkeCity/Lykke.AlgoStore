﻿using AutoMapper;
using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.ContentFilters;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [RequirePermission]
    [Route("api/v1/algo")]
    public class AlgoController : Controller
    {
        private readonly IAlgosService _clientDataService;
        private readonly IAlgoStoreService _service;
        private readonly IAlgoInstancesService _algoInstancesService;

        public AlgoController(IAlgosService clientDataService, IAlgoStoreService service, IAlgoInstancesService algoInstancesService)
        {
            _clientDataService = clientDataService;
            _service = service;
            _algoInstancesService = algoInstancesService;
        }

        //REMARK: This endpoint is a merge result between 'metadata - POST' and 'imageData/upload/string - POST' endpoints
        //In future, when we do a refactoring we should remove unused endpoints from code
        [HttpPost("create")]
        [SwaggerOperation("CreateAlgo")]
        [ProducesResponseType(typeof(AlgoDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAlgo([FromBody] CreateAlgoModel model)
        {
            var data = Mapper.Map<AlgoData>(model);
            data.ClientId = User.GetClientId();

            var result = await _clientDataService.CreateAlgoAsync(data, model.DecodedContent);

            var response = Mapper.Map<AlgoDataModel>(result);

            return Ok(response);
        }

        [HttpPost("edit")]
        [SwaggerOperation("EditAlgo")]
        [ProducesResponseType(typeof(AlgoDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> EditAlgo([FromBody] CreateAlgoModel model)
        {
            var data = Mapper.Map<AlgoData>(model);
            data.ClientId = User.GetClientId();

            var result = await _clientDataService.EditAlgoAsync(data, model.DecodedContent);

            var response = Mapper.Map<AlgoDataModel>(result);

            return Ok(response);
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

            var clientInstanceData = await _algoInstancesService.ValidateCascadeDeleteClientMetadataRequestAsync(data);

            await _service.DeleteImageAsync(clientInstanceData);

            await _clientDataService.DeleteMetadataAsync(data);

            return NoContent();
        }

        [HttpPost("sourceCode/upload/binary")]
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

        [HttpPost("sourceCode/upload/string")]
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

        [HttpGet("sourceCode/getString")]
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
    }
}
