using AntDesign;
using Microsoft.JSInterop;

namespace BlazorWebAdmin.Utils
{
    public static class DialogHelper
    {
        public static async Task<T> OpenDialog<Template, T>(this ModalService service, ModalOptions options, T param) where Template : FeedbackComponent<T, T>
        {
            TaskCompletionSource<T> tcs = new();
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

        public static Task<T> OpenDialog<Template, T>(this ModalService service, string title, T param) where Template : FeedbackComponent<T, T>
        {
            var options = new ModalOptions();
            options.Title = title;
            options.DestroyOnClose = true;
            return OpenDialog<Template, T>(service, options, param);
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
