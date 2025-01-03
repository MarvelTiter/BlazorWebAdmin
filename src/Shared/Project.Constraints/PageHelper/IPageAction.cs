namespace Project.Constraints.PageHelper;

public interface IPageAction
{
    //RenderFragment GetTitle();
    Task OnShowAsync();
    Task OnHiddenAsync();
}
