namespace Project.AppCore.Options
{
    public sealed class LangInfo
    {
        public string Name { get; set; }
        public string Culture { get; set; }
    } 
    public sealed class CultureOptions
    {
        public bool Enabled { get; set; }
        public LangInfo[] SupportedCulture { get; set; }
    }
}
