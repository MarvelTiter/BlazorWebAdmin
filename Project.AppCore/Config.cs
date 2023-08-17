using System.Reflection;

namespace Project.AppCore
{
    public class Config
    {
        static List<Assembly> PageAssemblies = new List<Assembly>();
        public static List<Assembly> Pages => PageAssemblies;
        public static void AddAssembly(params Type[] types)
        {
            foreach (var item in types)
            {
                PageAssemblies.Add(item.Assembly);
            }
        }

        public static void AddAssembly(params Assembly[] asms)
        {
            foreach (var item in asms)
            {
                PageAssemblies.Add(item);
            }
        }
    }
}
