using static Project.Web.Shared.Components.Camera;
#pragma warning disable IDE0130 
namespace Project.Web.Shared.Components;

public interface ICameraOptions
{
    IEnumerable<Resolution> Resolutions { get; }
    SelectItem<Resolution> ResolutionOptions { get; }
}
