using Project.Constraints.Models;
namespace Project.Constraints.Services
{
    [AutoInject]
    public interface ICustomSettingProvider
    {
        Type? GetUserPageType();
        Type? GetDashboardType();
        Type? GetPermissionPageType();
        Type? GetRolePermissionPageType();
        Type? GetRunLogPageType();
        Task<IQueryResult<UserInfo>> GetUserInfoAsync(string username, string password);
        Task<int> UpdateLoginInfo(UserInfo info);
    }
}
