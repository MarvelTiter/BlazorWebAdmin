using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Web.Shared.Layouts;
namespace BlazorAdmin.Client.TestPages
{
#if DEBUG
    [Route("/test4")]
    [PageInfo(Id = "TestPage4", Title = "测试4", GroupId = "test")]
    //[Layout(typeof(NotAuthorizedLayout))]
#endif
    public partial class TestPage4
    {
    }
}
