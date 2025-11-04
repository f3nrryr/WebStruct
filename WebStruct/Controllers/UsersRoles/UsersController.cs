using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersRoles.Services.DTOs;
using UsersRoles.Services.Enums;
using UsersRoles.Services.Interfaces;

namespace WebStruct.Controllers.UsersRoles
{
    /// <summary>
    /// Контроллер для управления пользователями системы
    /// </summary>
    [Route("api/v1/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="model">Данные для создания пользователя</param>
        /// <returns>Созданный пользователь</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Users.Create
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserServiceModel), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> CreateUser(
            [FromHeader] Guid traceId,
            [FromBody] UserCreateServiceModel model)
        {
            try
            {
                var result = await _usersService.CreateUserAsync(model);
                if (result.Succeeded)
                    return Ok(result.Data);
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="model">Данные для обновления</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Users.Update
        /// </remarks>
        [HttpPut("{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> UpdateUser(
            [FromHeader] Guid traceId,
            string userId,
            [FromBody] UserUpdateServiceModel model)
        {
            try
            {
                var result = await _usersService.UpdateUserAsync(userId, model);
                if (result.Succeeded)
                    return Ok();
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Users.Delete
        /// </remarks>
        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> DeleteUser(
            [FromHeader] Guid traceId,
            string userId)
        {
            try
            {
                var result = await _usersService.DeleteUserAsync(userId);
                if (result.Succeeded)
                    return Ok();
                else
                    return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        /// <summary>
        /// Получение пользователя по идентификатору
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Данные пользователя</returns>
        /// <remarks>
        /// Пользователи могут получать только свои данные, администраторы - любые
        /// </remarks>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserServiceModel), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> GetUserById(
            [FromHeader] Guid traceId,
            string userId)
        {
            try
            {
                var user = await _usersService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Получение списка всех пользователей
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <returns>Список пользователей</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Users.ReadAll
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<UserServiceModel>), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> GetAllUsers([FromHeader] Guid traceId)
        {
            try
            {
                var users = await _usersService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Назначение ролей пользователю
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="roles">Список ролей для назначения</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Roles.Assign
        /// </remarks>
        [HttpPost("{userId}/roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> AssignRolesToUser(
            [FromHeader] Guid traceId,
            string userId,
            [FromBody] List<string> roles)
        {
            try
            {
                var result = await _usersService.AssignRolesToUserAsync(userId, roles);
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
        /// Получение ролей пользователя
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список ролей пользователя</returns>
        /// <remarks>
        /// Пользователи могут получать только свои роли, администраторы - любые
        /// </remarks>
        [HttpGet("{userId}/roles")]
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> GetUserRoles(
            [FromHeader] Guid traceId,
            string userId)
        {
            try
            {
                var roles = await _usersService.GetUserRolesAsync(userId);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Смена пароля пользователем
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="model">Модель смены пароля</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Пользователи могут менять только свой пароль
        /// </remarks>
        [HttpPost("{userId}/change-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> ChangeUserPassword(
            [FromHeader] Guid traceId,
            string userId,
            [FromBody] ChangePasswordServiceModel model)
        {
            try
            {
                var result = await _usersService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
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
        /// Смена пароля пользователя администратором
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="model">Модель смены пароля администратором</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Users.ChangePassword
        /// </remarks>
        [HttpPost("{userId}/admin-password")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> AdminChangeUserPassword(
            [FromHeader] Guid traceId,
            string userId,
            [FromBody] AdminChangePasswordServiceModel model)
        {
            try
            {
                var result = await _usersService.AdminChangePasswordAsync(userId, model.NewPassword);
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
        /// Отправка email для подтверждения адреса электронной почты
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="callbackUrl">URL для обратного вызова после подтверждения</param>
        /// <returns>Результат операции</returns>
        [HttpPost("{userId}/email-confirmation")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> SendEmailConfirmation(
            [FromHeader] Guid traceId,
            string userId,
            [FromQuery] string callbackUrl)
        {
            try
            {
                var result = await _usersService.SendEmailConfirmationAsync(userId, callbackUrl);
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
        /// Подтверждение адреса электронной почты
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="token">Токен подтверждения</param>
        /// <returns>Результат операции</returns>
        [HttpPost("{userId}/confirm-email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> ConfirmUserEmail(
            [FromHeader] Guid traceId,
            string userId,
            [FromQuery] string token)
        {
            try
            {
                var result = await _usersService.ConfirmEmailAsync(userId, token);
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
        /// Отправка запроса на сброс пароля
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="email">Адрес электронной почты</param>
        /// <param name="callbackUrl">URL для обратного вызова после сброса</param>
        /// <returns>Результат операции</returns>
        [HttpPost("password-reset")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> SendPasswordReset(
            [FromHeader] Guid traceId,
            [FromQuery] string email,
            [FromQuery] string callbackUrl)
        {
            try
            {
                var result = await _usersService.SendPasswordResetAsync(email, callbackUrl);
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
        /// Сброс пароля пользователя
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="email">Адрес электронной почты</param>
        /// <param name="token">Токен сброса пароля</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <returns>Результат операции</returns>
        [HttpPost("reset-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> ResetUserPassword(
            [FromHeader] Guid traceId,
            [FromQuery] string email,
            [FromQuery] string token,
            [FromBody] string newPassword)
        {
            try
            {
                var result = await _usersService.ResetPasswordAsync(email, token, newPassword);
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
        /// Проверка наличия права у пользователя
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="permission">Право для проверки</param>
        /// <returns>Результат проверки</returns>
        /// <remarks>
        /// Пользователи могут проверять только свои права, администраторы - любые
        /// </remarks>
        [HttpGet("{userId}/permissions/check")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> CheckUserPermission(
            [FromHeader] Guid traceId,
            string userId,
            [FromQuery] PermissionEnum permission)
        {
            try
            {
                var hasPermission = await _usersService.UserHasPermissionAsync(userId, permission);
                return Ok(hasPermission);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Получение списка прав пользователя
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список прав пользователя</returns>
        /// <remarks>
        /// Пользователи могут получать только свои права, администраторы - любые
        /// </remarks>
        [HttpGet("{userId}/permissions")]
        [ProducesResponseType(typeof(List<PermissionServiceModel>), 200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> GetUserPermissions(
            [FromHeader] Guid traceId,
            string userId)
        {
            try
            {
                var permissions = await _usersService.GetUserPermissionsAsync(userId);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(new List<string> { ex.ToString() });
            }
        }

        /// <summary>
        /// Активация пользователя
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Users.Activate
        /// </remarks>
        [HttpPost("{userId}/activate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> ActivateUser(
            [FromHeader] Guid traceId,
            string userId)
        {
            try
            {
                var result = await _usersService.ActivateUserAsync(userId);
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
        /// Деактивация пользователя
        /// </summary>
        /// <param name="traceId">Идентификатор трейсинга</param>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Результат операции</returns>
        /// <remarks>
        /// Для выполнения операции требуется право доступа: Users.Deactivate
        /// </remarks>
        [HttpPost("{userId}/deactivate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(List<string>), 400)]
        public async Task<IActionResult> DeactivateUser(
            [FromHeader] Guid traceId,
            string userId)
        {
            try
            {
                var result = await _usersService.DeactivateUserAsync(userId);
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
    }
}