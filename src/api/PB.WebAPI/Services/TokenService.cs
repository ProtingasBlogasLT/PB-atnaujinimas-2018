using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PB.WebAPI.Models;

namespace PB.WebAPI.Services
{
    public class TokenService : ITokenService
    {
        private AppSettings AppSettings { get; }
        private SecurityTokenHandler TokenHandler { get; }

        public TokenService(IOptions<AppSettings> appSettings, SecurityTokenHandler tokenHandler)
        { 
            AppSettings = appSettings.Value;
            TokenHandler = tokenHandler;
        }

        public string GenerateToken(long userID)
        {
            var secret = Encoding.ASCII.GetBytes(AppSettings.TokenSecret);
            var key = new SymmetricSecurityKey(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userID.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = TokenHandler.CreateToken(tokenDescriptor);
            var tokenStr = TokenHandler.WriteToken(token);
            return tokenStr;
        }
    }
}
