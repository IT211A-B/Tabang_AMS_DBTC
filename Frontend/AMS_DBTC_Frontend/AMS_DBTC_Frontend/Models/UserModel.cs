using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AMS_DBTC_Frontend.Models
{
    public class UserModel
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; } = "Teacher";

        public string Course { get; set; } = "BSIT 1";

        //  Safe initials 
        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Username))
                    return "";

                var parts = Username
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Take(2)
                    .Select(n => n[0].ToString().ToUpper());

                return string.Concat(parts);
            }
        }
    }
}

