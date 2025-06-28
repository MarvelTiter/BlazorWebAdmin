namespace Project.Web.Shared.Store;

public abstract class StoreBase : IDisposable
{
    private bool disposedValue;

    public event Action? DataChangedEvent;
    protected void NotifyChanged()
    {
        DataChangedEvent?.Invoke();
    }

    protected virtual void Release()
    {

    }

    protected virtual void SetNull()
    {

    }

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Release();
            }
            SetNull();
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}