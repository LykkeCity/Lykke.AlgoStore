using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.Service.Security.Client;
using Lykke.AlgoStore.Core.Services;
using Lykke.AlgoStore.Job.GDPR.Client;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [Route("api/v1/users")]
    public class AlgoStoreUsersController : Controller
    {
        private readonly ISecurityClient _securityClient;
        private readonly IGdprClient _gdprClient;

        public AlgoStoreUsersController(ISecurityClient securityClient, IGdprClient gdprClient)
        {
            _securityClient = securityClient;
            _gdprClient = gdprClient;
        }

        [HttpPost("verifyUser")]
        [SwaggerOperation("VerifyUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> VerifyUser(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                clientId = User.GetClientId();

            await _securityClient.VerifyUserRoleAsync(clientId);

            await _gdprClient.SeedConsentAsync(clientId);

            return Ok();
        }

        [HttpGet("getAllWithRoles")]
        [SwaggerOperation("GetAllUserRoles")]
        [RequirePermission]
        [DescriptionAttribute("Allows users to see all available users and their roles")]
        [ProducesResponseType(typeof(List<AlgoStoreUserDataModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {
            var result = await _securityClient.GetAllUsersWithRolesAsync();

            return Ok(Mapper.Map<List<AlgoStoreUserDataModel>>(result));
        }

        [HttpGet("getByIdWithRoles")]
        [SwaggerOperation("GetUserByIdWithRoles")]
        [RequirePermission]
        [DescriptionAttribute("Allows users to see a specific user and his roles")]
        [ProducesResponseType(typeof(AlgoStoreUserDataModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserByIdWithRoles(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                clientId = User.GetClientId();

            var result = await _securityClient.GetUserByIdWithRolesAsync(clientId);

            return Ok(Mapper.Map<AlgoStoreUserDataModel>(result));
        }
    }
}
