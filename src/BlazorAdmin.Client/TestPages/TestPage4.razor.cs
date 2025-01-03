using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Web.Shared.Layouts;
namespace BlazorAdmin.Client.TestPages
{
#if DEBUG
    [Route("/test4")]
    [PageInfo(Title = "测试4", GroupId = "test")]
    //[Layout(typeof(NotAuthorizedLayout))]
#endif
    public partial class TestPage4
    {
    }
}
