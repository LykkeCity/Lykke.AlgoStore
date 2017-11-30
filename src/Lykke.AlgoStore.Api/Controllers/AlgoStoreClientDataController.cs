using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Lykke.AlgoStore.Controllers
{
    [Authorize]
    [Route("api/v001/clientData")]
    public class AlgoClientDataController : Controller
    {
        private readonly ILog _log;
        private readonly IAlgoStoreClientDataService _clientDataService;

        public AlgoClientDataController(ILog log, IAlgoStoreClientDataService clientDataService)
        {
            _log = log;
            _clientDataService = clientDataService;
        }

        [HttpGet("/algoMetadata")]
        [SwaggerOperation("GetAlgoMetadata")]
        [ProducesResponseType(typeof(List<AlgoMetaDataModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAlgoMetadata()
        {
            var result = await _clientDataService.GetClientMetadata(User.GetClientId());
            if (result == null || result.AlgoMetaData.IsNullOrEmptyCollection())
                return NotFound();

            var response = Mapper.Map<List<AlgoMetaDataModel>>(result.AlgoMetaData);

            return Ok(response);
        }

        [HttpPost("/algoMetadata")]
        [SwaggerOperation("SaveAlgoMetadata")]
        [ProducesResponseType(typeof(AlgoMetaDataModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveAlgoMetadata([FromBody]AlgoMetaDataModel model)
        {
            var data = Mapper.Map<AlgoMetaData>(model);

            var result = await _clientDataService.SaveClientMetadata(User.GetClientId(), data);

            var response = Mapper.Map<AlgoMetaDataModel>(result);

            return Ok(response);
        }

        [HttpPost("/algoMetadata/cascadeDelete")]
        [SwaggerOperation("DeleteAlgoMetadata")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAlgoMetadata([FromBody]AlgoMetaDataModel model)
        {
            var data = Mapper.Map<AlgoMetaData>(model);

            await _clientDataService.CascadeDeleteClientMetadata(User.GetClientId(), data);

            return NoContent();
        }

        [HttpPost("/algo/upload/binary")]
        [SwaggerOperation("UploadBinaryFile")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UploadBinaryFile(UploadAlgoBinaryModel model )
        {
            var data = Mapper.Map<UploadAlgoBinaryData>(model);

            await _clientDataService.SaveAlgoAsBinary(data);

            return Ok();
        }
    }
}
