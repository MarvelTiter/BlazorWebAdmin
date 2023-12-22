using Microsoft.AspNetCore.Components;

namespace BlazorWeb.Shared.Components
{
    public interface IExceptionHandler
    {
        Task HandleExceptionAsync(Exception exception);
    }
    public interface IError
    {
        //event Func<Exception, Task> OnHandleExcetionAsync;
        //event Action<Exception> OnHandleExcetion;
        void Register<TComponent>(TComponent component) where TComponent : IComponent, IExceptionHandler;
        void UnRegister<TComponent>(TComponent component) where TComponent : IComponent, IExceptionHandler;
    }
}
