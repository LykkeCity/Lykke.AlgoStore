using AutoMapper;
using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Services;
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
    [Route("api/v1/clients")]
    public class AlgoStoreClientsController : Controller
    {
        private readonly IAlgoStoreClientsService _algoStoreClientsService;

        public AlgoStoreClientsController(IAlgoStoreClientsService algoStoreClientsService)
        {
            _algoStoreClientsService = algoStoreClientsService;
        }

        [HttpGet("wallets")]
        [SwaggerOperation("GetAvailableClientWallets")]
        [ProducesResponseType(typeof(List<ClientWalletDataModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAvailableClientWallets()
        {
            var clientId = User.GetClientId();

            var wallets = await _algoStoreClientsService.GetAvailableClientWalletsAsync(clientId);

            var response = Mapper.Map<List<ClientWalletDataModel>>(wallets);

            return Ok(response);
        }
    }
}
