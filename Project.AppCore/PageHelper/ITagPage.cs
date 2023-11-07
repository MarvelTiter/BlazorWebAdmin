namespace Project.AppCore.PageHelper
{
    public interface ITagPage
    {
        Task OnShowAsync();
        Task OnHiddenAsync();
    }
}
