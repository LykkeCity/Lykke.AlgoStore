using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Infrastructure.Extensions;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
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
    [Route("api/v1/roles")]
    public class AlgoStoreUserRolesController: Controller
    {
        private readonly IUserRolesService _userRolesService;

        public AlgoStoreUserRolesController(
            IUserRolesService userRolesService)
        {
            _userRolesService = userRolesService;
        }

        [HttpGet("getAll")]
        [RequirePermissionAttribute]
        [SwaggerOperation("GetAllUserRoles")]
        [ProducesResponseType(typeof(List<UserRoleModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllUserRoles()
        {
            var result = await _userRolesService.GetAllRolesAsync();

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
            var result = await _userRolesService.GetRoleByIdAsync(roleId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("getByClientId")]
        [SwaggerOperation("GetRolesByClientId")]
        [ProducesResponseType(typeof(UserRoleModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetRolesByClientId(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                clientId = User.GetClientId();

            var result = await _userRolesService.GetRolesByClientIdAsync(clientId);

            return Ok(result);
        }

        [HttpPost("saveRole")]
        [RequirePermissionAttribute]
        [SwaggerOperation("SaveUserRole")]
        [ProducesResponseType(typeof(UserRoleModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SaveUserRole([FromBody] UserRoleModel role)
        {
            var data = AutoMapper.Mapper.Map<UserRoleData>(role);

            var result = await _userRolesService.SaveRoleAsync(data);

            return Ok(result);
        }

        [HttpPost("assignRole")]
        [RequirePermissionAttribute]
        [SwaggerOperation("AssignUserRole")]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AssignUserRole([FromBody] UserRoleMatchModel role)
        {
            var data = AutoMapper.Mapper.Map<UserRoleMatchData>(role);

            await _userRolesService.AssignRoleToUser(data);

            return NoContent();
        }

        [HttpPost("revokeRole")]
        [RequirePermissionAttribute]
        [SwaggerOperation("RevokeRoleFromUser")]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RevokeRoleFromUser([FromBody] UserRoleMatchModel role)
        {
            var data = AutoMapper.Mapper.Map<UserRoleMatchData>(role);

            await _userRolesService.RevokeRoleFromUser(data);

            return NoContent();
        }

        [HttpGet("verifyRole")]
        [SwaggerOperation("VerifyUserRole")]
        [ProducesResponseType(typeof(UserRoleModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> VerifyUserRole(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
                clientId = User.GetClientId();

            await _userRolesService.VerifyUserRole(clientId);

            return Ok();
        }

        [HttpDelete("deleteRole")]
        [RequirePermissionAttribute]
        [SwaggerOperation("DeleteUserRole")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteUserRole(string roleId)
        {
            await _userRolesService.DeleteRoleAsync(roleId);

            return NoContent();
        }
    }
}
