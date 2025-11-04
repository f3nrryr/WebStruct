using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsersRoles.Services.DTOs;
using UsersRoles.Services.Enums;
using UsersRoles.Services.Interfaces;

namespace WebStruct.Controllers.UsersRoles
{
    [Route("api/v1/roles")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(
            [FromHeader] Guid traceId,
            [FromBody] RoleCreateServiceModel model)
        {
            try
            {
                var result = await _rolesService.CreateRoleAsync(model);
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

        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateRole(
            [FromHeader] Guid traceId,
            string roleId,
            [FromBody] RoleUpdateServiceModel model)
        {
            try
            {
                var result = await _rolesService.UpdateRoleAsync(roleId, model);
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

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(
            [FromHeader] Guid traceId,
            string roleId)
        {
            try
            {
                var result = await _rolesService.DeleteRoleAsync(roleId);
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

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(
            [FromHeader] Guid traceId,
            string roleId)
        {
            try
            {
                var role = await _rolesService.GetRoleByIdAsync(roleId);
                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles([FromHeader] Guid traceId)
        {
            try
            {
                var roles = await _rolesService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("{roleId}/permissions/{permission}")]
        public async Task<IActionResult> AddPermissionToRole(
            [FromHeader] Guid traceId,
            string roleId,
            PermissionEnum permission)
        {
            try
            {
                var result = await _rolesService.AddPermissionToRoleAsync(roleId, permission, traceId);
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

        [HttpDelete("{roleId}/permissions/{permission}")]
        public async Task<IActionResult> RemovePermissionFromRole(
            [FromHeader] Guid traceId,
            string roleId,
            PermissionEnum permission)
        {
            try
            {
                var result = await _rolesService.RemovePermissionFromRoleAsync(roleId, permission);
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

        [HttpPut("{roleId}/permissions")]
        public async Task<IActionResult> SetRolePermissions(
            [FromHeader] Guid traceId,
            string roleId,
            [FromBody] List<PermissionEnum> permissions)
        {
            try
            {
                var result = await _rolesService.SetRolePermissionsAsync(roleId, permissions, traceId);
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

        [HttpGet("{roleId}/permissions")]
        public async Task<IActionResult> GetRolePermissions(
            [FromHeader] Guid traceId,
            string roleId)
        {
            try
            {
                var permissions = await _rolesService.GetRolePermissionsAsync(roleId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("exists/{roleName}")]
        public async Task<IActionResult> RoleExists(
            [FromHeader] Guid traceId,
            string roleName)
        {
            try
            {
                var exists = await _rolesService.RoleExistsAsync(roleName);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}