using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Project.AppCore.Auth;
using Project.AppCore.Services;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Permissions;

namespace Project.Services
{
	public partial class LoginService : ILoginService
	{
		private readonly IExpressionContext context;

		public LoginService(IExpressionContext context)
		{
			this.context = context;
		}
		public Task<bool> CheckUser(UserInfo info)
		{
			return Task.FromResult(true);
		}

		public async Task<IQueryResult<UserInfo>> LoginAsync(string username, string password)
		{
			var result = await GetUserInfo(username, password);
			await UpdateLastLoginTimeAsync(result.Payload);
			return result;
		}

		public async Task<IQueryResult<bool>> UpdateLastLoginTimeAsync(UserInfo info)
		{			
			var flag = await UpdateLoginInfo(info);
			info.Payload = JwtTokenHelper.GetToken(info.UserId, null, info.Roles.ToArray());
			return (flag > 0).Result();
		}

		public Task<IQueryResult<bool>> LogoutAsync()
		{
			//TODO 用户登出处理
			return Task.FromResult((true).Result());
		}
	}
}
