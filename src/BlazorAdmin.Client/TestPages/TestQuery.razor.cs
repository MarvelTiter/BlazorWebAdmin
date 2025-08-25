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