using Project.Constraints.Models;
namespace Project.Constraints.Services
{
    [AutoInject]
    public interface ICustomSettingProvider
    {
        Type GetUserPageType();
        Type? GetDashboardType();
        Task<IQueryResult<UserInfo>> GetUserInfoAsync(string username, string password);
        Task<int> UpdateLoginInfo(UserInfo info);
    }
}
