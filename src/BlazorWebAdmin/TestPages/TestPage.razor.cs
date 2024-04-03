using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;

namespace BlazorWebAdmin.TestPages
{
    [Route("/test")]
    [PageGroup("test", "测试", 5)]
    [PageInfo(Id = "TestPage", Title = "测试")]
    public partial class TestPage
    {
    }
}
