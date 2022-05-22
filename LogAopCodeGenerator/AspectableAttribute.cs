using System;

namespace LogAopCodeGenerator
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class AspectableAttribute : Attribute
    {
        public Type AspectHandleType { get; set; }
    }
}
