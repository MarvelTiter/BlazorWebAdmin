using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;

namespace Demo.Web.TestPages
{
#if DEBUG
    [Route("/test")]
    [PageGroup("test", "测试", 5)]
    [PageInfo(Id = "TestPage", Title = "测试")]
#endif
    public partial class TestPage
    {
    }
}
