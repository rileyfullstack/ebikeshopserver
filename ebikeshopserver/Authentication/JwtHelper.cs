using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ebikeshopserver.Models.User;
using Microsoft.IdentityModel.Tokens;
using System;
using ebikeshopserver.Services.SecretSettings;

namespace ebikeshopserver.Authentication
{
    public class JwtHelper
    {
        public static string GenerateAuthToken(User user)
        {
            string secretKey = SecretSettingsService.GetPasswordHasher();

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new Claim[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("role", user.Role),
                new Claim("firstName", user.FirstName)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "EbikeShopServer",
                audience: "EbikeShopFrontEnd",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

