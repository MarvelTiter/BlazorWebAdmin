using static BlazorTemplate.UI.Shared.Components.Camera;
#pragma warning disable IDE0130 
namespace BlazorTemplate.UI.Shared.Components;

public interface ICameraOptions
{
    IEnumerable<Resolution> Resolutions { get; }
    SelectItem<Resolution> ResolutionOptions { get; }
}
