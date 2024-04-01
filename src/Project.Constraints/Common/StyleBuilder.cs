namespace Project.Constraints.Common
{
    public static class StyleExtension
    {
        public static string ToPixel(this int val)
        {
            return $"{val}px";
        }
        public static string ToPixel(this int? val)
        {
            return $"{val}px";
        }
        public static string ToPixel(this string val)
        {
            return $"{val}px";
        }
    }
    public class StyleBuilder
    {
        struct StyleItem
        {
            public Func<string> Value { get; set; }
            public Func<bool> Condition { get; set; }
        }
        public static StyleBuilder Default => new StyleBuilder();

        //private Dictionary<Func<string>, string> styles = new Dictionary<Func<string>, string>();
        private Dictionary<string, StyleItem> styles = new Dictionary<string, StyleItem>();

        public StyleBuilder AddStyleBase(string styleString, Func<bool>? condition = null)
        {
            styles[""] = new StyleItem
            {
                Value = () => styleString,
                Condition = condition ??= () => true,
            };
            return this;
        }

        public StyleBuilder AddStyle(string styleName, Func<string> styleValue, Func<bool>? condition = null)
        {
            styles.TryAdd(styleName, new StyleItem
            {
                Value = styleValue,
                Condition = condition ??= () => true,
            });
            return this;
        }

        public string Style
        {
            get
            {
                return string.Join("", styles.Select(pair =>
                {
                    if (pair.Value.Condition())
                    {
                        return $"{pair.Key}: {pair.Value.Value()};";
                    }
                    return "";
                }));
            }
        }
    }
}