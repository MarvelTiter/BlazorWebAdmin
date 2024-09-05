using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Utils
{
    public class BooleanStatusManager : IDisposable, IAsyncDisposable
    {
        private bool disposedValue;
        private readonly Action<bool> setter;
        private readonly bool init;
        private readonly Func<Task>? callback;

        public static BooleanStatusManager New(Action<bool> setter, bool init = true, Func<Task>? callback = null) => new(setter, init, callback);
        public BooleanStatusManager(Action<bool> setter, bool init = true, Func<Task>? callback = null)
        {
            this.setter = setter;
            this.init = init;
            this.callback = callback;
            this.setter.Invoke(init);
            callback?.Invoke().GetAwaiter().GetResult();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    setter.Invoke(!init);
                    callback?.Invoke().GetAwaiter().GetResult();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            setter.Invoke(!init);
            if (callback != null)
            {
                await callback.Invoke();
            }
            GC.SuppressFinalize(this);
        }
    }
}
