using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersRoles.Services.Interfaces;

namespace WebStruct.Controllers.UsersRoles
{
    /// <summary>
    /// CRUD юзеров.
    /// </summary>
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("")]
        [Authorize(Roles = "")]
        public async Task<> LaLaLa()
        {
            try
            {

            }
            catch (Exception ex)
            {
                return BadRequest(new );
            }
        }
    }
}
