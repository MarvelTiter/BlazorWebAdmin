using System;
using WebApiGenerator.Models;

namespace WebApiGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class WebMethodAttribute : Attribute
    {
        public HttpMethod Method { get; set; }
        public string? Route { get; set; }
    }
}
