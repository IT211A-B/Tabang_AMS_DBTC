using Microsoft.AspNetCore.Mvc;

namespace AMS_DBTC_Frontend.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Attendance()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }
        public IActionResult Settings()
        {
            return View();
        }
    }
}
