
namespace BlazorTemplate.ClientCore.Common.Attributes;

#if NET8_0
[AttributeUsage(AttributeTargets.Class)]
public class ExcludeFromInteractiveRoutingAttribute : Attribute
{
}
#endif