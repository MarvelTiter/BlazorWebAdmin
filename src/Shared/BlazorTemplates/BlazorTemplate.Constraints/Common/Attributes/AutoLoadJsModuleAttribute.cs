namespace BlazorTemplate.Constraints.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AutoLoadJsModuleAttribute : Attribute
{
    /// <summary>
    /// 不包含js文件名称
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 包含js文件名
    /// </summary>
    public string? FullPath { get; set; }
}