//using System.Collections.Concurrent;

//namespace Project.Common
//{
//    public class BooleanArgs : EventArgs
//    {
//        public bool IsTrue { get; set; }
//        public static BooleanArgs New(bool b)
//        {
//            return new BooleanArgs { IsTrue = b };
//        }
//    }
//    [AutoInject]
//    public class EventDispatcher
//    {
//        ConcurrentDictionary<Type, Dictionary<string, Func<object, object, Task<object>>>> allActions = new ConcurrentDictionary<Type, Dictionary<string, Func<object, object, Task<object>>>>();

//        public void Register<TOwner>(string key, Func<object, object, Task<object>> action)
//        {
//            Register(typeof(TOwner), key, action);
//        }

//        public void Register(Type type, string key, Func<object, object, Task<object>> action)
//        {
//            if (!allActions.TryGetValue(type, out var typeDic))
//            {
//                typeDic = new Dictionary<string, Func<object, object, Task<object>>>();
//                allActions.TryAdd(type, typeDic);
//            }
//            if (!typeDic.ContainsKey(key))
//            {
//                typeDic.Add(key, action);
//            }
//        }

//        public Task<object> Invoke<TOwner>(string key, object sender, object args)
//        {
//            return Invoke<TOwner, object>(key, sender, args);
//        }

//        public async Task<TReturn> Invoke<TOwner, TReturn>(string key, object sender, object args)
//        {
//            if (string.IsNullOrEmpty(key)) return default;
//            var type = typeof(TOwner);
//            if (!allActions.TryGetValue(type, out var typeDic))
//            {
//                Console.WriteLine($"类型[{type.Name}]未注册任何事件");
//                return default;
//            }
//            if (!typeDic.TryGetValue(key, out var func))
//            {
//                throw new ArgumentException($"类型[{type.Name}]:事件[{key}]未注册");
//            }
//            var ret = await func.Invoke(sender, args);
//            return (TReturn)ret;
//        }
//    }
//}
