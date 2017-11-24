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
    [Route("api/[controller]")]
    public class AlgoClientDataController : Controller
    {
        private readonly ILog _log;
        private readonly IAlgoStoreClientDataService _clientDataService;

        public AlgoClientDataController(ILog log, IAlgoStoreClientDataService clientDataService)
        {
            _log = log;
            _clientDataService = clientDataService;
        }

        [HttpGet]
        [SwaggerOperation("GetAlgoMetadata")]
        [ProducesResponseType(typeof(AlgoMetaDataResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAlgoMetadata()
        {
            var result = await _clientDataService.GetClientMetadata(User.GetClientId());

            if (result.HasError)
                return result.ResultError.ToHttpStatusCode();

            var data = result.Data;

            var response = new AlgoMetaDataResponse { AlgoMetaData = new List<AlgoMetaDataModel>() };

            if (data != null && !data.AlgoMetaData.IsNullOrEmptyCollection())
            {
                foreach (var metadata in data.AlgoMetaData)
                {
                    response.AlgoMetaData.Add(Mapper.Map<AlgoMetaDataModel>(metadata));
                }
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAlgoMetadata([FromBody]AlgoMetaDataModel model)
        {
            var data = Mapper.Map<AlgoMetaData>(model);

            var result = await _clientDataService.SaveClientMetadata(User.GetClientId(), data);

            if (result.HasError)
                return result.ResultError.ToHttpStatusCode();

            var response = new AlgoMetaDataResponse { AlgoMetaData = new List<AlgoMetaDataModel> { Mapper.Map<AlgoMetaDataModel>(result.Data) } };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAlgoMetadata([FromBody]AlgoMetaDataModel model)
        {
            var data = Mapper.Map<AlgoMetaData>(model);

            var result = await _clientDataService.DeleteClientMetadata(User.GetClientId(), data);

            if (result.HasError)
                return result.ResultError.ToHttpStatusCode();

            return Ok();
        }
    }
}
