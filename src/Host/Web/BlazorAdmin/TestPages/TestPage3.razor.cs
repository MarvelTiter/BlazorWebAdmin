using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Constraints.UI;

namespace BlazorAdmin.TestPages
{
#if DEBUG
    [Route("/test3")]
    [PageInfo(Id = "TestPage3", Title = "测试3", GroupId = "test")]
#endif
    public partial class TestPage3
    {
        [Inject] IUIService UI { get; set; }
    }
}
