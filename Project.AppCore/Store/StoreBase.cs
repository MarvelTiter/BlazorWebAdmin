namespace Project.AppCore.Store
{
    public abstract class StoreBase
    {
        public event Action DataChangedEvent;
        protected void NotifyChanged()
        {
            DataChangedEvent?.Invoke();
        }
    }
}
