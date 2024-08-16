namespace Project.Web.Shared.Utils;

public static class DownloadExtensions
{
    public static Task DownloadAsync(this IDownloadService service, Func<Task>? serverSideDownload,
        Func<Task>? clientSideDownload)
    {
        if (OperatingSystem.IsBrowser())
        {
            if (clientSideDownload == null)
                throw new NotSupportedException("Not Provide a client side download handler");
            return clientSideDownload();
        }

        if (serverSideDownload == null)
            throw new NotSupportedException("Not Provide a server side download handler");
        return serverSideDownload();
    }
}