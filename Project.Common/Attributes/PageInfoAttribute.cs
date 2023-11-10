namespace Project.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PageInfoAttribute : Attribute
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Icon { get; set; }
        public int Sort { get; set; }
        public bool Pin { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PageGroupAttribute : Attribute
    {
        public string Name { get; }
        public string Icon { get; }
        public int Sort { get; set; }
        public PageGroupAttribute(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }
    }
}
