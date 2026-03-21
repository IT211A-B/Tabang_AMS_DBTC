    using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "DashBoard");

            return View(); 
        }

        public IActionResult Register()
        {
            return View(); 
        }

        [HttpPost]
        public IActionResult SetSession(string email, string role)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest();

            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetString("UserRole", role ?? "teacher");

            return Ok(new { success = true });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}