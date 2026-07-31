using static BlazorTemplate.UI.Shared.Components.Camera;
#pragma warning disable IDE0130 
namespace BlazorTemplate.UI.Shared.Components;

public interface ICameraObject
{
    IEnumerable<DeviceInfo> Devices { get; }
    RenderFragment CameraView { get; }
    RenderFragment CameraActionsView { get; }
    //SelectItem<Resolution> Resolutions { get; }
    ICameraOptions CameraOptions { get; }
    Task SwitchCamera(string deviceId, Resolution? resolution = null, bool? exact = null);
    Task Start(Resolution? resolution, bool? exact = null);
    Task Start();
    Task Stop();
    Task ToggleClipBoxAsync();
    Task<CaptureInfo> Capture();
}
