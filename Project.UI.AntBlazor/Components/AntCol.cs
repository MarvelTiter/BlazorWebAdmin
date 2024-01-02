using AntDesign;
using Microsoft.AspNetCore.Components;

namespace Project.UI.AntBlazor.Components
{
    public class AntCol : GridCol
    {
#pragma warning disable BL0007 // Component parameters should be auto properties
        [Parameter] public int ColSpan { get => base.Span.AsT1; set => base.Span = value; }
#pragma warning restore BL0007 // Component parameters should be auto properties
    }
}
