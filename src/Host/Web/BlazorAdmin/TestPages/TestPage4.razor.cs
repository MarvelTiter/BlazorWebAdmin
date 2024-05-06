using Project.Constraints.Common.Attributes;
using Microsoft.AspNetCore.Components;
using Project.AppCore.Layouts;
namespace BlazorAdmin.TestPages
{
#if DEBUG
    [Route("/test4")]
    [Layout(typeof(AuthorizedLayout))]
#endif
    public partial class TestPage4
    {
    }
}
