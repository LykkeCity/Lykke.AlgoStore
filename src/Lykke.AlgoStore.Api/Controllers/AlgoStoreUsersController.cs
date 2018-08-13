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

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [RequirePermission]
    [Route("api/v1/users")]
    public class AlgoStoreUsersController: Controller
    {
        private readonly ISecurityClient _securityClient;
        private readonly IUsersService _usersService;

        public AlgoStoreUsersController(ISecurityClient securityClient, IUsersService usersService)
        {
            _securityClient = securityClient;
            _usersService = usersService;
        }

        [HttpGet("getAllWithRoles")]
        [SwaggerOperation("GetAllUserRoles")]
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

        [HttpPost("gdprConsent")]
        [SwaggerOperation("SetUserGDPRConsent")]
        [DescriptionAttribute("Allows users to agree with the GDPR requirements")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SetUserGDPRConsent()
        {            
            var clientId = User.GetClientId();

            await _usersService.SetGDPRConsentAsync(clientId);

            return NoContent();
        }

        [HttpPost("cookieConsent")]
        [SwaggerOperation("SetUserCookieConsent")]
        [DescriptionAttribute("Allows users to agree with the Cookie requirements")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SetUserCookieConsent()
        {
            var clientId = User.GetClientId();

            await _usersService.SetCookieConsentAsync(clientId);

            return NoContent();
        }

        [HttpPost("deactivateAccount")]
        [SwaggerOperation("DeactivateUserAccount")]
        [DescriptionAttribute("Allows users to deactivate his account")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeactivateUserAccount()
        {
            var clientId = User.GetClientId();

            await _usersService.DeactivateAccountAsync(clientId);

            return NoContent();
        }
    }
}
