using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersRoles.Services.DTOs;
using UsersRoles.Services.Enums;
using UsersRoles.Services.Interfaces;

namespace WebStruct.Controllers.UsersRoles
{
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

        [HttpPost]
        public async Task<IActionResult> CreateUser(
            [FromHeader] Guid traceId,
            [FromBody] UserCreateServiceModel model)
        {
            try
            {
                var result = await _usersService.CreateUserAsync(model);
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

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(
            [FromHeader] Guid traceId,
            string userId,
            [FromBody] UserUpdateServiceModel model)
        {
            try
            {
                var result = await _usersService.UpdateUserAsync(userId, model);
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

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(
            [FromHeader] Guid traceId,
            string userId)
        {
            try
            {
                var result = await _usersService.DeleteUserAsync(userId);
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

        [HttpGet("{userId}")]
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
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromHeader] Guid traceId)
        {
            try
            {
                var users = await _usersService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AssignRolesToUser(
            [FromHeader] Guid traceId,
            string userId,
            [FromBody] List<string> roles)
        {
            try
            {
                var result = await _usersService.AssignRolesToUserAsync(userId, roles);
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

        [HttpGet("{userId}/roles")]
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
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("{userId}/change-password")]
        public async Task<IActionResult> ChangeUserPassword(
            [FromHeader] Guid traceId,
            string userId,
            [FromBody] ChangePasswordServiceModel model)
        {
            try
            {
                var result = await _usersService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
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

        [HttpPost("{userId}/admin-password")]
        public async Task<IActionResult> AdminChangeUserPassword(
            [FromHeader] Guid traceId,
            string userId,
            [FromBody] AdminChangePasswordServiceModel model)
        {
            try
            {
                var result = await _usersService.AdminChangePasswordAsync(userId, model.NewPassword);
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

        [HttpPost("{userId}/email-confirmation")]
        public async Task<IActionResult> SendEmailConfirmation(
            [FromHeader] Guid traceId,
            string userId,
            [FromQuery] string callbackUrl)
        {
            try
            {
                var result = await _usersService.SendEmailConfirmationAsync(userId, callbackUrl);
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

        [HttpPost("{userId}/confirm-email")]
        public async Task<IActionResult> ConfirmUserEmail(
            [FromHeader] Guid traceId,
            string userId,
            [FromQuery] string token)
        {
            try
            {
                var result = await _usersService.ConfirmEmailAsync(userId, token);
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

        [HttpPost("password-reset")]
        public async Task<IActionResult> SendPasswordReset(
            [FromHeader] Guid traceId,
            [FromQuery] string email,
            [FromQuery] string callbackUrl)
        {
            try
            {
                var result = await _usersService.SendPasswordResetAsync(email, callbackUrl);
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

        [HttpPost("reset-password")]
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
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("{userId}/permissions/check")]
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
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("{userId}/permissions")]
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
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost("{userId}/activate")]
        public async Task<IActionResult> ActivateUser(
            [FromHeader] Guid traceId,
            string userId)
        {
            try
            {
                var result = await _usersService.ActivateUserAsync(userId);
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

        [HttpPost("{userId}/deactivate")]
        public async Task<IActionResult> DeactivateUser(
            [FromHeader] Guid traceId,
            string userId)
        {
            try
            {
                var result = await _usersService.DeactivateUserAsync(userId);
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
    }
}