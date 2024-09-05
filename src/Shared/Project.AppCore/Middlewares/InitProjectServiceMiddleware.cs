// using Microsoft.AspNetCore.Http;
// using Project.Constraints;
// using System;
// using System.Collections.Generic;
// using System.Diagnostics.CodeAnalysis;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
//
// namespace Project.AppCore.Middlewares
// {
//     public class InitProjectServiceMiddleware : IMiddleware, IAsyncDisposable
//     {
//         private readonly IServiceProvider services;
//         private IAppSession? appSession;
//         private IProjectSettingService? project;
//         readonly List<IAddtionalInterceptor> initActions = [];
//         public InitProjectServiceMiddleware(IServiceProvider services)
//         {
//             this.services = services;
//         }
//
//         public Task InvokeAsync(HttpContext context, RequestDelegate next)
//         {
//             appSession = services.GetService<IAppSession>()!;
//             project = services.GetService<IProjectSettingService>()!;
//             appSession.RouterStore.RouterChangingEvent += project.RouterChangingAsync;
//             appSession.RouterStore.RouteMetaFilterEvent += project.RouteMetaFilterAsync;
//             appSession.UserStore.LoginSuccessEvent += project.LoginSuccessAsync;
//             appSession.WebApplicationAccessedEvent += project.AfterWebApplicationAccessed;
//
//             var interceptors = services.GetServices<IAddtionalInterceptor>();
//
//             foreach (var additional in interceptors)
//             {
//                 initActions.Add(additional);
//                 appSession.RouterStore.RouterChangingEvent += additional.RouterChangingAsync;
//                 appSession.RouterStore.RouteMetaFilterEvent += additional.RouteMetaFilterAsync;
//                 appSession.UserStore.LoginSuccessEvent += additional.LoginSuccessAsync;
//                 appSession.WebApplicationAccessedEvent += additional.AfterWebApplicationAccessedAsync;
//             }
//
//             return next(context);
//         }
//
//         public ValueTask DisposeAsync()
//         {
//             if (appSession != null && project != null)
//             {
//                 appSession.RouterStore.RouterChangingEvent -= project.RouterChangingAsync;
//                 appSession.RouterStore.RouteMetaFilterEvent -= project.RouteMetaFilterAsync;
//                 appSession.UserStore.LoginSuccessEvent -= project.LoginSuccessAsync;
//                 appSession.WebApplicationAccessedEvent -= project.AfterWebApplicationAccessed;
//             }
//
//             foreach (var additional in initActions)
//             {
//                 if (appSession == null) break;
//                 appSession.UserStore.LoginSuccessEvent -= additional.LoginSuccessAsync;
//                 appSession.RouterStore.RouterChangingEvent -= additional.RouterChangingAsync;
//                 appSession.WebApplicationAccessedEvent -= additional.AfterWebApplicationAccessedAsync;
//             }
//             GC.SuppressFinalize(this);
//             return ValueTask.CompletedTask;
//         }
//     }
// }
