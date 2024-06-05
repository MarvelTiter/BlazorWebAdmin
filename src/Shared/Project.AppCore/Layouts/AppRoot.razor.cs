using AspectCore.DynamicProxy;
using Microsoft.AspNetCore.Components;
using Project.AppCore.Store;
using Project.Constraints;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Project.AppCore.Layouts
{
    public partial class AppRoot : IAsyncDisposable
    {
        [Parameter, NotNull] public Assembly AppAssembly { get; set; }
        [Inject, NotNull] IAppSession? Context { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Inject, NotNull] IServiceProvider? Services { get; set; }
        [Inject, NotNull] IProjectSettingService? SettingService { get; set; }

        readonly List<IAddtionalInterceptor> initActions = [];
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Context.RouterStore.RouterChangingEvent += SettingService.RouterChangingAsync;
            Context.RouterStore.RouteMetaFilterEvent += SettingService.RouteMetaFilterAsync;
            Context.UserStore.LoginSuccessEvent += SettingService.LoginSuccessAsync;
            Context.WebApplicationAccessedEvent += SettingService.AfterWebApplicationAccessed;

            var interceptors = Services.GetServices<IAddtionalInterceptor>();

            foreach (var additional in interceptors)
            {
                initActions.Add(additional);
                Context.RouterStore.RouterChangingEvent += additional.RouterChangingAsync;
                Context.RouterStore.RouteMetaFilterEvent += additional.RouteMetaFilterAsync;
                Context.UserStore.LoginSuccessEvent += additional.LoginSuccessAsync;
                Context.WebApplicationAccessedEvent += additional.AfterWebApplicationAccessedAsync;
            }
        }

        public ValueTask DisposeAsync()
        {
            Context.RouterStore.RouterChangingEvent -= SettingService.RouterChangingAsync;
            Context.RouterStore.RouteMetaFilterEvent -= SettingService.RouteMetaFilterAsync;
            Context.UserStore.LoginSuccessEvent -= SettingService.LoginSuccessAsync;
            Context.WebApplicationAccessedEvent -= SettingService.AfterWebApplicationAccessed;

            foreach (var additional in initActions)
            {
                Context.UserStore.LoginSuccessEvent -= additional.LoginSuccessAsync;
                Context.RouterStore.RouteMetaFilterEvent -= additional.RouteMetaFilterAsync;
                Context.RouterStore.RouterChangingEvent -= additional.RouterChangingAsync;
                Context.WebApplicationAccessedEvent -= additional.AfterWebApplicationAccessedAsync;
            }

            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }
}
