using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Project.Constraints.Utils;

namespace Project.Web.Shared.Auth;

// 标记为自动注入的服务，用于Blazor WebAssembly身份验证
[AutoInject(Group = "WASM", ServiceType = typeof(IAuthenticationStateProvider))]
[AutoInject(Group = "WASM", ServiceType = typeof(AuthenticationStateProvider))]
// 提供持久化身份验证状态的类
public class PersistentAuthenticationStateProvider : AuthenticationStateProvider, IAuthenticationStateProvider
{
    // 默认的未认证状态任务
    private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    // 应用会话接口，用于获取用户信息
    private readonly IUserStore userStore;
    private readonly NavigationManager navigation;
    private readonly IProjectSettingService settingService;
    private readonly IAuthService? authService;

    private CancellationTokenSource? roopTokenSource;

    // 身份验证状态任务，初始为未认证状态
    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

    // 日志记录接口，用于记录身份验证信息
    private readonly ILogger<IAuthenticationStateProvider> logger;

    // 构造函数，初始化PersistentAuthenticationStateProvider
    public PersistentAuthenticationStateProvider(PersistentComponentState state
        , IUserStore userStore
        , NavigationManager navigation
        , IProjectSettingService settingService
        , ILogger<IAuthenticationStateProvider> logger
        , IServiceProvider provider)
    {
        this.userStore = userStore;
        this.navigation = navigation;
        this.settingService = settingService;
        this.authService = provider.GetService<IAuthService>();
        this.logger = logger;

        // 从持久化状态中获取用户信息，如果存在，则设置用户信息并更新认证状态
        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var u) || u is null) return;
        userStore.SetUser(u);
        authenticationStateTask = Task.FromResult(new AuthenticationState(u.BuildClaims("Cookies")));
    }

    // 清除认证状态的方法，包括清除用户信息和执行登出重定向
    public Task ClearState()
    {
        //appSession.UserStore.ClearUser();
        //await httpContextAccessor.HttpContext.SignOutAsync();
        var redirect = navigation.ToAbsoluteUri(navigation.Uri);
        var encoded = Uri.EscapeDataString(redirect.PathAndQuery);
        navigation.NavigateTo($"/api/account/logout?Redirect={encoded}", true);
        return Task.CompletedTask;
    }

    // 获取当前用户信息的方法
    public UserInfo? Current => userStore.UserInfo;

    // 获取当前认证状态的方法
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (roopTokenSource is not null)
        {
            roopTokenSource.Cancel();
            roopTokenSource.Dispose();
        }
        roopTokenSource = new CancellationTokenSource();
        _ = RevalidationLoop(authenticationStateTask, roopTokenSource.Token);
        return authenticationStateTask;
    }

    private async Task RevalidationLoop(Task<AuthenticationState> task, CancellationToken cancellationToken)
    {
        try
        {
            if (authService is null)
            {
                return;
            }
            var authenticationState = await task;
            if (authenticationState.User.Identity?.IsAuthenticated == true)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(settingService.RevalidationInterval, cancellationToken);
                        var isValid = await authService.CheckUserStatusAsync(Current);
                        if (!isValid)
                        {
                            await ClearState();
                            break;
                        }
                    }
                    catch (TaskCanceledException tce)
                    {
                        if (tce.CancellationToken == cancellationToken)
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while revalidating authentication state");
            await ClearState();
        }
    }

    // 构建用户声明的方法，用于创建ClaimsPrincipal对象
    // private static ClaimsPrincipal BuildClaims(UserInfo info)
    // {
    //     // 初始化一个声明列表，包含用户的必要信息
    //     var claims = new List<Claim>
    //     {
    //         new(ClaimTypes.Name, info.UserId),
    //         new(ClaimTypes.GivenName, info.UserName!),
    //         //new(nameof(UserInfo.UserPowers), JsonSerializer.Serialize(info.UserPowers)),
    //         new(nameof(UserInfo.Token), info.Token ?? ""),
    //         new(nameof(UserInfo.CreatedTime), $"{info.CreatedTime.ToBinary()}"),
    //         new(nameof(UserInfo.AdditionalValue), JsonSerializer.Serialize(info.AdditionalValue)),
    //         new(nameof(UserInfo.PasswordHash), info.PasswordHash)
    //     };
    //     // 遍历用户角色，为每个角色添加一个声明
    //     claims.AddRange(info.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
    //
    //     // 遍历用户权限，为每个权限添加一个声明
    //     claims.AddRange(info.UserPowers.Select(p => new Claim("POWER", p)));
    //
    //     // 遍历用户菜单，为每个菜单添加一个声明 
    //     claims.AddRange(info.UserPages.Select(p => new Claim("MENU", p)));
    //
    //     // 返回一个新的ClaimsPrincipal对象，包含所有声明
    //     return new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies"));
    // }
}