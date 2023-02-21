using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;

namespace BlazorWebAdmin.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class JwtAuthenticateController : ControllerBase
    {
        [HttpGet]
        public IActionResult Authenticate()
        {
            //        var jwtConfig = Configuration.GetSection("Jwt");
            //        //秘钥，就是标头，这里用Hmacsha256算法，需要256bit的密钥
            //        var securityKey = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.GetValue<string>("Secret"))), SecurityAlgorithms.HmacSha256);
            //        //Claim，JwtRegisteredClaimNames中预定义了好多种默认的参数名，也可以像下面的Guid一样自己定义键名.
            //        //ClaimTypes也预定义了好多类型如role、email、name。Role用于赋予权限，不同的角色可以访问不同的接口
            //        //相当于有效载荷
            //        var claims = new Claim[] {
            //            new Claim(JwtRegisteredClaimNames.Iss,jwtConfig.GetValue<string>("Iss")),
            //            new Claim(JwtRegisteredClaimNames.Aud,jwtConfig.GetValue<string>("Aud")),
            //            new Claim("Guid",Guid.NewGuid().ToString("D")),
            //            new Claim(ClaimTypes.Role,"system"),
            //            new Claim(ClaimTypes.Role,"admin"),
            //};
            //        SecurityToken securityToken = new JwtSecurityToken(
            //            signingCredentials: securityKey,
            //            expires: DateTime.Now.AddMinutes(2),//过期时间
            //            claims: claims
            //        );
            //        //生成jwt令牌
            //        return Content(new JwtSecurityTokenHandler().WriteToken(securityToken));
            throw new Exception();
        }
    }
}
