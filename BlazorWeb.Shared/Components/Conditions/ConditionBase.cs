using Microsoft.AspNetCore.Components;

namespace BlazorWeb.Shared.Components
{
    public class ConditionBase: ComponentBase, ICondition
    {
        [CascadingParameter] public IQueryCondition Parent { get; set; }
        [Parameter] public string? Label { get; set; }
        [Parameter] public DateType? DateConfig { get; set; }
        [Parameter] public string? Style { get; set; }
    }
}
