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
    /// <summary>
    /// Контроллер для управления ролями системы
    /// </summary>
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

        /// <summary>
        /// Создание новой роли
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="model">Данные для создания роли</param>
        /// <returns>Созданная роль</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Roles.Create
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(RoleServiceModel), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> CreateRole(
            [FromHeader] Guid traceId,
            [FromBody] RoleCreateServiceModel model)
        {
            try
            {
                var result = await _rolesService.CreateRoleAsync(model);
                if (result.Succeeded)
                    return Ok(result.Data);
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Обновление данных роли
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <param name="model">Данные для обновления</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Roles.Update
        /// </remarks>
        [HttpPut("{roleId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> UpdateRole(
            [FromHeader] Guid traceId,
            string roleId,
            [FromBody] RoleUpdateServiceModel model)
        {
            try
            {
                var result = await _rolesService.UpdateRoleAsync(roleId, model);
                if (result.Succeeded)
                    return Ok();
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Удаление роли
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Roles.Delete
        /// </remarks>
        [HttpDelete("{roleId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> DeleteRole(
            [FromHeader] Guid traceId,
            string roleId)
        {
            try
            {
                var result = await _rolesService.DeleteRoleAsync(roleId);
                if (result.Succeeded)
                    return Ok();
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Получение роли по идентификатору
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <returns>Данные роли</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Roles.Read
        /// </remarks>
        [HttpGet("{roleId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(RoleServiceModel), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
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
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Получение списка всех ролей
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <returns>Список ролей</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Roles.ReadAll
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<RoleServiceModel>), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> GetAllRoles([FromHeader] Guid traceId)
        {
            try
            {
                var roles = await _rolesService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Добавление права к роли
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <param name="permission">Право для добавления</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Assign
        /// </remarks>
        [HttpPost("{roleId}/permissions/{permission}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> AddPermissionToRole(
            [FromHeader] Guid traceId,
            string roleId,
            PermissionEnum permission)
        {
            try
            {
                var result = await _rolesService.AddPermissionToRoleAsync(roleId, permission, traceId);
                if (result.Succeeded)
                    return Ok();
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Удаление права из роли
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <param name="permission">Право для удаления</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Revoke
        /// </remarks>
        [HttpDelete("{roleId}/permissions/{permission}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> RemovePermissionFromRole(
            [FromHeader] Guid traceId,
            string roleId,
            PermissionEnum permission)
        {
            try
            {
                var result = await _rolesService.RemovePermissionFromRoleAsync(roleId, permission);
                if (result.Succeeded)
                    return Ok();
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Установка прав для роли
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <param name="permissions">Список прав для установки</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Manage
        /// </remarks>
        [HttpPut("{roleId}/permissions")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> SetRolePermissions(
            [FromHeader] Guid traceId,
            string roleId,
            [FromBody] List<PermissionEnum> permissions)
        {
            try
            {
                var result = await _rolesService.SetRolePermissionsAsync(roleId, permissions, traceId);
                if (result.Succeeded)
                    return Ok();
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Получение прав роли
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <returns>Список прав роли</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Read
        /// </remarks>
        [HttpGet("{roleId}/permissions")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<PermissionEnum>), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
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
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Проверка существования роли по имени
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="roleName">Название роли</param>
        /// <returns>Результат проверки</returns>
        [HttpGet("exists/{roleName}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
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
                return BadRequest(new List<string> { ex.ToString() });
            }
        }
    }
}