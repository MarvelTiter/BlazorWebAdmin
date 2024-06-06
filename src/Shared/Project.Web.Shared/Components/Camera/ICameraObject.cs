using static Project.Web.Shared.Components.Camera;

namespace Project.Web.Shared.Components
{
    public interface ICameraObject
    {
        IEnumerable<DeviceInfo> Devices { get; }
        SelectItem<Resolution> Resolutions { get; }
        Task SwitchCamera(string deviceId, Resolution? resolution = null);
        Task Start(Resolution? resolution);
        Task Start();
        Task Stop();
        Task ToggleClipBoxAsync();
        Task<CaptureInfo> Capture();
    }
}
