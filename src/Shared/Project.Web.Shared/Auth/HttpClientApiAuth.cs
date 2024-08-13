using AutoInjectGenerator;
using AutoWasmApiGenerator;
using Project.Constraints.Store;

namespace Project.Web.Shared.Auth;

[AutoInject(Group = "WASM")]
public class HttpClientApiAuth : IHttpClientHeaderHandler
{
    private readonly IUserStore userStore;

    public HttpClientApiAuth(IUserStore userStore)
    {
        this.userStore = userStore;
    }
    public void SetRequestHeader(HttpRequestMessage request)
    {
        Console.WriteLine($"SetRequestHeader: {userStore.UserId}");
        // request.Headers.Add();
    }
}