using BlazorTemplate.ClientCore.Common.Attributes;
using Microsoft.AspNetCore.Components;

namespace BlazorAdmin.Client.TestPages;

[Route("/test")]
[PageGroup("LayoutTest", "布局测试", Icon = "fa fa-question-circle-o", GroupId = "test")]
[PageInfo(Title = "Grid布局测试", Icon = "fa fa-question-circle-o", GroupId = "LayoutTest")]
[AutoLoadJsModule(Path = "TestPages")]
public partial class TestGrid
{
}
