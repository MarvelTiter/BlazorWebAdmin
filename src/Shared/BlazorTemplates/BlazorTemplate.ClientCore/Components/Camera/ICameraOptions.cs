using static BlazorTemplate.ClientCore.Components.Camera;
#pragma warning disable IDE0130 
namespace BlazorTemplate.ClientCore.Components;

public interface ICameraOptions
{
    IEnumerable<Resolution> Resolutions { get; }
    SelectItem<Resolution> ResolutionOptions { get; }
}
