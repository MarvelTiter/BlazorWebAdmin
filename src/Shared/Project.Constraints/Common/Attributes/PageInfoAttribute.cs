namespace Project.Constraints.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PageInfoAttribute : Attribute
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Icon { get; set; }
        public int Sort { get; set; }
        public bool Pin { get; set; }
        public string? GroupId { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PageGroupAttribute(string id, string name, int sort) : Attribute
    {
        public string Id { get; } = id;
        public string Name { get; } = name;
        public string? Icon { get; set; }
        public int Sort { get; } = sort;
    }
}
