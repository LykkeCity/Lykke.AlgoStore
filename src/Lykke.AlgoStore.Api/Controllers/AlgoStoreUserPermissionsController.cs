﻿using Lykke.AlgoStore.Api.Infrastructure.Attributes;
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
        [RequirePermissionAttribute]
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
        [RequirePermissionAttribute]
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
        [RequirePermissionAttribute]
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
        [RequirePermissionAttribute]
        [SwaggerOperation("SavePermission")]
        [ProducesResponseType(typeof(UserPermissionModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> SavePermission([FromBody] UserPermissionModel permission)
        {
            var data = AutoMapper.Mapper.Map<UserPermissionData>(permission);

            var result = await _permissionsService.SavePermissionAsync(data);

            return Ok(result);
        }

        [HttpPost("assignPermission")]
        [RequirePermissionAttribute]
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

        [HttpPost("assignPermissions")]
        [RequirePermissionAttribute]
        [SwaggerOperation("AssignMultiplePermissionToRole")]
        [ProducesResponseType(typeof(UserPermissionModel), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AssignMultiplePermissionToRole([FromBody] List<RolePermissionMatchModel> permissions)
        {
            var data = AutoMapper.Mapper.Map<List<RolePermissionMatchData>>(permissions);

            await _permissionsService.AssignPermissionsToRoleAsync(data);

            return NoContent();
        }

        [HttpPost("revokePermission")]
        [RequirePermissionAttribute]
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

        [HttpPost("revokePermissions")]
        [RequirePermissionAttribute]
        [SwaggerOperation("RevokeMultiplePermissions")]
        [ProducesResponseType(typeof(UserPermissionModel), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RevokeMultiplePermissions([FromBody] List<RolePermissionMatchModel> role)
        {
            var data = AutoMapper.Mapper.Map<List<RolePermissionMatchData>>(role);

            await _permissionsService.RevokePermissionsFromRole(data);

            return NoContent();
        }

        [HttpDelete("deletePermission")]
        [RequirePermissionAttribute]
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
