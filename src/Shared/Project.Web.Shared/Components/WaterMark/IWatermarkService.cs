
namespace Project.Web.Shared.Components;

public sealed class WatermarkServiceFactory : IWatermarkServiceFactory
{
    private const string KEY = "DEFAULT";
    readonly Dictionary<string, IWatermarkService> services = [];
    public IWatermarkService GetKeyedWatermarkService(string key)
    {
        return services[key];
    }

    public IWatermarkService GetWatermarkService()
    {
        return GetKeyedWatermarkService(KEY);
    }

    public void RegisterKeyedWatermarkService(string key, IWatermarkService watermarkService)
    {
        services[key] = watermarkService;
    }

    public void RegisterWatermarkService(IWatermarkService watermarkService)
    {
        RegisterKeyedWatermarkService(KEY, watermarkService);
    }
}

public interface IWatermarkServiceFactory
{
    IWatermarkService GetWatermarkService();
    IWatermarkService GetKeyedWatermarkService(string key);
    void RegisterWatermarkService(IWatermarkService watermarkService);
    void RegisterKeyedWatermarkService(string key, IWatermarkService watermarkService);
}

public class WaterMarkOptions
{
    public int Top { get; set; } = 20;
    public (int X, int Y) Gap { get; set; } = (100, 100);
    public int Width { get; set; } = 120;
    public int Height { get; set; } = 64;
    public int Rotate { get; set; } = -22;
    public float Alpha { get; set; } = 1f;
    public int FontSize { get; set; } = 14;
    public string FontColor { get; set; } = "rgba(0,0,0,.15)";
    public int LineSpace { get; set; } = 16;
}
public interface IWatermarkService
{
    Task UpdateWaterMarkAsync(WaterMarkOptions? options, params string?[] contents);
    Task UpdateWaterMarkAsync(params string?[] contents);
}
