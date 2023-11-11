namespace Project.AppCore.PageHelper
{
    public interface IPageAction
    {
        Task OnShowAsync();
        Task OnHiddenAsync();
    }
}
