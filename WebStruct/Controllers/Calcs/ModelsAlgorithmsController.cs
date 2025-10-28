using Microsoft.AspNetCore.Mvc;

namespace WebStruct.Controllers.Calcs
{
    /// <summary>
    /// Справка о комп. моделях. Их параметры и т.п. в json'ах для фронтенда.
    /// </summary>
    public class ModelsAlgorithmsController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
