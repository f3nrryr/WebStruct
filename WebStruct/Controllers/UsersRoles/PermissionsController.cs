using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsersRoles.Services.DTOs;
using UsersRoles.Services.Interfaces;

namespace WebStruct.Controllers.UsersRoles
{
    [Route("api/v1/permissions")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionsService _permissionsService;

        public PermissionsController(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission(
            [FromHeader] Guid traceId,
            [FromBody] PermissionCreateServiceModel model)
        {
            try
            {
                var result = await _permissionsService.CreatePermissionAsync(model);
                if (result.Succeeded)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete("{permissionId}")]
        public async Task<IActionResult> DeletePermission(
            [FromHeader] Guid traceId,
            int permissionId)
        {
            try
            {
                var result = await _permissionsService.DeletePermissionAsync(permissionId);
                if (result.Succeeded)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("{permissionId}")]
        public async Task<IActionResult> GetPermissionById(
            [FromHeader] Guid traceId,
            int permissionId)
        {
            try
            {
                var permission = await _permissionsService.GetPermissionByIdAsync(permissionId);
                return Ok(permission);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetPermissionByName(
            [FromHeader] Guid traceId,
            string name)
        {
            try
            {
                var permission = await _permissionsService.GetPermissionByNameAsync(name);
                return Ok(permission);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPermissions([FromHeader] Guid traceId)
        {
            try
            {
                var permissions = await _permissionsService.GetAllPermissionsAsync();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("roles/{roleId}/permissions/{permissionId}")]
        public async Task<IActionResult> AddPermissionToRole(
            [FromHeader] Guid traceId,
            string roleId,
            int permissionId)
        {
            try
            {
                var result = await _permissionsService.AddPermissionToRoleAsync(roleId, permissionId, traceId);
                if (result.Succeeded)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpDelete("roles/{roleId}/permissions/{permissionId}")]
        public async Task<IActionResult> RemovePermissionFromRole(
            [FromHeader] Guid traceId,
            string roleId,
            int permissionId)
        {
            try
            {
                var result = await _permissionsService.RemovePermissionFromRoleAsync(roleId, permissionId);
                if (result.Succeeded)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPut("roles/{roleId}/permissions")]
        public async Task<IActionResult> SetRolePermissions(
            [FromHeader] Guid traceId,
            string roleId,
            [FromBody] List<int> permissionIds)
        {
            try
            {
                var result = await _permissionsService.SetRolePermissionsAsync(roleId, permissionIds, traceId);
                if (result.Succeeded)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("roles/{roleId}")]
        public async Task<IActionResult> GetRolePermissions(
            [FromHeader] Guid traceId,
            string roleId)
        {
            try
            {
                var permissions = await _permissionsService.GetRolePermissionsAsync(roleId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserPermissions(
            [FromHeader] Guid traceId,
            string userId)
        {
            try
            {
                var permissions = await _permissionsService.GetUserPermissionsAsync(userId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("users/{userId}/check/{permissionId}")]
        public async Task<IActionResult> UserHasPermissionById(
            [FromHeader] Guid traceId,
            string userId,
            int permissionId)
        {
            try
            {
                var hasPermission = await _permissionsService.UserHasPermissionAsync(userId, permissionId);
                return Ok(hasPermission);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("users/{userId}/check/name/{permissionName}")]
        public async Task<IActionResult> UserHasPermissionByName(
            [FromHeader] Guid traceId,
            string userId,
            string permissionName)
        {
            try
            {
                var hasPermission = await _permissionsService.UserHasPermissionAsync(userId, permissionName);
                return Ok(hasPermission);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("roles/{roleId}/permissions/batch")]
        public async Task<IActionResult> AddPermissionsToRole(
            [FromHeader] Guid traceId,
            string roleId,
            [FromBody] List<int> permissionIds)
        {
            try
            {
                var result = await _permissionsService.AddPermissionsToRoleAsync(roleId, permissionIds, traceId);
                if (result.Succeeded)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("roles/batch-permissions")]
        public async Task<IActionResult> GetPermissionsByRoles(
            [FromHeader] Guid traceId,
            [FromBody] List<string> roleIds)
        {
            try
            {
                var permissionsByRoles = await _permissionsService.GetPermissionsByRolesAsync(roleIds);
                return Ok(permissionsByRoles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}