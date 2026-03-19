using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            if (IsLoggedIn())
                return RedirectToAction("Index", "DashBoard");

            return View("~/Views/AMS/Auth/Login.cshtml");
        }

        public IActionResult Register()
        {
            if (IsLoggedIn())
                return RedirectToAction("Index", "DashBoard");

            return View("~/Views/AMS/Auth/Register.cshtml");
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult SetSession([FromForm] string email, [FromForm] string role)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { error = "Email required." });

            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetString("UserRole", role ?? "teacher");

            return Ok(new { success = true });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        private bool IsLoggedIn() =>
            HttpContext.Session.GetString("UserEmail") != null;
    }
}
