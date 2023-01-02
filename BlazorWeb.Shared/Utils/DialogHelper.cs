using AntDesign;
using Microsoft.JSInterop;

namespace BlazorWeb.Shared.Utils
{
    public static class DialogHelper
    {
        public static async Task<T> OpenDialog<Template, T>(this ModalService service, ModalOptions options, T? param = default)
            where Template : FeedbackComponent<T, T>
            where T : new()
        {
            TaskCompletionSource<T> tcs = new();
            options.OkText = "确定";
            options.CancelText = "取消";
            var modalRef = await service.CreateModalAsync<Template, T, T>(options, param);

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

        public static Task<T> OpenDialog<Template, T>(this ModalService service, string title, T? param = default, int width = 0)
            where Template : FeedbackComponent<T, T>
            where T : new()
        {
            var options = new ModalOptions();
            options.Title = title;
            if (width > 0) options.Width = width;
            options.DestroyOnClose = true;
            return OpenDialog<Template, T>(service, options, param);
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

        public static async Task OpenNewTab(this IJSRuntime runtime, string url)
        {
            await runtime.InvokeVoidAsync("open", url);
        }
    }
}
