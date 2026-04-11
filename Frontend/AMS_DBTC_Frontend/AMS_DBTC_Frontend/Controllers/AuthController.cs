using Microsoft.AspNetCore.Mvc;
using AMS_DBTC_Frontend.Models.AuthRequest;
namespace AMS_DBTC_Frontend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Username and password are required"
                });
            }
            if (model.Username == "admin" && model.Password == "1234")
            {
                HttpContext.Session.SetString("UserEmail", model.Username);
                return Ok(new
                {
                    success = true,
                    user = new { username = model.Username },
                    redirect = "Dashboard/Index"
                });
            }
            return Unauthorized(new
            {
                success = false,
                message = "Invalid credentials"
            });
        }

        public IActionResult Register() { 
            return Ok(new
            {
                success = true,
                message = "Regester endpoint is working"
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { success = true });
        }
    }
}