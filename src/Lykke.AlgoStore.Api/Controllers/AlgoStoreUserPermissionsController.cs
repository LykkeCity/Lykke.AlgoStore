using Lykke.AlgoStore.Api.Infrastructure.ContentFilters;
using Lykke.AlgoStore.Api.Models;
using Lykke.AlgoStore.Core.Domain.Entities;
using Lykke.AlgoStore.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
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
        [RequiredPermissionFilter(nameof(GetAllpermissions))]
        [SwaggerOperation("GetAllpermissions")]
        [ProducesResponseType(typeof(List<UserPermissionModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAllpermissions()
        {
            var result = await _permissionsService.GetAllPermissionsAsync();

            return Ok(result);
        }

        [HttpGet("getById")]
        [RequiredPermissionFilter(nameof(GetPermissionById))]
        [SwaggerOperation("GetPermissionById")]
        [ProducesResponseType(typeof(UserPermissionModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPermissionById(string permissionId)
        {
            var result = await _permissionsService.GetPermissionByIdAsync(permissionId);

            return Ok(result);
        }

        [HttpGet("getByRoleId")]
        [RequiredPermissionFilter(nameof(GetPermissionsByRoleId))]
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
        [RequiredPermissionFilter(nameof(SavePermission))]
        [SwaggerOperation("SavePermission")]
        [ProducesResponseType(typeof(UserPermissionModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> SavePermission([FromBody] UserPermissionModel role)
        {
            var data = AutoMapper.Mapper.Map<UserPermissionData>(role);

            var result = await _permissionsService.SavePermissionAsync(data);

            return Ok(result);
        }

        [HttpPost("assignPermission")]
        [RequiredPermissionFilter(nameof(AssignPermissionToRole))]
        [SwaggerOperation("AssignPermissionToRole")]
        [ProducesResponseType(typeof(UserPermissionModel), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AssignPermissionToRole([FromBody] RolePermissionMatchModel role)
        {
            var data = AutoMapper.Mapper.Map<RolePermissionMatchData>(role);

            await _permissionsService.AssignPermissionToRoleAsync(data);

            return NoContent();
        }

        [HttpPost("revokePermission")]
        [RequiredPermissionFilter(nameof(RevokePermissionFromRole))]
        [SwaggerOperation("RevokePermissionFromRole")]
        [ProducesResponseType(typeof(UserPermissionModel), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RevokePermissionFromRole([FromBody] RolePermissionMatchModel role)
        {
            var data = AutoMapper.Mapper.Map<RolePermissionMatchData>(role);

            await _permissionsService.RevokePermissionFromRole(data);

            return NoContent();
        } 

        [HttpDelete("deletePermission")]
        [RequiredPermissionFilter(nameof(DeletePermission))]
        [SwaggerOperation("DeletePermission")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeletePermission(string permissionId)
        {
            await _permissionsService.DeletePermissionAsync(permissionId);

            return NoContent();
        }

    }
}
