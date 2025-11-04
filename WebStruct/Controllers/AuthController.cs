using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UsersRoles.DAL.CodeFirst;
using WebStruct.JWT;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<WebStructUser> _signInManager;
    private readonly UserManager<WebStructUser> _userManager;
    private readonly IJwtService _jwtService;

    public AuthController(
        SignInManager<WebStructUser> signInManager,
        UserManager<WebStructUser> userManager, IJwtService jwtService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles);

            return Ok(new
            {
                token = token,
                user = new
                {
                    username = user.UserName,
                    email = user.Email,
                    fullName = user.FullName,
                    roles = roles
                }
            });
        }

        return Unauthorized("Invalid credentials");
    }

    [HttpPost("refresh")]
    [Authorize]
    public async Task<IActionResult> RefreshToken()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        var newToken = _jwtService.GenerateToken(user, roles);

        return Ok(new { token = newToken });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}