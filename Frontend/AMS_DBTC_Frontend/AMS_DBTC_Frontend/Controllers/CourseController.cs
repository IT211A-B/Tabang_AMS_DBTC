using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/AMS/Course/Index.cshtml");
        }
    }
}