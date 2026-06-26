using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Web.Shared.Layouts;

namespace BlazorAdmin.Client.TestPages;
#if DEBUG
[Route("/test4")]
[PageInfo(Title = "Query测试", Icon = "fa fa-question-circle-o", GroupId = "test")]
//[Layout(typeof(NotAuthorizedLayout))]
#endif
public partial class TestQuery
{
}

#if DEBUG
[Route("/test-auth")]
[PageInfo(Title = "权限页测试", Icon = "fa fa-question-circle-o", GroupId = "test")]
//[Layout(typeof(NotAuthorizedLayout))]
[Authorize(Roles = "NOOOOO")]
#endif
public partial class TestAuth : ComponentBase
{

}
