using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LogAopCodeGenerator
{
    public class InitContext
    {
        public static BaseContext[] InitTypesContext(Type impl, Type interfaces)
        {
            var interfacesMethods = interfaces.GetMethods();
            List<BaseContext> result = new List<BaseContext>();
            foreach (var method in interfacesMethods)
            {
                var pTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                result.Add(new BaseContext
                {
                    ServiceType = interfaces,
                    ImplementType = impl,
                    ServiceMethod = method,
                    ImplementMethod = impl.GetMethod(method.Name, pTypes)
                });
            }
            return result.ToArray();
        }
        public static AspectContext BuildContext(BaseContext @base)
        {
            return new AspectContext
            {
                ServiceType = @base.ServiceType,
                ServiceMethod = @base.ServiceMethod,
                ImplementType = @base.ImplementType,
                ImplementMethod = @base.ImplementMethod,
            };
        }
    }
    public struct BaseContext
    {
        public Type ServiceType { get; set; }
        public Type ImplementType { get; set; }
        public MethodInfo ServiceMethod { get; set; }
        public MethodInfo ImplementMethod { get; set; }
    }
    public class AspectContext
    {
        public Type ServiceType { get; set; }
        public Type ImplementType { get; set; }
        public MethodInfo ServiceMethod { get; set; }
        public MethodInfo ImplementMethod { get; set; }
        public IEnumerable<object> Parameters { get; set; }
        public bool IsAsync { get; set; }
        public bool HasReturnValue { get; set; }
        public object ReturnValue { get; set; }
        /// <summary>
        /// 是否已执行
        /// </summary>
        public bool Exected { get; set; }
        public Func<Task> Proceed { get; set; }
    }
}
