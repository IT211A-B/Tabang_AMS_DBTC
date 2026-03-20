using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/AMS/Student/Index.cshtml");
        }
    }
}