using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAdmin.Common
{
    public class BooleanArgs : EventArgs
    {
        public bool IsTrue { get; set; }
        public static BooleanArgs New(bool b)
        {
            return new BooleanArgs { IsTrue = b };
        }
    }
    public class EventDispatcher
    {
        Dictionary<Type, Dictionary<string, Func<object, Task>>> allActions = new Dictionary<Type, Dictionary<string, Func<object, Task>>>();
        Dictionary<string, Func<object, Task>> actions = new Dictionary<string, Func<object, Task>>();

        public void Register<T>(string key, Func<object, Task> action)
        {
            Register(typeof(T), key, action);
        }

        public void Register(Type type, string key, Func<object, Task> action)
        {
            if (!allActions.TryGetValue(type, out var typeDic))
            {
                typeDic = new Dictionary<string, Func<object, Task>>();
                allActions.Add(type, typeDic);
            }
            if (!typeDic.ContainsKey(key))
            {
                typeDic.Add(key, action);
            }
        }

        public async Task Invoke<T>(string key, object args)
        {
            if (string.IsNullOrEmpty(key)) return;
            var type = typeof(T);
            if (!allActions.TryGetValue(type, out var typeDic))
            {
                Console.WriteLine($"类型[{type.Name}]未注册任何事件");
                return;
            }
            if (!typeDic.TryGetValue(key, out var func))
            {
                throw new ArgumentException($"类型[{type.Name}]:事件[{key}]未注册");
            }
            await func.Invoke(args);
        }
    }
}
