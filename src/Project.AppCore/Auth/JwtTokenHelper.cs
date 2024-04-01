using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Project.AppCore.Auth
{
    public class JwtTokenHelper
    {
        public static string GetToken(string uid, IConfiguration? configuration, params string[] roles)
        {
            // 1. 选择加密算法
            //var algorithm = SecurityAlgorithms.HmacSha256;

            // 2. 定义需要使用到的Claims
            //var claims = new[]
            //{
            //    //sub user Id
            //    new Claim(JwtRegisteredClaimNames.Sub, "Duke"),
            //    //role Admin
            //    new Claim(ClaimTypes.Role, "Admin"),
            //};

            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Name, uid)
            };

            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }

            // 3. 从 appsettings.json 中读取SecretKey
            //var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            // 4. 生成Credentials
            //var signingCredentials = new SigningCredentials(secretKey, algorithm);

            // 5. 根据以上组件，生成token
            var token = new JwtSecurityToken(claims: claims);
            // 6. 将token变为string
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static (string? Uid, string[] Roles) ReadToken(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(token);
            try
            {
                jwtToken.Payload.TryGetValue(JwtRegisteredClaimNames.Name, out var name);
                return (Uid: name?.ToString(), Roles: jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray());
            }
            catch (Exception e)
            {
                throw;
            }

        }
    }
}
