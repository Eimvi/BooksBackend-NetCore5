using Microsoft.IdentityModel.Tokens;
using Servicios.api.Seguridad.Core.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.api.Seguridad.Core.JwtLogic
{
    public class JwtGenerator : IJwtGenerator
    {
        public string CreateToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                //new Claim(JwtRegisteredClaimNames.NameId, usuario.UserName)
                new Claim("username", usuario.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(""));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokeDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credential
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokeDescription);

            return tokenHandler.WriteToken(token);
        }
    }
}
