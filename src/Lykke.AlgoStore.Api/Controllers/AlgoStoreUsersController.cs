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
        private readonly IAlgoInstancesService _instancesService;

        public AlgoStoreUsersController(ISecurityClient securityClient, IAlgoInstancesService instancesService)
        {
            _securityClient = securityClient;
            _instancesService = instancesService;
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

        [HttpGet("me/instances")]
        [SwaggerOperation("GetInstancesForUser")]
        [DescriptionAttribute("Allows users to see a list of all of his instances")]
        [ProducesResponseType(typeof(List<UserInstanceModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInstancesForUser()
        {
            string clientId = User.GetClientId();

            var result = await _instancesService.GetUserInstancesAsync(clientId);

            return Ok(Mapper.Map<List<UserInstanceModel>>(result));
        }
    }
}
