
using AutoInjectGenerator;
using AutoWasmApiGenerator;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;

namespace BlazorAdmin.Client.HttpClientHandlers;

//[AutoInjectSelf(Group = "WASM")]
//public class GeneratedApiHandler : DelegatingHandler//AutoWasmApiGenerator.GeneratedApiInvokeDelegatingHandler
//{
//    private readonly ILogger<GeneratedApiHandler> logger;
//    private readonly IUIService ui;

//    public GeneratedApiHandler(ILogger<GeneratedApiHandler> logger, IUIService ui)
//    {
//        this.logger = logger;
//        this.ui = ui;
//    }
//    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//    {
//        var response = await base.SendAsync(request, cancellationToken);
//        var value = await response.Content.ReadAsStringAsync(cancellationToken);

//        //logger.LogInformation("{RequestUri} -> {StatusCode} -> {value}", request.RequestUri, response.StatusCode, value);
//        //if (OperatingSystem.IsBrowser()) { }
//        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
//        {
//            ui.Error("用户过期，请重新登录");
//        }
//        return response;
//    }

//}
[AutoInject(Group = "WASM", ServiceType = typeof(IGeneratedApiInvokeDelegatingHandler))]
public class GeneratedApiHandler(ILogger<GeneratedApiHandler> logger, IUIService ui) : GeneratedApiInvokeDelegatingHandler
{
    public override Task BeforeSendAsync(SendContext context)
    {
        logger.LogDebug("before request {Message}", context.TargetMethod);
        return base.BeforeSendAsync(context);
    }
    public override Task OnExceptionAsync(ExceptionContext context)
    {
        logger.LogDebug("请求发生异常: {Message}", context.Exception.Message);
        if (context.SendContext.Response?.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            context.Handled = true;
            ui.Error("用户过期，请重新登录");
        }
        return Task.CompletedTask;
    }
}