namespace Project.Constraints.Options;

public sealed class LangInfo
{
    [NotNull] public string? Name { get; set; }
    [NotNull] public string? Culture { get; set; }
}
public sealed class CultureOptions
{
    public bool Enabled { get; set; }
    public LangInfo[] SupportedCulture { get; set; } = [];
}
