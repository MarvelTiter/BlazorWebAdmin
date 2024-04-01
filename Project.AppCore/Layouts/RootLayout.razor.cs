using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Project.AppCore.Routers;
using Project.AppCore.Store;
using Project.Constraints;
using Project.Constraints.Store;
using Project.Constraints.UI;

namespace Project.AppCore.Layouts
{
    public partial class RootLayout : IDomEventHandler, IDisposable
    {
        public event Func<MouseEventArgs, Task> BodyClickEvent;
        public event Func<KeyboardEventArgs, Task> OnKeyDown;
        public event Func<KeyboardEventArgs, Task> OnKeyUp;
        protected ElementReference? RootWrapper { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if (RootWrapper.HasValue)
                    await RootWrapper.Value.FocusAsync();
            }
        }

        protected Task HandleRootClick(MouseEventArgs e)
        {
            return BodyClickEvent?.Invoke(e);
        }

        protected Task HandleKeyAction(KeyboardEventArgs e)
        {
            return OnKeyDown?.Invoke(e);
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                    //Navigator.LocationChanged -= NavigationManager_LocationChanged;
                }

                // 释放未托管的资源(未托管的对象)并重写终结器
                // 将大型字段设置为 null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
