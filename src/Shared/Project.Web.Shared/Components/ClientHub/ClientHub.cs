using Microsoft.JSInterop;

namespace Project.Web.Shared.Components;

public class ClientHub : JsComponentBase
{
    // [CascadingParameter] public Task<AuthenticationState>? AuthenticationState { get; set; }
    private ClientInfo? client;
    [Inject] [NotNull] private IClientService? ClientService { get; set; }

    [Inject] [NotNull] private IUserStore? UserStore { get; set; }
    private new string Id { get; } = Guid.NewGuid().ToString();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var info = await InvokeAsync<string[]>("init", new
            {
                interval = 1000,
                dotnetRef = DotNetObjectReference.Create(this)
            });
            client = new ClientInfo(Id)
            {
                UserStore = UserStore,
                IpAddress = info[0],
                UserAgent = info[1]
            };
            await ClientService.AddOrUpdateAsync(client);
        }
    }

    [JSInvokable("Tick")]
    public async Task SendBeatAsync()
    {
        if (client == null) return;
        await ClientService.AddOrUpdateAsync(client);
    }
}