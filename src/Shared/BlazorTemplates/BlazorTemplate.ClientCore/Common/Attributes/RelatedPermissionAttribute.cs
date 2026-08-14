namespace BlazorTemplate.ClientCore.Common.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RelatedPermissionAttribute : Attribute
{
    public string? PermissionId { get; set; }
}
