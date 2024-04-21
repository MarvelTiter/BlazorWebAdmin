using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Constraints.UI;

namespace BlazorWinFormsAdmin.TestPages
{
#if DEBUG
    [Route("/test2")]
    [PageInfo(Id = "TestPage2", Title = "测试2", GroupId = "test")]
#endif
    public partial class TestPage2
    {
        [Inject] IUIService UI { get; set; }
    }
}
