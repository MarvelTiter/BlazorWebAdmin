using Project.Common.Attributes;
using Project.Repositories;
using Project.Repositories.interfaces;
using System.Reflection;

namespace BlazorWebAdmin
{
    public static class AutoDI
    {
        public static IServiceCollection AutoInjects(this IServiceCollection self)
        {
            var all = LoadAllAssembly();
            var allTypes = LoadTypeFromAssembly(all.ToArray())
                .Where(t => t.FullName!.StartsWith("Project"));

            //class的程序集
            var implementTypes = allTypes.Where(x => x.IsClass).ToArray();
            //接口的程序集
            var interfaceTypes = allTypes.Where(x => x.IsInterface).ToArray();

            InjectServices(self, implementTypes, interfaceTypes);

            var entities = GetDbEntities(all.First(asm => asm.FullName!.Contains("Project.Models"))).ToArray();

            InjectGeneralRepository(self, entities, typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

            return self;
        }

        private static void InjectServices(IServiceCollection services, Type[] implementTypes, Type[] interfaceTypes)
        {
            foreach (var implementType in implementTypes)
            {
                if (implementType.IsAbstract)
                {
                    continue;
                }
                var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
                var injectType = implementType.GetCustomAttribute<AutoInjectAttribute>();
                DIType dIType = DIType.Scope;
                if (injectType != null)
                {
                    dIType = injectType.TimeLife;
                }
                //class有接口，用接口注入
                if (interfaceType != null)
                {
                    //判断用什么方式注入
                    if (dIType == DIType.Scope)
                    {
                        services.AddScoped(interfaceType, implementType);
                    }
                    else if (dIType == DIType.Singleton)
                    {
                        services.AddSingleton(interfaceType, implementType);
                    }
                    else
                    {
                        services.AddTransient(interfaceType, implementType);
                    }
                }
                else //class没有接口，直接注入class
                {
                    if (dIType == DIType.Scope)
                    {
                        services.AddScoped(implementType);
                    }
                    else if (dIType == DIType.Singleton)
                    {
                        services.AddSingleton(implementType);
                    }
                    else
                    {
                        services.AddTransient(implementType);
                    }
                }
            }
        }

        private static void InjectGeneralRepository(IServiceCollection services, Type[] entities, Type baseInterface, Type baseImpl)
        {
            foreach (var item in entities)
            {
                var i = baseInterface.MakeGenericType(item);
                var p = baseImpl.MakeGenericType(item);
                services.AddScoped(i, p);
            }
        }

        private static IEnumerable<Type> GetDbEntities(Assembly assembly)
        {
            foreach (var item in assembly.GetTypes())
            {
                if (item.FullName!.Contains("Entities") || item.FullName!.Contains("Permissions"))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<Type> LoadTypeFromAssembly(params Assembly[] assembly)
        {
            foreach (var asm in assembly)
            {
                var full = asm.GetCustomAttributesData().Any(cd => cd.AttributeType == typeof(AutoInjectAttribute));
                foreach (var type in asm.GetTypes())
                {
                    if (full && type.GetCustomAttribute<IgnoreAutoInjectAttribute>(false) == null)
                    {
                        yield return type;
                    }
                    else if (type.GetCustomAttribute<AutoInjectAttribute>() != null)
                    {
                        yield return type;
                    }
                }
            }
        }

        private static IEnumerable<Assembly> LoadAllAssembly()
        {
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(folder);
            //var asms = AppDomain.CurrentDomain.GetAssemblies().Where(asm => asm.FullName!.StartsWith("BlazorWebAdmin")|| asm.FullName!.StartsWith("Project"));
            foreach (var file in files)
            {
                var filename = Path.GetFileName(file).Replace("\\", "/");
                var fileext = Path.GetExtension(file);
                if ((filename.StartsWith("BlazorWebAdmin") || filename.StartsWith("Project")) && fileext == ".dll")
                {
                    var asm = Assembly.LoadFrom(file);
                    if (asm != null)
                        yield return asm;
                }
            }
        }
    }
}
