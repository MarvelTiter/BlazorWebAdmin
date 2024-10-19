namespace Project.Constraints.Common.Attributes;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public class LangNameAttribute : Attribute
{
    public LangNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}