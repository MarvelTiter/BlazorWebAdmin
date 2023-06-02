using AntDesign;
using BlazorWeb.Shared.Template.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Project.AppCore;

namespace BlazorWeb.Shared.Utils
{
    public static class DialogHelper
    {
        static IStringLocalizer<ModalOptions> GetLocalizer() => ServiceLocator.Instance.GetService<IStringLocalizer<ModalOptions>>();
     
        static FormParam<T> CreateParam<T>(T? value, bool? edit)
        {
            if (edit.HasValue) return new FormParam<T>(value, edit.Value);
            else return new FormParam<T>(value);
        }

        public static async Task<T> OpenDialog<Template, T>(this ModalService service, ModalOptions options, T? param, bool? edit = null)
            where Template : FeedbackComponent<FormParam<T>, T>
            where T : new()
        {
            TaskCompletionSource<T> tcs = new();
            var localizer = GetLocalizer();
            options.OkText = localizer["CustomButtons.Ok"].Value;
            options.CancelText = localizer["CustomButtons.Cancel"].Value;
            var p = CreateParam(param, edit);
            var modalRef = await service.CreateModalAsync<Template, FormParam<T>, T>(options, p);
            
            modalRef.OnOk = async (result) =>
            {
                await Task.Delay(1);
                tcs.SetResult(result);
            };

            modalRef.OnCancel = async (result) =>
            {
                await Task.Delay(1);
                tcs.SetCanceled();
            };

            return await tcs.Task;
        }

        public static Task<T> OpenDialog<Template, T>(this ModalService service, string title, T? param = default, bool? edit = null, int width = 0)
            where Template : FeedbackComponent<FormParam<T>, T>
            where T : new()
        {
            var options = new ModalOptions();
            options.Title = title;
            if (width > 0) options.Width = width;
            options.DestroyOnClose = true;
            return OpenDialog<Template, T>(service, options, param, edit);
        }

        public static async Task<T> OpenDrawer<Template, T>(this DrawerService service, DrawerOptions options, T? param = default)
            where Template : FeedbackComponent<T, T>
            where T : new()
        {
            TaskCompletionSource<T> tcs = new();
            var drawerRef = await service.CreateAsync<Template, T, T>(options, param);

            drawerRef.OnClosed = (e) =>
            {
                tcs.SetResult(e);
                return Task.CompletedTask;
            };
            return await tcs.Task;
        }

        public static Task<T> OpenDrawer<Template, T>(this DrawerService service, string title, T? param = default, int width = 0)
           where Template : FeedbackComponent<T, T>
           where T : new()
        {
            var options = new DrawerOptions();
            options.Title = title;
            if (width > 0) options.Width = width;
            return OpenDrawer<Template, T>(service, options, param);
        }

        public static async Task OpenDialogView<Template, T>(this ModalService service, ModalOptions options, T param) where Template : FeedbackComponent<T>
        {
            options.DestroyOnClose = true;
            var modalRef = await service.CreateModalAsync<Template, T>(options, param);
        }

        public static Task OpenDialogView<Template, T>(this ModalService service, string title, T param) where Template : FeedbackComponent<T>
        {
            var options = new ModalOptions();
            options.Title = title;
            options.DestroyOnClose = true;
            return OpenDialogView<Template, T>(service, options, param);
        }
    }
}
