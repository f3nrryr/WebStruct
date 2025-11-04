using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsersRoles.Services.DTOs;
using UsersRoles.Services.Interfaces;

namespace WebStruct.Controllers.UsersRoles
{
    /// <summary>
    /// Контроллер для управления правами доступа системы
    /// </summary>
    [Route("api/v1/permissions")]
    [ApiController]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionsService _permissionsService;

        public PermissionsController(IPermissionsService permissionsService)
        {
            _permissionsService = permissionsService;
        }

        /// <summary>
        /// Создание нового права доступа
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="model">Данные для создания права</param>
        /// <returns>Созданное право доступа</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Create
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PermissionServiceModel), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> CreatePermission(
            [FromHeader] Guid traceId,
            [FromBody] PermissionCreateServiceModel model)
        {
            try
            {
                var result = await _permissionsService.CreatePermissionAsync(model);
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
        /// Удаление права доступа
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="permissionId">Идентификатор права</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Delete
        /// </remarks>
        [HttpDelete("{permissionId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> DeletePermission(
            [FromHeader] Guid traceId,
            int permissionId)
        {
            try
            {
                var result = await _permissionsService.DeletePermissionAsync(permissionId);
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
        /// Получение права доступа по идентификатору
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="permissionId">Идентификатор права</param>
        /// <returns>Данные права доступа</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Read
        /// </remarks>
        [HttpGet("{permissionId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PermissionServiceModel), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
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
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Получение права доступа по имени
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="name">Название права</param>
        /// <returns>Данные права доступа</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Read
        /// </remarks>
        [HttpGet("name/{name}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PermissionServiceModel), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
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
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Получение списка всех прав доступа
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <returns>Список прав доступа</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.ReadAll
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<PermissionServiceModel>), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> GetAllPermissions([FromHeader] Guid traceId)
        {
            try
            {
                var permissions = await _permissionsService.GetAllPermissionsAsync();
                return Ok(permissions);
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
        /// <param name="permissionId">Идентификатор права</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Assign
        /// </remarks>
        [HttpPost("roles/{roleId}/permissions/{permissionId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> AddPermissionToRole(
            [FromHeader] Guid traceId,
            string roleId,
            int permissionId)
        {
            try
            {
                var result = await _permissionsService.AddPermissionToRoleAsync(roleId, permissionId, traceId);
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
        /// <param name="permissionId">Идентификатор права</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Revoke
        /// </remarks>
        [HttpDelete("roles/{roleId}/permissions/{permissionId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> RemovePermissionFromRole(
            [FromHeader] Guid traceId,
            string roleId,
            int permissionId)
        {
            try
            {
                var result = await _permissionsService.RemovePermissionFromRoleAsync(roleId, permissionId);
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
        /// <param name="permissionIds">Список идентификаторов прав</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Manage
        /// </remarks>
        [HttpPut("roles/{roleId}/permissions")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> SetRolePermissions(
            [FromHeader] Guid traceId,
            string roleId,
            [FromBody] List<int> permissionIds)
        {
            try
            {
                var result = await _permissionsService.SetRolePermissionsAsync(roleId, permissionIds, traceId);
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
        [HttpGet("roles/{roleId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<PermissionServiceModel>), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
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
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Получение прав пользователя
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список прав пользователя</returns>
        /// <remarks>
        /// Пользователи могут получать только свои права, администраторы - любые
        /// </remarks>
        [HttpGet("users/{userId}")]
        [ProducesResponseType(typeof(List<PermissionServiceModel>), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
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
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Проверка наличия права у пользователя по идентификатору
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="permissionId">Идентификатор права</param>
        /// <returns>Результат проверки</returns>
        /// <remarks>
        /// Пользователи могут проверять только свои права, администраторы - любые
        /// </remarks>
        [HttpGet("users/{userId}/check/{permissionId}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
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
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Проверка наличия права у пользователя по имени
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="permissionName">Название права</param>
        /// <returns>Результат проверки</returns>
        /// <remarks>
        /// Пользователи могут проверять только свои права, администраторы - любые
        /// </remarks>
        [HttpGet("users/{userId}/check/name/{permissionName}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
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
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Массовое добавление прав к роли
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="roleId">Идентификатор роли</param>
        /// <param name="permissionIds">Список идентификаторов прав</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.Assign
        /// </remarks>
        [HttpPost("roles/{roleId}/permissions/batch")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> AddPermissionsToRole(
            [FromHeader] Guid traceId,
            string roleId,
            [FromBody] List<int> permissionIds)
        {
            try
            {
                var result = await _permissionsService.AddPermissionsToRoleAsync(roleId, permissionIds, traceId);
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
        /// Получение прав по списку ролей
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="roleIds">Список идентификаторов ролей</param>
        /// <returns>Словарь с правами по ролям</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Permissions.ReadAll
        /// </remarks>
        [HttpPost("roles/batch-permissions")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Dictionary<string, List<PermissionServiceModel>>), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
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
                return BadRequest(new List<string> { ex.ToString() });
            }
        }
    }
}