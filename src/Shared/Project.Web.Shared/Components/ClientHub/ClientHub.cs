using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Project.Constraints.Options;

namespace Project.Web.Shared.Components;

public class ClientHub : JsComponentBase
{
    // [CascadingParameter] public Task<AuthenticationState>? AuthenticationState { get; set; }
    private ClientInfo? client;
    [Inject] [NotNull] private IClientService? ClientService { get; set; }

    [Inject] [NotNull] private IUserStore? UserStore { get; set; }
    [Inject, NotNull] private IOptions<AppSetting>? Options { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await InvokeAsync<string[]>("init", new
            {
                interval = Options.Value.ClientHubOptions.ClientSendFrequency.TotalMilliseconds,
                dotnetRef = DotNetObjectReference.Create(this)
            });
            //client = new ClientInfo(Id)
            //{
            //    UserStore = UserStore,
            //    IpAddress = info[0],
            //    UserAgent = info[1]
            //};
        }
    }

    [JSInvokable("Tick")]
    public async Task SendBeatAsync(string[] info)
    {
        client ??= new ClientInfo()
        {
            CircuitId = info[0],
            UserInfo = UserStore.UserInfo,
            IpAddress = info[1],
            UserAgent = info[2]
        };
        await ClientService.AddOrUpdateAsync(client);
    }
}