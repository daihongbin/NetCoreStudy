using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Michael.JwtProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        public ActionResult<string> Get()
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name,"Michael"),
                new Claim(JwtRegisteredClaimNames.Email,"320459582@qq.com"),
                new Claim(JwtRegisteredClaimNames.Sub,"D21D099B-B49B-4604-A247-71B0518A0B1C")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456"));

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5001",
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            //还可以这样创建Token
            //var claims = new Claim[]
            //{
            //    new Claim(ClaimTypes.Name,"Michael"),
            //    new Claim(JwtRegisteredClaimNames.Email,"320459582@qq.com"),
            //    new Claim(JwtRegisteredClaimNames.Sub,"D21D099B-B49B-4604-A247-71B0518A0B1C"),
            //    new Claim(JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMilliseconds(1)).ToUnixTimeSeconds()}"),
            //    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}")
            //};

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456"));

            //var jwtToken = new JwtSecurityToken(new JwtHeader(new SigningCredentials(key, SecurityAlgorithms.HmacSha256)), new JwtPayload(claims));

            return jwtToken;
        }
    }
}
