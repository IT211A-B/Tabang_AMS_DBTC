using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/AMS/DashBoard/Index.cshtml");
        }

        public IActionResult Attendance()
        {
            return View("~/Views/AMS/DashBoard/Attendance.cshtml");
        }

        public IActionResult Reports()
        {
            return View("~/Views/AMS/DashBoard/Reports.cshtml");
        }

        public IActionResult Settings()
        {
            return View("~/Views/AMS/DashBoard/Settings.cshtml");
        }
    }
}