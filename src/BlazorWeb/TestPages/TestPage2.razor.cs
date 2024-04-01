using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;

namespace Demo.Web.TestPages
{
#if DEBUG
    [Route("/test2")]
    [PageInfo(Id = "TestPage2", Title = "测试2", GroupId = "test")]
#endif
    public partial class TestPage2
    {
    }
}
