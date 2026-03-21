using AMS.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AMS.Helpers
{
    /// <summary>
    /// Generates and validates JWT tokens.
    /// Referenced as 'JwtHelper' in existing AuthService.cs.
    /// </summary>
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        // Inject IConfiguration to read JWT settings from appsettings.json
        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        // ── Generate Token ─────────────────────────────────────────────────
        /// <summary>
        /// Create a signed JWT token for the given user.
        /// Token contains: userId, email, role as claims.
        /// </summary>
        public string GenerateToken(User user)
        {
            // Read JWT config from appsettings.json → "Jwt" section
            var secretKey = _config["Jwt:Key"] ?? "default_secret_key_change_me_32chars";
            var issuer = _config["Jwt:Issuer"] ?? "AMS";
            var audience = _config["Jwt:Audience"] ?? "AMS";
            var expireHours = int.Parse(_config["Jwt:ExpireHours"] ?? "8");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Build claims — these are embedded inside the JWT payload
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),   // subject = userId
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),                        // used for [Authorize(Roles="Admin")]
                new Claim("fullName", $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // unique token ID
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expireHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ── Get Expiry ─────────────────────────────────────────────────────
        /// <summary>Returns the expiry DateTime for a newly generated token.</summary>
        public DateTime GetExpiry()
        {
            var expireHours = int.Parse(_config["Jwt:ExpireHours"] ?? "8");
            return DateTime.UtcNow.AddHours(expireHours);
        }
    }
}