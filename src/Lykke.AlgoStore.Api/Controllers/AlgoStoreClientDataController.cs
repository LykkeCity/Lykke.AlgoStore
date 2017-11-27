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

namespace Lykke.AlgoStore.Controllers
{
    [Authorize]
    [Route("api/clientData")]
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
        [ProducesResponseType(typeof(AlgoMetaDataResponse<List<AlgoMetaDataModel>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAlgoMetadata()
        {
            var result = await _clientDataService.GetClientMetadata(User.GetClientId());

            if (result.HasError)
                return result.ResultError.ToHttpStatusCode();

            var data = result.Data;

            var response = new AlgoMetaDataResponse<List<AlgoMetaDataModel>> { Data = new List<AlgoMetaDataModel>() };

            if (data != null && !data.AlgoMetaData.IsNullOrEmptyCollection())
            {
                foreach (var metadata in data.AlgoMetaData)
                {
                    response.Data.Add(Mapper.Map<AlgoMetaDataModel>(metadata));
                }
            }

            return Ok(response);
        }

        [HttpPost("/algoMetadata")]
        [SwaggerOperation("SaveAlgoMetadata")]
        [ProducesResponseType(typeof(AlgoMetaDataResponse<List<AlgoMetaDataModel>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveAlgoMetadata([FromBody]AlgoMetaDataModel model)
        {
            var data = Mapper.Map<AlgoMetaData>(model);

            var result = await _clientDataService.SaveClientMetadata(User.GetClientId(), data);

            if (result.HasError)
                return result.ResultError.ToHttpStatusCode();

            var response = new AlgoMetaDataResponse<List<AlgoMetaDataModel>> { Data = new List<AlgoMetaDataModel> { Mapper.Map<AlgoMetaDataModel>(result.Data) } };

            return Ok(response);
        }

        [HttpPost("/algoMetadata/cascadeDelete")]
        [SwaggerOperation("DeleteAlgoMetadata")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CascadeDeleteAlgoMetadata([FromBody]AlgoMetaDataModel model)
        {
            var data = Mapper.Map<AlgoMetaData>(model);

            var result = await _clientDataService.CascadeDeleteClientMetadata(User.GetClientId(), data);

            if (result.HasError)
                return result.ResultError.ToHttpStatusCode();

            return Ok();
        }

        [HttpGet("/template")]
        [SwaggerOperation("GetTemplates")]
        [ProducesResponseType(typeof(AlgoMetaDataResponse<List<AlgoTemplateModel>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTemplates(string languageId)
        {
            var result = await _clientDataService.GetTemplate(languageId);

            if (result.HasError)
                return result.ResultError.ToHttpStatusCode();

            var data = result.Data;

            var response = new AlgoMetaDataResponse<List<AlgoTemplateModel>> { Data = new List<AlgoTemplateModel>() };

            if (data != null && !data.IsNullOrEmptyCollection())
            {
                foreach (var template in data)
                {
                    response.Data.Add(Mapper.Map<AlgoTemplateModel>(template));
                }
            }

            return Ok(response);
        }

        [HttpGet("/source")]
        [SwaggerOperation("GetSource")]
        [ProducesResponseType(typeof(AlgoMetaDataResponse<AlgoDataModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetSource(string clientAlgoId)
        {
            var result = await _clientDataService.GetSource(clientAlgoId);

            if (result.HasError)
                return result.ResultError.ToHttpStatusCode();

            var response = new AlgoMetaDataResponse<AlgoDataModel> { Data = Mapper.Map<AlgoDataModel>(result.Data) };

            return Ok(response);
        }

        [HttpPost("/source")]
        [SwaggerOperation("SaveSource")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SaveSource(AlgoDataModel model)
        {
            var data = Mapper.Map<AlgoData>(model);
            var result = await _clientDataService.SaveSource(data);

            if (result.HasError)
                return result.ResultError.ToHttpStatusCode();

            return Ok();
        }

        [HttpGet("/runtimeData")]
        [SwaggerOperation("GetRuntimeData")]
        //[ProducesResponseType(typeof(ErrResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(AlgoMetaDataResponse<List<AlgoRuntimeDataModel>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRuntimeData(string clientAlgoId)
        {
            var result = await _clientDataService.GetRuntimeData(clientAlgoId);

            if (result.HasError)
                return result.ResultError.ToHttpStatusCode();

            var response = new AlgoMetaDataResponse<List<AlgoRuntimeDataModel>> { Data = Mapper.Map< List < AlgoRuntimeData > , List <AlgoRuntimeDataModel>>(result.Data.RuntimeData) };

            return Ok(response);

            // return  BadRequest(new ErrResponse())
        }
    }
}
