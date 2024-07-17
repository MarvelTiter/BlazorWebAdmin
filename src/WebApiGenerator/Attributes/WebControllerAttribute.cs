using System;

namespace WebApiGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WebControllerAttribute : Attribute
    {
        public string? Route { get; set; }
    }
}
