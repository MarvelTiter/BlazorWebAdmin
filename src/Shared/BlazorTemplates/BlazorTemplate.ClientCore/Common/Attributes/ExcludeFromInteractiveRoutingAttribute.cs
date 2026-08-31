
#if NET8_0
#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.AspNetCore.Components;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配

[AttributeUsage(AttributeTargets.Class)]
public class ExcludeFromInteractiveRoutingAttribute : Attribute
{
}
#endif