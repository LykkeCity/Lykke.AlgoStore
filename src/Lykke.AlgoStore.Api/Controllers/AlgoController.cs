using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [RequirePermission]
    [Route("api/v1/algo")]
    public class AlgoController : Controller
    {
        private readonly IAlgoStoreClientDataService _clientDataService;

        public AlgoController(IAlgoStoreClientDataService clientDataService)
        {
            _clientDataService = clientDataService;
        }

        //REMARK: This endpoint is a merge result between 'metadata - POST' and 'imageData/upload/string - POST' endpoints
        //In future, when we do a refactoring we should remove unused endpoints from code
        [HttpPost("create")]
        [SwaggerOperation("CreateAlgo")]
        [ProducesResponseType(typeof(AlgoDataModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int) HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAlgo([FromBody] CreateAlgoModel model)
        {
            var clientId = User.GetClientId();
            var data = Mapper.Map<AlgoData>(model);

            var result = await _clientDataService.CreateAlgoAsync(clientId, model.Author, data, model.DecodedContent);

            var response = Mapper.Map<AlgoDataModel>(result);
            //response.Author = result.Author; //REMARK: Should refactor things like this in future and use AutoMapper for everything

            return Ok(response);
        }
    }
}
