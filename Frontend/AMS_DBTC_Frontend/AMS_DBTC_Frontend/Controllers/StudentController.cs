using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToLogin();
            return View("~/Views/AMS/Student/Index.cshtml");
        }

        private bool IsLoggedIn() =>
            HttpContext.Session.GetString("UserEmail") != null;

        private IActionResult RedirectToLogin() =>
            RedirectToAction("Login", "Auth");
    }
}
