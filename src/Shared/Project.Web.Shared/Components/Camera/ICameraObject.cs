using static Project.Web.Shared.Components.Camera;
#pragma warning disable IDE0130 
namespace Project.Web.Shared.Components;

public interface ICameraObject
{
    IEnumerable<DeviceInfo> Devices { get; }
    RenderFragment CameraView { get; }
    RenderFragment CameraActionsView { get; }
    //SelectItem<Resolution> Resolutions { get; }
    Task SwitchCamera(string deviceId, Resolution? resolution = null);
    Task Start(Resolution? resolution);
    Task Start();
    Task Stop();
    Task ToggleClipBoxAsync();
    Task<CaptureInfo> Capture();
}
