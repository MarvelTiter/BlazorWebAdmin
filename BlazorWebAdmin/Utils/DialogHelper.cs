using AntDesign;

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
    }
}
