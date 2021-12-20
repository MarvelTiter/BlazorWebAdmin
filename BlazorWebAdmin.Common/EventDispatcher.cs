using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAdmin.Common
{
    public class EventDispatcher
    {
        Dictionary<string, Func<Task>> actions = new Dictionary<string, Func<Task>>();
        
        public void Register(string key, Func<Task> action)
        {
            if (!actions.ContainsKey(key))
            {
                actions.Add(key, action);
            }
        }

        public async Task Invoke(string key)
        {
            if (actions.ContainsKey(key))
                await actions[key].Invoke();
        }

    }
}
