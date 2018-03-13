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
            var data = Mapper.Map<PublicAlgoData>(model);

            var result = await _clientDataService.AddToPublicAsync(data);

            var response = Mapper.Map<PublicAlgoDataModel>(result);

            return Ok(response);

        }

        [HttpGet("metadata")]
        [SwaggerOperation("GetAlgoMetadata")]
        [ProducesResponseType(typeof(List<AlgoMetaDataModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAlgoMetadata()
        {
            var clientId = User.GetClientId();

            var result = await _clientDataService.GetClientMetadataAsync(clientId);

            if (result == null || result.AlgoMetaData.IsNullOrEmptyCollection())
                return NotFound();

            var response = Mapper.Map<List<AlgoMetaDataModel>>(result.AlgoMetaData);

            return Ok(response);
        }

        [HttpGet("algoMetadata")]
        [SwaggerOperation("GetAlgoMetadata")]
        [ProducesResponseType(typeof(AlgoClientMetaDataInformationModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAlgoMetadata(string clientId, string algoId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                clientId = User.GetClientId();

            var result = await _clientDataService.GetAlgoMetaDataInformationAsync(clientId, algoId);

            if (result == null)
                return NotFound();

            var response = Mapper.Map<AlgoClientMetaDataInformationModel>(result);

            return Ok(response);
        }

        [HttpPost("metadata")]
        [SwaggerOperation("SaveAlgoMetadata")]
        [ProducesResponseType(typeof(AlgoMetaDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SaveAlgoMetadata([FromBody]AlgoMetaDataModel model)
        {
            var clientId = User.GetClientId();
            var clientName = User.Identity.Name;

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

        [HttpGet("instanceData/all")]
        [SwaggerOperation("GetAllAlgoInstanceDataAsync")]
        [ProducesResponseType(typeof(List<AlgoClientInstanceModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllAlgoInstanceDataAsync(string algoId)
        {
            var data = new CSharp.AlgoTemplate.Models.Models.BaseAlgoData
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

            data.AssetPair = model.AlgoMetaDataInformation.Parameters.SingleOrDefault(t => t.Key == "AssetPair")?.Value;
            data.Volume = Convert.ToDouble(model.AlgoMetaDataInformation.Parameters.SingleOrDefault(t => t.Key == "Volume")?.Value);
            data.TradedAsset = model.AlgoMetaDataInformation.Parameters.SingleOrDefault(t => t.Key == "TradedAsset")?.Value;

            var result = await _clientDataService.SaveAlgoInstanceDataAsync(data, model.AlgoClientId);
            var response = Mapper.Map<AlgoClientInstanceModel>(result);

            //we could extract it into modles in NuGet Algo Template project
            response.AlgoClientId = model.AlgoClientId;

            return Ok(response);
        }
    }
}
