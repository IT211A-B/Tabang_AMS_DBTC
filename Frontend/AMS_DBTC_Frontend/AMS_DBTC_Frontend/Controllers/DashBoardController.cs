using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class DashBoardController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/AMS/DashBoard/Index.cshtml");
        }

        [HttpGet]
        public IActionResult Attendance()
        {
            return View("~/Views/AMS/DashBoard/Attendance.cshtml");
        }

        [HttpGet]
        public IActionResult Reports()
        {
            return View("~/Views/AMS/DashBoard/Reports.cshtml");
        }

        [HttpGet]
        public IActionResult Settings()
        {
            return View("~/Views/AMS/DashBoard/Settings.cshtml");
        }
    }
}