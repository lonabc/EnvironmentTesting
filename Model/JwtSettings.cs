using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TempModbusProject.Model
{
    public class JwtSettings
    {
        public string SecKey { get; set; }
        public int ExpireSeconds { get; set; }

        public static string buildeToken(List<Claim> claims, string key, DateTime expire)
        {
            byte[] secBytes=Encoding.UTF8.GetBytes(key);
            var secKey = new SymmetricSecurityKey(secBytes);
            var credentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(claims: claims, expires: expire, signingCredentials: credentials);
            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }

}
