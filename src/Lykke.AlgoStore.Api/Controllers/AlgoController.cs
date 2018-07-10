using AutoMapper;
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
        private readonly IAlgosService _algosService;

        public AlgoController(IAlgosService clientDataService)
        {
            _algosService = clientDataService;
        }

        //REMARK: This endpoint is a merge result between 'metadata - POST' and 'imageData/upload/string - POST' endpoints
        //In future, when we do a refactoring we should remove unused endpoints from code
        [HttpPost("create")]
        [SwaggerOperation("CreateAlgo")]
        [DescriptionAttribute("Allows users to create a new algo")]
        [ProducesResponseType(typeof(AlgoDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAlgo([FromBody] CreateAlgoModel model)
        {
            var data = Mapper.Map<AlgoData>(model);
            data.ClientId = User.GetClientId();

            var result = await _algosService.CreateAlgoAsync(data, model.DecodedContent);

            var response = Mapper.Map<AlgoDataModel>(result);

            return Ok(response);
        }

        [HttpPost("edit")]
        [SwaggerOperation("EditAlgo")]
        [DescriptionAttribute("Allows users to edit an existing algo")]
        [ProducesResponseType(typeof(AlgoDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> EditAlgo([FromBody] CreateAlgoModel model)
        {
            var data = Mapper.Map<AlgoData>(model);
            data.ClientId = User.GetClientId();

            var result = await _algosService.EditAlgoAsync(data, model.DecodedContent);

            var response = Mapper.Map<AlgoDataModel>(result);

            return Ok(response);
        }

        [HttpDelete("delete")]
        [SwaggerOperation("DeleteAlgo")]
        [DescriptionAttribute("Allows users to delete an existing algo if it's not public and doesn't have any running instances")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteAlgo([FromBody] DeleteAlgoModel model)
        {
            var clientId = User.GetClientId();

            await _algosService.DeleteAlgoAsync(model.AlgoClientId, model.AlgoId, model.ForceDelete, clientId);

            return Ok();
        }

        [HttpGet("getAllAlgos")]
        [SwaggerOperation("GetAllAlgos")]
        [DescriptionAttribute("Gives users the ability to see all available public algos")]
        [ProducesResponseType(typeof(List<AlgoRatingMetaDataModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllAlgos()
        {
            var result = await _algosService.GetAllAlgosWithRatingAsync();

            if (result == null || result.IsNullOrEmptyCollection())
                return NotFound();

            var response = Mapper.Map<List<AlgoRatingMetaDataModel>>(result);

            return Ok(response);
        }

        [HttpGet("getAllUserAlgos")]
        [SwaggerOperation("GetAllUserAlgos")]
        [DescriptionAttribute("Gives users the ability to see their personal algos")]
        [ProducesResponseType(typeof(List<AlgoDataModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllUserAlgos()
        {
            var clientId = User.GetClientId();
            var result = await _algosService.GetAllUserAlgosAsync(clientId);

            var response = Mapper.Map<List<AlgoDataModel>>(result);

            return Ok(response);
        }

        [HttpPost("algoRating")]
        [SwaggerOperation("algoRating")]
        [DescriptionAttribute("Allows users to set rating to a specific algo")]
        [ProducesResponseType(typeof(AlgoRatingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RateAlgo([FromBody] AlgoRatingModel model)
        {
            var data = Mapper.Map<AlgoRatingData>(model);

            data.ClientId = User.GetClientId();

            var result = await _algosService.SaveAlgoRatingAsync(data);

            var response = Mapper.Map<AlgoRatingModel>(result);

            return Ok(response);
        }

        [HttpGet("userAlgoRating")]
        [SwaggerOperation("userAlgoRating")]
        [DescriptionAttribute("Allows users see the rating that a they provided for an algo")]
        [ProducesResponseType(typeof(AlgoRatingModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUserAlgoRating(string algoId, string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                clientId = User.GetClientId();

            var result = await _algosService.GetAlgoRatingForClientAsync(algoId, clientId);

            if (result == null)
                return NotFound();

            var response = Mapper.Map<AlgoRatingModel>(result);

            return Ok(response);
        }

        [HttpPost("addToPublic")]
        [SwaggerOperation("addToPublic")]
        [DescriptionAttribute("Allows users to change the Algo status to Public and make it visible in the Public Algos page")]
        [ProducesResponseType(typeof(PublicAlgoDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddToPublic([FromBody] PublicAlgoDataModel model)
        {
            var clientId = User.GetClientId();
            var data = Mapper.Map<PublicAlgoData>(model);

            var result = await _algosService.AddToPublicAsync(data, clientId);

            var response = Mapper.Map<PublicAlgoDataModel>(result);

            return Ok(response);

        }

        [HttpPost("removeFromPublic")]
        [SwaggerOperation("removeFromPublic")]
        [DescriptionAttribute("Allows users to unpublish the algo if it doesn't have any instances. It will be visible only for the creator")]
        [ProducesResponseType(typeof(PublicAlgoDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemoveFromPublic([FromBody] PublicAlgoDataModel model)
        {
            var clientId = User.GetClientId();
            var data = Mapper.Map<PublicAlgoData>(model);

            var result = await _algosService.RemoveFromPublicAsync(data, clientId);

            var response = Mapper.Map<PublicAlgoDataModel>(result);

            return Ok(response);

        }

        [HttpGet("getAlgoInformation")]
        [SwaggerOperation("GetAlgoInformation")]
        [DescriptionAttribute("Gives users the ability to see all available Algo information")]
        [ProducesResponseType(typeof(AlgoDataInformationModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAlgoInformation(string clientId, string algoId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                clientId = User.GetClientId();

            var userClientId = User.GetClientId();

            var result = await _algosService.GetAlgoDataInformationAsync(userClientId, clientId, algoId);

            if (result == null)
                return NotFound();

            var response = Mapper.Map<AlgoDataInformationModel>(result);

            return Ok(response);
        }

        [HttpPost("sourceCode/upload/binary")]
        [SwaggerOperation("UploadBinaryFile")]
        [DescriptionAttribute("Allows users to upload binary code for an algo")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        [ServiceFilter(typeof(ValidateMimeMultipartContentFilter))]
        public async Task<IActionResult> UploadBinaryFile(UploadAlgoBinaryModel model)
        {
            var clientId = User.GetClientId();

            var data = Mapper.Map<UploadAlgoBinaryData>(model);

            await _algosService.SaveAlgoAsBinaryAsync(clientId, data);

            return NoContent();
        }

        [HttpPost("sourceCode/upload/string")]
        [SwaggerOperation("UploadString")]
        [DescriptionAttribute("Allows users to upload source code for an algo")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UploadSting([FromBody]UploadAlgoStringModel model)
        {
            var clientId = User.GetClientId();

            var data = Mapper.Map<UploadAlgoStringData>(model);

            await _algosService.SaveAlgoAsStringAsync(clientId, data);

            return NoContent();
        }

        [HttpGet("sourceCode/getString")]
        [SwaggerOperation("GetUploadString")]
        [DescriptionAttribute("Gives users the ability to see the source code of the algo")]
        [ProducesResponseType(typeof(ContentStringModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetUploadString(string clientId, string algoId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                clientId = User.GetClientId();

            var content = await _algosService.GetAlgoAsStringAsync(clientId, algoId);

            return Ok(new ContentStringModel
            {
                Content = content
            });
        }

        [HttpGet("getAssetsForAssetPair")]
        [SwaggerOperation("GetAssetsForAssetPair")]
        [DescriptionAttribute("By given asset pair, returns the assets involved. Required for running an algo")]
        [ProducesResponseType(typeof(List<EnumValue>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAssetsForAssetPair(string assetPairId)
        {
            var clientId = User.GetClientId();
            var result = await _algosService.GetAssetsForAssetPairAsync(assetPairId, clientId);

            if (result.IsNullOrEmptyCollection())
                return NotFound();

            return Ok(result);
        }
    }
}
