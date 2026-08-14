namespace BlazorTemplate.ClientCore.Services;

public interface IDownloadService
{
    Task DownloadFileAsync(string filename, params string[] paths);
    Task DownloadStreamAsync(string filename, Stream stream);
}