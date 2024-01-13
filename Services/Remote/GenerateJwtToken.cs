using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LinuxApp.Services.Remote
{
    public class GenerateJwtToken
    {
        
        public string Generate()
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345dfgbM4kf5kfklel3skdkbjvnDFGDD234fgdsdfsdtkdsmcmsdllasLSDaEVVSF123445gdfgdfgdfgwercvddk"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: config.GetSection("BaseUrl").Value,
                audience: "webapp",
                claims: new List<Claim>(),
                signingCredentials: signinCredentials,
                expires: DateTime.Now.AddMinutes(5)
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return tokenString;
        }
    
    }
}