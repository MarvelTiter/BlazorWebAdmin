using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Project.Constraints;
using Project.Web.Shared.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Project.Web.Shared.Layouts
{
    public partial class AppRoot : IAsyncDisposable
    {
        [Parameter, NotNull] public Assembly? AppAssembly { get; set; }
        [Inject, NotNull] IAppSession? Context { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Inject, NotNull] IServiceProvider? Services { get; set; }
        [Inject, NotNull] IProjectSettingService? SettingService { get; set; }
        [Inject, NotNull] IJSRuntime? Js { get; set; }

        readonly List<IAddtionalInterceptor> initActions = [];
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Context.RouterStore.RouterChangingEvent += SettingService.RouterChangingAsync;
            Context.RouterStore.RouteMetaFilterEvent += SettingService.RouteMetaFilterAsync;
            Context.LoginSuccessEvent += SettingService.LoginSuccessAsync;
            Context.WebApplicationAccessedEvent += SettingService.AfterWebApplicationAccessed;

            var interceptors = Services.GetServices<IAddtionalInterceptor>();

            foreach (var additional in interceptors)
            {
                initActions.Add(additional);
                Context.RouterStore.RouterChangingEvent += additional.RouterChangingAsync;
                Context.RouterStore.RouteMetaFilterEvent += additional.RouteMetaFilterAsync;
                Context.LoginSuccessEvent += additional.LoginSuccessAsync;
                Context.WebApplicationAccessedEvent += additional.AfterWebApplicationAccessedAsync;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                var c = await Js.InvokeUtilsAsync<string[]>("getClient");
                Context.UserStore.Ip = c[0];
                Context.UserStore.UserAgent = c[1];
                await Context.NotifyWebApplicationAccessedAsync();
            }
        }

        public ValueTask DisposeAsync()
        {
            Context.RouterStore.RouterChangingEvent -= SettingService.RouterChangingAsync;
            Context.RouterStore.RouteMetaFilterEvent -= SettingService.RouteMetaFilterAsync;
            Context.LoginSuccessEvent -= SettingService.LoginSuccessAsync;
            Context.WebApplicationAccessedEvent -= SettingService.AfterWebApplicationAccessed;

            foreach (var additional in initActions)
            {
                Context.LoginSuccessEvent -= additional.LoginSuccessAsync;
                Context.RouterStore.RouteMetaFilterEvent -= additional.RouteMetaFilterAsync;
                Context.RouterStore.RouterChangingEvent -= additional.RouterChangingAsync;
                Context.WebApplicationAccessedEvent -= additional.AfterWebApplicationAccessedAsync;
            }

            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }

    }
}
