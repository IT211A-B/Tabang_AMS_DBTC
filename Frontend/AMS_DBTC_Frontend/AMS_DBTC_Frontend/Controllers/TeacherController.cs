using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class TeacherController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/AMS/Teacher/Index.cshtml");
        }
    }
}