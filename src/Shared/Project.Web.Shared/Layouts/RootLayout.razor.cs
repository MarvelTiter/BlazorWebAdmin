using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Project.Constraints;
using Project.Constraints.UI;

namespace Project.Web.Shared.Layouts
{
    public partial class RootLayout : IDisposable
    {
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
