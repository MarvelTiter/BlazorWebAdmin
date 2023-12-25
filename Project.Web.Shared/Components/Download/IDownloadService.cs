namespace Project.Web.Shared
{
    public interface IDownloadService
    {
        Task DownloadAsync(string filename);
    }
}
