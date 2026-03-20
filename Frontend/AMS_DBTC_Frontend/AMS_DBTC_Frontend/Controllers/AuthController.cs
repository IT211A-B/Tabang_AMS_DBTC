using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/AMS/Auth/Login.cshtml");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/AMS/Auth/Register.cshtml");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View("~/Views/AMS/Auth/Login.cshtml");
        }
    }
}