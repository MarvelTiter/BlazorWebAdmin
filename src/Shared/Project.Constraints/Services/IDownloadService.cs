namespace Project.Constraints.Services
{
    public interface IDownloadService
    {
        Task DownloadAsync(string filename, params string[] paths);
    }
}
