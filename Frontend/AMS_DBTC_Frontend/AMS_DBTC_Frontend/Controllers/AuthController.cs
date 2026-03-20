using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View("~/Views/AMS/Auth/Login.cshtml");
        }

        public IActionResult Register()
        {
            return View("~/Views/AMS/Auth/Register.cshtml");
        }

        public IActionResult Logout()
        {
            return View("~/Views/AMS/Auth/Login.cshtml");
        }
    }
}