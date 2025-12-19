namespace Project.Constraints.Common.Attributes;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class LangNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}