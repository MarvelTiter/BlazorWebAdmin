using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAdmin.Store
{
    public class StateContainer
    {
        private Dictionary<string, object> _state = new();
        public T Get<O, T>(string key) where T : class, new()
        {
            var fullKey = $"{typeof(O).FullName}_{key}";
            if (!_state.TryGetValue(fullKey, out var value))            
            {
                value = new T();
                _state.Add(fullKey, value);
            }
            return (T)value;
        }
    }
}
