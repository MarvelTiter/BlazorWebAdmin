using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LogAopCodeGenerator
{
    public struct AspectContext
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
        public static AspectContext[] InitTypesContext(Type impl, Type interfaces)
        {
            var interfacesMethods = interfaces.GetMethods();
            List<AspectContext> result = new List<AspectContext>();
            foreach (var method in interfacesMethods)
            {
                var pTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                result.Add(new AspectContext
                {
                    ServiceType = interfaces,
                    ImplementType = impl,
                    ServiceMethod = method,
                    ImplementMethod = impl.GetMethod(method.Name, pTypes)
                });
            }
            return result.ToArray();
        }
    }
}
