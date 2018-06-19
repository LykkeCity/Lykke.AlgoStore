using Lykke.AlgoStore.Api.Infrastructure.Attributes;
using Lykke.AlgoStore.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.Service.Security.Client;
using RolePermissionMatchModel = Lykke.AlgoStore.Api.Models.RolePermissionMatchModel;
using UserPermissionModel = Lykke.AlgoStore.Api.Models.UserPermissionModel;

namespace Lykke.AlgoStore.Api.Controllers
{
    [Authorize]
    [RequirePermission]
    [Route("api/v1/permissions")]
    public class AlgoStoreUserPermissionsController: Controller
    {
        private readonly ISecurityClient _securityClient;

        public AlgoStoreUserPermissionsController(ISecurityClient securityClient)
        {
            _securityClient = securityClient;
        }

        [HttpGet("getAll")]
        [SwaggerOperation("GetAllPermissions")]
        [ProducesResponseType(typeof(List<UserPermissionModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllPermissions()
        {
            var result = await _securityClient.GetAllPermissionsAsync();

            return Ok(Mapper.Map<List<UserPermissionModel>>(result));
        }

        [HttpGet("getById")]
        [SwaggerOperation("GetPermissionById")]
        [ProducesResponseType(typeof(UserPermissionModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetPermissionById(string permissionId)
        {
            var result = await _securityClient.GetPermissionByIdAsync(permissionId);

            if (result == null)
                return NotFound();

            return Ok(Mapper.Map<UserPermissionModel>(result));
        }

        [HttpGet("getByRoleId")]
        [SwaggerOperation("GetPermissionsByRoleId")]
        [ProducesResponseType(typeof(List<UserPermissionModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPermissionsByRoleId(string roleId)
        {          
            var result = await _securityClient.GetPermissionsByRoleIdAsync(roleId);

            return Ok(Mapper.Map<List<UserPermissionModel>>(result));
        }        

        [HttpPost("assignPermissions")]
        [SwaggerOperation("AssignMultiplePermissionToRole")]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> AssignMultiplePermissionToRole([FromBody] List<RolePermissionMatchModel> permissions)
        {
            var data = Mapper.Map<List<Lykke.Service.Security.Client.AutorestClient.Models.RolePermissionMatchModel>> (permissions);

            await _securityClient.AssignMultiplePermissionToRoleAsync(data);

            return NoContent();
        }

        [HttpPost("revokePermissions")]
        [SwaggerOperation("RevokeMultiplePermissions")]
        [ProducesResponseType(typeof(BaseErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RevokeMultiplePermissions([FromBody] List<RolePermissionMatchModel> role)
        {
            var data = Mapper.Map<List<Lykke.Service.Security.Client.AutorestClient.Models.RolePermissionMatchModel>>(role);

            await _securityClient.RevokeMultiplePermissionsAsync(data);

            return NoContent();
        }        

    }
}
