using System.Text;

namespace Project.Constraints.Common;

public class ClassHelper
{
    public static ClassHelper Default => new ClassHelper();

    private Dictionary<string, Func<bool>> cssClassMap = new Dictionary<string, Func<bool>>();

    public string Class
    {
        get
        {
            List<string> classes = new List<string>();
            foreach (var item in cssClassMap)
            {
                if (item.Value())
                {
                    classes.Add(item.Key);
                }
            }
            return string.Join(" ", classes.ToArray());
        }
    }


    public ClassHelper AddClass(string? cssClass, Func<bool>? condition = null)
    {
        if (cssClass == null) return this;
        condition ??= () => true;
        cssClassMap.Add(cssClass, condition);
        return this;
    }

    public ClassHelper RemoveClass(string cssClass)
    {
        cssClassMap.Remove(cssClass);
        return this;
    }
}