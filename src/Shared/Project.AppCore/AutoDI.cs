using Microsoft.Extensions.DependencyInjection;
using Project.Constraints.Common.Attributes;
using System.Reflection;

namespace Project.AppCore
{
    public class AutoInjectFilter
    {
        public Func<string, bool> FileFilter { get; set; } = f => false;
        public Func<Type, bool> TypeFilter { get; set; } = t => true;
    }
    //public static class AutoDI
    //{
    //    public static IServiceCollection AutoInjects(this IServiceCollection self, Action<AutoInjectFilter>? action = null)
    //    {
    //        return self;
    //        var filter = new AutoInjectFilter();
    //        action?.Invoke(filter);
    //        var all = LoadAllAssembly(filter);
    //        var allTypes = LoadTypeFromAssembly(filter, all.ToArray());

    //        //class的程序集
    //        var implementTypes = allTypes.Where(x => x.IsClass).ToArray();
    //        //接口的程序集
    //        var interfaceTypes = allTypes.Where(x => x.IsInterface).ToArray();

    //        InjectServices(self, implementTypes, interfaceTypes);

    //        return self;
    //    }

    //    private static void InjectServices(IServiceCollection services, Type[] implementTypes, Type[] interfaceTypes)
    //    {
    //        foreach (var implementType in implementTypes)
    //        {
    //            if (implementType.IsAbstract)
    //            {
    //                continue;
    //            }
    //            var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
    //            var injectType = implementType.GetCustomAttribute<AutoInjectAttribute>();
    //            //if (implementType.GetCustomAttribute<AspectableAttribute>() != null) continue;
    //            DIType dIType = DIType.Scope;
    //            if (injectType != null)
    //            {
    //                dIType = injectType.TimeLife;
    //            }
    //            //class有接口，用接口注入
    //            if (interfaceType != null)
    //            {
    //                //判断用什么方式注入
    //                if (dIType == DIType.Scope)
    //                {
    //                    services.AddScoped(interfaceType, implementType);
    //                }
    //                else if (dIType == DIType.Singleton)
    //                {
    //                    services.AddSingleton(interfaceType, implementType);
    //                }
    //                else
    //                {
    //                    services.AddTransient(interfaceType, implementType);
    //                }
    //            }
    //            else //class没有接口，直接注入class
    //            {
    //                if (dIType == DIType.Scope)
    //                {
    //                    services.AddScoped(implementType);
    //                }
    //                else if (dIType == DIType.Singleton)
    //                {
    //                    services.AddSingleton(implementType);
    //                }
    //                else
    //                {
    //                    services.AddTransient(implementType);
    //                }
    //            }
    //        }
    //    }


    //    private static IEnumerable<Type> LoadTypeFromAssembly(AutoInjectFilter filter, params Assembly[] assembly)
    //    {
    //        foreach (var asm in assembly)
    //        {
    //            var full = asm.GetCustomAttributesData().Any(cd => cd.AttributeType == typeof(AutoInjectAttribute));
    //            var types = asm.GetExportedTypes();
    //            foreach (var type in types)
    //            {
    //                if (!type.FullName!.StartsWith("Project.") && !filter.TypeFilter(type)) continue;
    //                if (full && type.GetCustomAttribute<IgnoreAutoInjectAttribute>(false) == null)
    //                {
    //                    yield return type;
    //                }
    //                else if (type.GetCustomAttribute<AutoInjectAttribute>() != null)
    //                {
    //                    yield return type;
    //                }
    //            }
    //        }
    //    }

    //    private static IEnumerable<Assembly> LoadAllAssembly(AutoInjectFilter filter)
    //    {
    //        var entry = Assembly.GetEntryAssembly();
    //        var asms = entry?.GetReferencedAssemblies().Select(Assembly.Load);
    //        foreach (var asm in asms ?? Enumerable.Empty<Assembly>())
    //        {
    //            var filename = asm.FullName;
    //            if ((filename!.StartsWith("Project") || filter.FileFilter.Invoke(filename)))
    //            {
    //                yield return asm;
    //            }
    //        }
    //    }
    //}
}
