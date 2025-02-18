using Microsoft.Extensions.Options;
using Project.Constraints.Options;
using static Project.Web.Shared.Components.Camera;

namespace Project.Web.Shared.Components;

[AutoInject(LifeTime = InjectLifeTime.Scoped)]
public class CameraOptions : ICameraOptions, IDisposable
{
    //private Lazy<IEnumerable<Resolution>> lazyResolutions;
    private List<Resolution>? resolutions;
    public IEnumerable<Resolution> Resolutions => resolutions ?? [];

    //private Lazy<SelectItem<Resolution>> lazyResolutionOptions;
    private SelectItem<Resolution>? resolutionOptions;
    private bool disposedValue;
    private readonly IOptionsMonitor<AppSetting> options;
    private readonly IDisposable? listener;

    public SelectItem<Resolution> ResolutionOptions => resolutionOptions ?? [];
    public CameraOptions(IOptionsMonitor<AppSetting> options)
    {
        this.options = options;
        UpdateCameraResolutions();
        listener = options.OnChange(OptionChanged);
    }
    private void OptionChanged(AppSetting newSetting)
    {
        UpdateCameraResolutions();
    }
    private void UpdateCameraResolutions()
    {
        resolutionOptions ??= [];
        resolutionOptions.Clear();
        resolutions ??= [];
        resolutions.Clear();

        AddResolution(Resolution.QVGA);
        AddResolution(Resolution.VGA);
        AddResolution(Resolution.HD);
        AddResolution(Resolution.FullHD);
        AddResolution(Resolution.Television4K);
        AddResolution(Resolution.Cinema4K);
        AddResolution(Resolution.A4);

        TryAddCustomResolution(options.CurrentValue.CameraResolutions ?? []);
    }
    private void AddResolution(Resolution item)
    {
        resolutionOptions!.Add(item.Name, item);
        resolutions!.Add(item);
    }

    private void TryAddCustomResolution(IEnumerable<CameraResolution> customs)
    {
        foreach (var item in customs)
        {
            AddResolution(item);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                listener?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
