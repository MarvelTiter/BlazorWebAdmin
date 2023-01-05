using Project.Common.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common
{
    public class BooleanArgs : EventArgs
    {
        public bool IsTrue { get; set; }
        public static BooleanArgs New(bool b)
        {
            return new BooleanArgs { IsTrue = b };
        }
    }
    [AutoInject]
    public class EventDispatcher
    {
        ConcurrentDictionary<Type, Dictionary<string, Func<object, Task<object>>>> allActions = new ConcurrentDictionary<Type, Dictionary<string, Func<object, Task<object>>>>();
        ConcurrentDictionary<string, Func<object, Task>> actions = new ConcurrentDictionary<string, Func<object, Task>>();

        public void Register<TSender>(string key, Func<object, Task<object>> action)
        {
            Register(typeof(TSender), key, action);
        }

        public void Register(Type type, string key, Func<object, Task<object>> action)
        {
            if (!allActions.TryGetValue(type, out var typeDic))
            {
                typeDic = new Dictionary<string, Func<object, Task<object>>>();
                allActions.TryAdd(type, typeDic);
            }
            if (!typeDic.ContainsKey(key))
            {
                typeDic.Add(key, action);
            }
        }

        public async Task<object> Invoke<T>(string key, object args)
        {
            if (string.IsNullOrEmpty(key)) return default;
            var type = typeof(T);
            if (!allActions.TryGetValue(type, out var typeDic))
            {
                Console.WriteLine($"类型[{type.Name}]未注册任何事件");
                return default;
            }
            if (!typeDic.TryGetValue(key, out var func))
            {
                throw new ArgumentException($"类型[{type.Name}]:事件[{key}]未注册");
            }
            return  await func.Invoke(args);
        }
    }
}
