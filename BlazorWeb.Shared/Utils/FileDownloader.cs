using Microsoft.JSInterop;

namespace BlazorWeb.Shared.Utils
{
    public static class FileDownloadHelper
    {
        public static async Task PushAsync(this IJSRuntime js, string path)
        {
            var fileStream = GetFileStream(path);
            var fileName = Path.GetFileName(path);
            using var streamRef = new DotNetStreamReference(stream: fileStream);
            await js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }

        private static Stream GetFileStream(string path)
        {
            return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
