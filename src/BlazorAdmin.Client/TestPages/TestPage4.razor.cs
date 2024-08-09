using Microsoft.AspNetCore.Components;
using Project.Web.Shared.Layouts;
namespace BlazorAdmin.Client.TestPages
{
#if DEBUG
    [Route("/test4")]
    [Layout(typeof(NotAuthorizedLayout))]
#endif
    public partial class TestPage4
    {
    }
}
