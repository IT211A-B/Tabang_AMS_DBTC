using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToLogin();
            return View("~/Views/AMS/DashBoard/Index.cshtml");
        }

        public IActionResult Attendance()
        {
            if (!IsLoggedIn()) return RedirectToLogin();
            return View("~/Views/AMS/DashBoard/Attendance.cshtml");
        }

        public IActionResult Reports()
        {
            if (!IsLoggedIn()) return RedirectToLogin();
            return View("~/Views/AMS/DashBoard/Reports.cshtml");
        }

        public IActionResult Settings()
        {
            if (!IsLoggedIn()) return RedirectToLogin();
            return View("~/Views/AMS/DashBoard/Settings.cshtml");
        }

        private bool IsLoggedIn() =>
            HttpContext.Session.GetString("UserEmail") != null;

        private IActionResult RedirectToLogin() =>
            RedirectToAction("Login", "Auth");
    }
}
