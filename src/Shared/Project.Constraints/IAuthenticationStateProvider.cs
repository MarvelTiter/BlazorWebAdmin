using Microsoft.AspNetCore.Http;

namespace Project.Constraints;

public interface IAuthenticationStateProvider
{
    UserInfo? Current {  get; }
    HttpContext? HttpContext { get; }
    Task IdentifyUser(UserInfo info);
    Task ClearState();
}
