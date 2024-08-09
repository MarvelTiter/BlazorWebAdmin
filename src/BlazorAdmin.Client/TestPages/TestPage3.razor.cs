using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace BlazorAdmin.Client.TestPages
{
#if DEBUG
    [Route("/test3")]
    [PageInfo(Id = "TestPage3", Title = "测试3", GroupId = "test")]
#endif
    public partial class TestPage3
    {
        [Inject, NotNull] IUIService? UI { get; set; }
        class RefInt()
        {
            public int Value { get; set; }
        }
        async Task OpenDialog()
        {
            var refInt = new RefInt();
            refInt.Value = 1;
            await UI.ShowDialogAsync(val => builder =>
            {
                builder.Div().AddContent(b =>
                {
                    b.AddContent(0, "要大于10");
                    b.AddContent(1, UI.BuildInput<int>(this).Bind(() => val!.Value).Render());
                }).Build();
            }
               , refInt, false, config =>
               {
                   config.Title = "测试PostCheck";
                   config.PostCheck = (v, validate) =>
                   {
                       return v!.Value > 10;
                   };
               });
        }
    }
}
