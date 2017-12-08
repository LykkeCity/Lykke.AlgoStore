using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Api.Infrastructure;
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
        private readonly ILog _log;
        private readonly IAlgoStoreClientDataService _clientDataService;

        public AlgoClientDataController(ILog log, IAlgoStoreClientDataService clientDataService)
        {
            _log = log;
            _clientDataService = clientDataService;
        }

        [HttpGet("metadata")]
        [SwaggerOperation("GetAlgoMetadata")]
        [ProducesResponseType(typeof(List<AlgoMetaDataModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAlgoMetadata()
        {
            var result = await _clientDataService.GetClientMetadata(User.GetClientId());
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
            var data = Mapper.Map<AlgoMetaData>(model);

            var result = await _clientDataService.SaveClientMetadata(User.GetClientId(), data);

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
            var data = Mapper.Map<AlgoMetaData>(model);

            await _clientDataService.CascadeDeleteClientMetadata(User.GetClientId(), data);

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
            var data = Mapper.Map<UploadAlgoBinaryData>(model);

            await _clientDataService.SaveAlgoAsBinary(data);

            return NoContent();
        }
    }
}
