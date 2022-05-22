using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ApplicationStore.Store
{
    public class StateContainer
    {
        private readonly Dictionary<string, object> _state = new();
        public T Get<OwnerType, T>(string key = "key") where T : class, new()
        {
            var fullKey = $"{typeof(OwnerType).FullName}_{typeof(T).FullName}_{key}";
            if (!_state.TryGetValue(fullKey, out var value))
            {
                value = new T();
                _state.Add(fullKey, value);
            }
            return (T)value;
        }
    }
}
