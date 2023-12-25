using Microsoft.JSInterop;

namespace Project.Web.Shared.Utils
{
    public static class FileDownloadHelper
    {
        public static async Task PushAsync(this IJSRuntime js, string path)
        {
            if (GetFileStream(path, out var fileStream))
            {
                var fileName = Path.GetFileName(path);
                using var streamRef = new DotNetStreamReference(stream: fileStream);
                await js.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
            }
        }

        private static bool GetFileStream(string path, out Stream? stream)
        {
            var fileExist = File.Exists(path);
            stream = null;
            if (fileExist)
            {
                stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            return fileExist;
        }
    }
}
