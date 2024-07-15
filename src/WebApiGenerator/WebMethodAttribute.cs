using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiGenerator
{
    public class WebApiGeneratorHepers
    {
        public const string Enums = @"""
namespace WebApiGenerator
{
    public enum HttpMethod
    {
        Get,
        Post,
    }
}
""";
        public const string WebMethodAttribute = @"""
namespace WebApiGenerator
{
    [System.AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class WebMethodAttribute : System.Attribute
    {
        public HttpMethod Method { get; set; }
        public string Route { get; set; }
    }
}
""";

        public const string WebControllerAttribute = @"""
namespace WebApiGenerator
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WebControllerAttribute : System.Attribute
    {

    }
}
""";
    }

    
    
}
