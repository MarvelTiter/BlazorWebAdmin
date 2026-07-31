namespace BlazorTemplate.Constraints.Common.Attributes;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false)]
public class LangNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}