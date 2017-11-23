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
        [ProducesResponseType(typeof(AlgoClientMetaDataResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAlgoMetadata()
        {
            var data = await _clientDataService.GetClientMetadata(User.GetClientId());

            var response = new AlgoClientMetaDataResponse { AlgoMetaData = new List<AlgoMetaDataModel>() };

            if (data != null && !data.AlgosData.IsNullOrEmptyCollection())
            {
                foreach (var metadata in data.AlgosData)
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

            var clientData = await _clientDataService.SaveClientMetadata(User.GetClientId(), data);
            var response = new AlgoClientMetaDataResponse { AlgoMetaData = new List<AlgoMetaDataModel> { Mapper.Map<AlgoMetaDataModel>(clientData) } };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAlgoMetadata([FromBody]AlgoMetaDataModel model)
        {
            var data = Mapper.Map<AlgoMetaData>(model);

            await _clientDataService.DeleteClientMetadata(User.GetClientId(), data);

            return Ok();
        }
    }
}
