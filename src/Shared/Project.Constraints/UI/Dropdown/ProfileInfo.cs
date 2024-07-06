using Microsoft.AspNetCore.Components;

namespace Project.Constraints.UI.Dropdown;

public class ProfileInfo
{
    public RenderFragment? Content { get; set; }
    public string? UserName { get; set; }
    public List<ActionInfo>? Actions { get; set; }
}