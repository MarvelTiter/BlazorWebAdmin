using DocumentFormat.OpenXml.EMMA;
using MDbContext.Repository;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
	public partial class LoginService
	{
		public async Task<IQueryResult<UserInfo>> GetUserInfo(string username, string password)
		{
			var u = await context.Repository<User>().GetSingleAsync(u => u.UserId == username);
			var userInfo = new UserInfo
			{
				UserId = username,
				UserName = u?.UserName ?? "",
			};
			var result = userInfo.Result(u != null);
			if (!result.Success)
			{
				result.Message = $"用户：{username} 不存在";
				return result;
			}
			if (u!.Password != password)
			{
				result.Message = "密码错误";
				result.Success = false;
				return result;
			}
			var roles = await context.Repository<UserRole>().GetListAsync(ur => ur.UserId == username);
			//var userInfo = new UserInfo
			//{
			//    UserId = username,
			//    UserName = u.UserName,
			//    Roles = roles.Select(ur => ur.RoleId).ToList()
			//};
			userInfo.Roles = roles.Select(ur => ur.RoleId).ToList();

			return result;
		}

		public async Task<int> UpdateLoginInfo(UserInfo info)
		{
			return await context.Update<User>()
									.Set(u => u.LastLogin, DateTime.Now)
									.Where(u => u.UserId == info.UserId).ExecuteAsync();
		}
	}
}
