namespace Project.Constraints.UI;

public readonly struct IconInfo(string name, string @class)
{
    public string Name { get; } = name;
    public string Class { get; } = @class;
}