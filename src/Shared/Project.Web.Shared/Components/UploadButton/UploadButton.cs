using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Project.Web.Shared.Components;
public class UploadButton : AppComponentBase
{
    [Parameter] public EventCallback<InputFileChangeEventArgs> OnChange { get; set; }
    [Parameter] public string? Accept { get; set; } 
    [Parameter] public string Id { get; set; } = Guid.NewGuid().ToString("N");
    [Parameter] public string? Text { get; set; } 
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        // <label for= "ExcelHandle_Import">
        //   <InputFile id = "ExcelHandle_Import" hidden OnChange = "@ImportExcelAsync" accept = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ></InputFile >
        //    @UI.BuildFakeButton(new() { ButtonType = Project.Constraints.UI.ButtonType.Primary, Text = "导入Excel" })
        //</label>
        builder.OpenElement(0, "label");
        builder.AddAttribute(1, "for", Id);
        builder.AddContent(2, child =>
        {
            child.OpenComponent<InputFile>(0);
            child.AddAttribute(1, "id", Id);
            child.AddAttribute(2, "hidden", true);
            child.AddAttribute(3, "OnChange", OnChange);
            if (!string.IsNullOrEmpty(Accept))
            {
                child.AddAttribute(4, "accept", Accept);
            }
            child.CloseComponent();
            child.AddContent(5, UI.BuildFakeButton(new() { ButtonType = Project.Constraints.UI.ButtonType.Primary, Text = Text }));
        });
        builder.CloseElement();
    }
}
