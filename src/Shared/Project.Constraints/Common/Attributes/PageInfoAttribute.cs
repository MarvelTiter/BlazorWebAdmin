namespace Project.Constraints.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PageInfoAttribute : Attribute
{
    public PageInfoAttribute(string id)
    {
        Id = id;
    }
    public PageInfoAttribute()
    {
            
    }

    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Icon { get; set; }
    public int Sort { get; set; }
    public bool Pin { get; set; }
    public string? GroupId { get; set; }
    /// <summary>
    /// 强制显示在菜单
    /// </summary>
    public bool ForceShowOnNavMenu { get; set; }
}

/// <summary>
/// 只在没有<see cref="PageInfoAttribute"/>时才生效
/// </summary>
/// <param name="title"></param>
[AttributeUsage(AttributeTargets.Class)]
public class TagTitleAttribute(string title) : Attribute
{
    public string Title { get; set; } = title;
}

/// <summary>
/// 定义一个页面分组
/// </summary>
/// <param name="id"></param>
/// <param name="name"></param>
/// <param name="sort"></param>
[AttributeUsage(AttributeTargets.Class)]
public class PageGroupAttribute(string id, string name, int sort) : Attribute
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public string? Icon { get; set; }
    public int Sort { get; } = sort;
}