namespace Project.Constraints.PageHelper;

public interface IPageAction
{
    Task OnShowAsync();
    Task OnHiddenAsync();
}
