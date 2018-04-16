using Lykke.AlgoStore.Api.Infrastructure.Attributes;
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
    [RequirePermissionAttribute]
    [Route("api/v1/permissions")]
    public class AlgoStoreUserPermissionsController: Controller
    {
        private readonly IUserRolesService _userRolesService;
        private readonly IUserPermissionsService _permissionsService;

        public AlgoStoreUserPermissionsController(
            IUserRolesService userRolesService,
            IUserPermissionsService permissionsService)
        {
            _userRolesService = userRolesService;
            _permissionsService = permissionsService;
        }

        [HttpGet("getAll")]
        [SwaggerOperation("GetAllPermissions")]
        [ProducesResponseType(typeof(List<UserPermissionModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllPermissions()
        {
            var result = await _permissionsService.GetAllPermissionsAsync();

            return Ok(result);
        }

        [HttpGet("getById")]
        [SwaggerOperation("GetPermissionById")]
        [ProducesResponseType(typeof(UserPermissionModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPermissionById(string permissionId)
        {
            var result = await _permissionsService.GetPermissionByIdAsync(permissionId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("getByRoleId")]
        [SwaggerOperation("GetPermissionsByRoleId")]
        [ProducesResponseType(typeof(List<UserPermissionModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPermissionsByRoleId(string roleId)
        {          
            var result = await _permissionsService.GetPermissionsByRoleIdAsync(roleId);

            return Ok(result);
        }

        [HttpPost("savePermission")]
        [SwaggerOperation("SavePermission")]
        [ProducesResponseType(typeof(UserPermissionModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SavePermission([FromBody] UserPermissionModel permission)
        {
            var data = AutoMapper.Mapper.Map<UserPermissionData>(permission);

            var result = await _permissionsService.SavePermissionAsync(data);

            return Ok(result);
        }

        [HttpPost("assignPermission")]
        [SwaggerOperation("AssignPermissionToRole")]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AssignPermissionToRole([FromBody] RolePermissionMatchModel role)
        {
            var data = AutoMapper.Mapper.Map<RolePermissionMatchData>(role);

            await _permissionsService.AssignPermissionToRoleAsync(data);

            return NoContent();
        }

        [HttpPost("assignPermissions")]
        [SwaggerOperation("AssignMultiplePermissionToRole")]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AssignMultiplePermissionToRole([FromBody] List<RolePermissionMatchModel> permissions)
        {
            var data = AutoMapper.Mapper.Map<List<RolePermissionMatchData>>(permissions);

            await _permissionsService.AssignPermissionsToRoleAsync(data);

            return NoContent();
        }

        [HttpPost("revokePermission")]
        [SwaggerOperation("RevokePermissionFromRole")]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RevokePermissionFromRole([FromBody] RolePermissionMatchModel role)
        {
            var data = AutoMapper.Mapper.Map<RolePermissionMatchData>(role);

            await _permissionsService.RevokePermissionFromRole(data);

            return NoContent();
        }

        [HttpPost("revokePermissions")]
        [SwaggerOperation("RevokeMultiplePermissions")]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RevokeMultiplePermissions([FromBody] List<RolePermissionMatchModel> role)
        {
            var data = AutoMapper.Mapper.Map<List<RolePermissionMatchData>>(role);

            await _permissionsService.RevokePermissionsFromRole(data);

            return NoContent();
        }

        [HttpDelete("deletePermission")]
        [SwaggerOperation("DeletePermission")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeletePermission(string permissionId)
        {
            await _permissionsService.DeletePermissionAsync(permissionId);

            return NoContent();
        }

    }
}
