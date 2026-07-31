using Microsoft.AspNetCore.Components;
using BlazorTemplate.Constraints.Common.Attributes;
using BlazorTemplate.Constraints.Page;

namespace BlazorAdmin.Client.TestPages;
#if DEBUG
[Route("/test-form-builder")]
[PageInfo(Title = "表单构建测试", Icon = "fa fa-question-circle-o", GroupId = "test")]
#endif
public partial class FormBuilderTest : AppComponentBase
{
    private class TestFormModel
    {
        public int IntValue { get; set; }
        public string StringValue { get; set; } = string.Empty;
        public DateTime DateValue { get; set; } = DateTime.Now;
    }

    private TestFormModel model = new();
}