using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ebikeshopserver.Models.User;
using Microsoft.IdentityModel.Tokens;
using System;
using appsettings.

namespace ebikeshopserver.Authentication
{
    public class JwtHelper
    {
        public static string GenerateAuthToken(User user)
        {
            secretKey = 

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new Claim[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("isAdmin", user.IsAdmin.ToString()),
                new Claim("isAdmin", user.IsAdmin.ToString()),
                new Claim("first", user.UserName.FirstName.ToString()),
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "CardsServer",
                audience: "CardReactFront",
                claims: claims,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

