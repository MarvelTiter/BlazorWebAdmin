using System;

namespace LogAopCodeGenerator
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class AopMethodFlagAttribute : Attribute
    {

    }
}
