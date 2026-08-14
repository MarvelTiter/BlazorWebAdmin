namespace BlazorTemplate.ClientCore;

public interface IAuthenticationStateProvider
{
    IAuthService? AuthService { get; }
    UserInfo? Current {  get; }
    // Task IdentifyUser(UserInfo info);
    Task ClearState();
}
