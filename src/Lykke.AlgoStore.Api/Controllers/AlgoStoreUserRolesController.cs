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
using Lykke.Service.Security.Client.AutorestClient.Models;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [Route("api/v1/roles")]
    public class AlgoStoreUserRolesController: Controller
    {
        private readonly ISecurityClient _securityClient;

        public AlgoStoreUserRolesController(ISecurityClient securityClient)
        {
            _securityClient = securityClient;
        }

        [HttpGet("getAll")]
        [RequirePermissionAttribute]
        [SwaggerOperation("GetAllUserRoles")]
        [ProducesResponseType(typeof(List<UserRoleModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllUserRoles()
        {
            var result = await _securityClient.GetAllUserRolesAsync();
            return Ok(result);
        }

        [HttpGet("getById")]
        [RequirePermissionAttribute]
        [SwaggerOperation("GetRoleById")]
        [ProducesResponseType(typeof(UserRoleModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            var result = await _securityClient.GetRoleByIdAsync(roleId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("getByClientId")]
        [SwaggerOperation("GetRolesByClientId")]
        [ProducesResponseType(typeof(UserRoleModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRolesByClientId(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                clientId = User.GetClientId();

            var result = await _securityClient.GetRolesByClientIdAsync(clientId);

            return Ok(result);
        }

        [HttpPost("saveRole")]
        [RequirePermissionAttribute]
        [SwaggerOperation("SaveUserRole")]
        [ProducesResponseType(typeof(UserRoleModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveUserRole([FromBody] UserRoleCreateModel role)
        {
            var data = AutoMapper.Mapper.Map<Lykke.Service.Security.Client.AutorestClient.Models.UserRoleModel>(role);

            var result = await _securityClient.SaveUserRoleAsync(data);

            return Ok(result);
        }

        [HttpPost("updateRole")]
        [RequirePermissionAttribute]
        [SwaggerOperation("SaveUserRole")]
        [ProducesResponseType(typeof(UserRoleModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserRoleUpdateModel role)
        {
            var data = AutoMapper.Mapper.Map<Lykke.Service.Security.Client.AutorestClient.Models.UserRoleModel>(role);

            var result = await _securityClient.SaveUserRoleAsync(data);

            return Ok(result);
        }

        [HttpPost("assignRole")]
        [RequirePermissionAttribute]
        [SwaggerOperation("AssignUserRole")]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AssignUserRole([FromBody] UserRoleMatchModel role)
        {
            var data = AutoMapper.Mapper.Map<Lykke.Service.Security.Client.AutorestClient.Models.UserRoleMatchModel>(role);

            await _securityClient.AssignUserRoleAsync(data);

            return NoContent();
        }

        [HttpPost("revokeRole")]
        [RequirePermissionAttribute]
        [SwaggerOperation("RevokeRoleFromUser")]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RevokeRoleFromUser([FromBody] UserRoleMatchModel role)
        {
            var data = AutoMapper.Mapper.Map<Lykke.Service.Security.Client.AutorestClient.Models.UserRoleMatchModel>(role);

            await _securityClient.RevokeRoleFromUserAsync(data);

            return NoContent();
        }

        [HttpGet("verifyRole")]
        [SwaggerOperation("VerifyUserRole")]
        [ProducesResponseType(typeof(UserRoleModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> VerifyUserRole(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                clientId = User.GetClientId();

            await _securityClient.VerifyUserRoleAsync(clientId);

            return Ok();
        }

        [HttpDelete("deleteRole")]
        [RequirePermissionAttribute]
        [SwaggerOperation("DeleteUserRole")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteUserRole(string roleId)
        {
            await _securityClient.DeleteUserRoleAsync(roleId);

            return NoContent();
        }
    }
}
