namespace BlazorWeb.Shared
{
    public interface IDownloadService
    {
        Task DownloadAsync(string filename);
    }
}
