using System;

namespace WebApiGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
    public class WebControllerAttribute : Attribute
    {
        public string? Route { get; set; }
    }
}
