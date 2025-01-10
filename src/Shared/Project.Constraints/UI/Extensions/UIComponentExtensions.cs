using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Builders;
using Project.Constraints.UI.Flyout;
using Project.Constraints.UI.Props;

namespace Project.Constraints.UI.Extensions;

public static class UIComponentExtensions
{
    #region Tabs
    public static IUIComponent<TabsProp> AddTab(this IUIComponent<TabsProp> tab, string title, RenderFragment content)
    {
        tab.Model.TabContents.Add(new()
        {
            Title = title,
            Content = content
        });
        return tab;
    }
    public static IUIComponent<TabsProp> AddTab<T>(this IUIComponent<TabsProp> tab, string title)
        where T : IComponent
    {
        tab.Model.TabContents.Add(new()
        {
            Title = title,
            Content = b => b.Component<T>().Build()
        });
        return tab;
    }
    #endregion

    #region Popover

    public static IUIComponent<PopoverOptions> SetTrigger(this IUIComponent<PopoverOptions> pop, RenderFragment trigger)
    {
        pop.Model.Trigger = trigger;
        return pop;
    }

    public static IUIComponent<PopoverOptions> SetPopContent(this IUIComponent<PopoverOptions> pop , RenderFragment content)
    {
        pop.Model.Content = content;
        return pop;
    }

    #endregion
}
