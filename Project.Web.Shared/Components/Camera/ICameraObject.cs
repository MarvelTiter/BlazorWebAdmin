using static BlazorWeb.Shared.Components.Camera;

namespace BlazorWeb.Shared.Components
{
    public interface ICameraObject
    {
        IEnumerable<DeviceInfo> Devices { get; }
        Task SwitchCamera(string deviceId, Resolution? resolution = null);
        Task Start(Resolution? resolution);
        Task Stop();
        Task<CaptureInfo> Capture();
    }
}
