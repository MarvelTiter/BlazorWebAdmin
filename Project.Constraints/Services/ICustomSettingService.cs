namespace Project.Constraints.Services
{
    [AutoInject]
    public interface ICustomSettingService
    {
        Type? GetUserPageType();
        Type? GetDashboardType();
        Type? GetPermissionPageType();
        Type? GetRolePermissionPageType();
        Type? GetRunLogPageType();
        Task<IQueryResult<UserInfo>> GetUserInfoAsync(string username, string password);
        Task<int> UpdateLoginInfo(UserInfo info);
        Task<bool> OnLoginSuccessAsync(IQueryResult<UserInfo> result);
    }
}
