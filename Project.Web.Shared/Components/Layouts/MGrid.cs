using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Project.Web.Shared.Components.Layouts
{
	public static class GridHelper
	{
		//public static GridColumn Repeat(int count, double percent)
		//{

		//}
	}

	public class GridColumn
	{
        public int Mode { get; set; }

    }

	public class MGrid : ComponentBase
	{
		[Parameter] public int GridColumns { get; set; } = 1;
		[Parameter] public RenderFragment ChildContent { get; set; }
		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<CascadingValue<MGrid>>(0);
			builder.AddAttribute(1, nameof(CascadingValue<MGrid>.Value), this);
			builder.OpenElement(2, "div");
			builder.AddAttribute(3, "style", $"display:grid; grid-template-columns:repeat({GridColumns}, 1fr);");
			builder.AddContent(4, ChildContent);
			builder.CloseElement();
			builder.CloseComponent();
		}
	}
}
