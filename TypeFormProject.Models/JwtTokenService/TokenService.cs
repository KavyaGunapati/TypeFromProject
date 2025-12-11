
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TypeFormProject.Models.DTOs;

namespace TypeFormProject.Models.JwtTokenService
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UserTokenData user)
        {
            // Read settings
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var minutesStr = _configuration["Jwt:AccessTokenMinutes"];

            if (string.IsNullOrWhiteSpace(key) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience) ||
                string.IsNullOrWhiteSpace(minutesStr))
            {
                throw new InvalidOperationException("JWT configuration is missing (Key/Issuer/Audience/AccessTokenMinutes).");
            }

            var accessMinutes = int.Parse(minutesStr);

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Claims
            var claims = new List<Claim>
            {
                // Standard JWT claims
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),          
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                // Identity-style claims
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            if (user.Roles is not null)
            {
                foreach (var role in user.Roles)
                {
                    if (!string.IsNullOrWhiteSpace(role))
                        claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(accessMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string Token, DateTime ExpiresAt) GenerateRefreshToken()
        {
            var daysStr = _configuration["Jwt:RefreshTokenDays"];
            var days = string.IsNullOrWhiteSpace(daysStr) ? 14 : int.Parse(daysStr);

            var randomBytes = new byte[64]; 
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var token = Convert.ToBase64String(randomBytes);
            var expiresAt = DateTime.UtcNow.AddDays(days);

            return (token, expiresAt);
        }
    }
}
