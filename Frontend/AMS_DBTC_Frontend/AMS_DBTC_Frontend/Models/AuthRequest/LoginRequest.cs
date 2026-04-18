using System.ComponentModel.DataAnnotations;

namespace AMS_DBTC_Frontend.Models.AuthRequest
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
