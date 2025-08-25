using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Layouts;
using System.Diagnostics.CodeAnalysis;

namespace BlazorAdmin.Client.TestPages;
#if DEBUG
[Route("/test3")]
[PageInfo(Title = "StackPanel测试", Icon = "fa fa-question-circle-o", GroupId = "test")]
//[Layout(typeof(NotAuthorizedLayout))]
#endif
public partial class TestStackPanel
{
    [Inject, NotNull] IUIService? UI { get; set; }
    class RefInt(int value)
    {
        public int Value { get; set; } = value;
    }
    async Task OpenDialog()
    {
        RefInt refInt = new(1);
        await UI.ShowDialogAsync(val => builder =>
            {
                builder.Div().AddContent(b =>
                {
                    b.AddContent(0, "要大于10");
                    b.AddContent(1, UI.BuildNumberInput<int>(this).Bind(() => val!.Value).Render());
                }).Build();
            }
            , refInt, false, config =>
            {
                config.Title = "测试PostCheck";
                config.PostCheckAsync = (val, validate) => Task.FromResult(val!.Value > 10);
            });
    }
}