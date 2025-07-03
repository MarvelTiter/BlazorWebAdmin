namespace Project.Constraints.UI.Props;

[AttributeUsage(AttributeTargets.Property)]
public class PropNameAttribute : Attribute
{
    public PropNameAttribute(string name)
    {
        Name = name;
    }

    public PropNameAttribute()
    {
            
    }

    public string? Name { get; }
}
public class HtmlProp
{
    [PropName("class")]
    public string? Class { get; set; }
    [PropName("style")]
    public string? Style { get; set; }
}