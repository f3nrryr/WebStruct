using Microsoft.AspNetCore.Mvc;

namespace WebStruct.Controllers.Admin
{
    /// <summary>
    /// CRUD юзеров. НЕ AUTH!
    /// </summary>
    public class UsersController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
