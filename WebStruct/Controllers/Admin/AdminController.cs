using Microsoft.AspNetCore.Mvc;

namespace WebStruct.Controllers.Admin
{
    /// <summary>
    /// Супер-админка.
    /// </summary>
    public class AdminController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
