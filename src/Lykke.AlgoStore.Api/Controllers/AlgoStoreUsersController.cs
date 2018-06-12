using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.AlgoStore.Service.Security.Client;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [RequirePermissionAttribute]
    [Route("api/v1/users")]
    public class AlgoStoreUsersController: Controller
    {
        private readonly ISecurityClient _securityClient;

        public AlgoStoreUsersController(ISecurityClient securityClient)
        {
            _securityClient = securityClient;
        }

        [HttpGet("getAllWithRoles")]
        [SwaggerOperation("GetAllUserRoles")]
        [ProducesResponseType(typeof(List<AlgoStoreUserDataModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {
            var result = await _securityClient.GetAllUsersWithRolesAsync();

            return Ok(result);
        }

        [HttpGet("getByIdWithRoles")]
        [SwaggerOperation("GetUserByIdWithRoles")]
        [ProducesResponseType(typeof(AlgoStoreUserDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserByIdWithRoles(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                clientId = User.GetClientId();

            var result = await _securityClient.GetUserByIdWithRolesAsync(clientId);

            return Ok(result);
        }
    }
}
