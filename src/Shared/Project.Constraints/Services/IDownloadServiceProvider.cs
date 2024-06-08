namespace Project.Constraints.Services
{
    public interface IDownloadServiceProvider
    {
        IDownloadService? GetService();
        void Register(IDownloadService service);
    }

    public class DownloadServiceProvider : IDownloadServiceProvider
    {
        private IDownloadService? service;
        public IDownloadService? GetService() => service;

        public void Register(IDownloadService service)
        {
            this.service = service;
        }
    }
}
