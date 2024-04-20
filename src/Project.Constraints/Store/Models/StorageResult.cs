namespace Project.Constraints.Store.Models
{
    public readonly struct StorageResult<TValue>
    {
        public StorageResult(bool success, TValue? value)
        {
            Success = success;
            Value = value;
        }
        public bool Success { get; }
        public TValue? Value { get; }
    }
}
