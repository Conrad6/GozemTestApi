using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace GozemApi {
    public class JwtTokenGenerator
    {
        public static string GenerateJwtToken(IEnumerable<Claim> loadedClaims, string secretKey, int lifetime, string audience, string issuer)
        {
            var claims = loadedClaims.ToList();

            var secKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: now.AddDays(lifetime),
                signingCredentials: credentials
            );
#if DEBUG
            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(token);
            return tokenString;
#else
            return new JwtSecurityTokenHandler().WriteToken(token);
#endif
        }
    }
}