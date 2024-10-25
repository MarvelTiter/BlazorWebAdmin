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
    private readonly IAppSession appSession;
    // 身份验证状态任务，初始为未认证状态
    private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;
    // 日志记录接口，用于记录身份验证信息
    private readonly ILogger<IAuthenticationStateProvider> logger;

    // 构造函数，初始化PersistentAuthenticationStateProvider
    public PersistentAuthenticationStateProvider(PersistentComponentState state
        , IAppSession appSession
        , ILogger<IAuthenticationStateProvider> logger)
    {
        this.appSession = appSession;
        this.logger = logger;

        // 从持久化状态中获取用户信息，如果存在，则设置用户信息并更新认证状态
        if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var u) || u is null) return;
        UserStore.SetUser(u);
        authenticationStateTask = Task.FromResult(new AuthenticationState(BuildClaims(u)));
    }

    // 用户存储属性，用于获取当前用户信息
    private IUserStore UserStore => appSession.UserStore;

    // 清除认证状态的方法，包括清除用户信息和执行登出重定向
    public Task ClearState()
    {
        //appSession.UserStore.ClearUser();
        //await httpContextAccessor.HttpContext.SignOutAsync();
        var redirect = appSession.Navigator.ToBaseRelativePath(appSession.Navigator.Uri);
        appSession.Navigator.NavigateTo($"/api/account/logout?Redirect={HttpUtility.UrlEncode(redirect)}", true);
        return Task.CompletedTask;
    }

    // 获取当前用户信息的方法
    public UserInfo? Current => UserStore.UserInfo;

    // 获取当前认证状态的方法
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return authenticationStateTask;
    }

    // 构建用户声明的方法，用于创建ClaimsPrincipal对象
    private static ClaimsPrincipal BuildClaims(UserInfo info)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, info.UserId),
            new(ClaimTypes.GivenName, info.UserName!),
            new(nameof(UserInfo.Token), info.Token)
        };
        claims.AddRange(info.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(PersistentAuthenticationStateProvider)));
    }
}
