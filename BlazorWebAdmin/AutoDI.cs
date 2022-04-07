using Project.Common.Attributes;
using Project.IRepositories;
using System.Diagnostics;
using System.Reflection;

namespace BlazorWebAdmin
{
    public static class AutoDI
    {
        public static IServiceCollection AutoInjects(this IServiceCollection self)
        {

            var all = LoadAllAssembly();
            var allTypes = LoadTypeFromAssembly(all.ToArray())
                .Where(t => t.Namespace.StartsWith("Project"));

            //class的程序集
            var implementTypes = allTypes.Where(x => x.IsClass).ToArray();
            //接口的程序集
            var interfaceTypes = allTypes.Where(x => x.IsInterface).ToArray();

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
                        self.AddScoped(interfaceType, implementType);
                    }
                    else if (dIType == DIType.Singleton)
                    {
                        self.AddSingleton(interfaceType, implementType);
                    }
                    else
                    {
                        self.AddTransient(interfaceType, implementType);
                    }
                }
                else //class没有接口，直接注入class
                {
                    if (dIType == DIType.Scope)
                    {
                        self.AddScoped(implementType);
                    }
                    else if (dIType == DIType.Singleton)
                    {
                        self.AddSingleton(implementType);
                    }
                    else
                    {
                        self.AddTransient(implementType);
                    }
                }
            }

            return self;
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

            //AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm =>
            //{
            //    var full = asm.GetCustomAttributesData().Any(cd => cd.AttributeType == typeof(AutoInjectAttribute));
            //    if (full)
            //    {
            //        return asm.GetTypes().Where(t => t.GetCustomAttribute<IgnoreAutoInjectAttribute>(false) == null);
            //    }
            //    else
            //    {
            //        return asm.GetTypes().Where(t => t.GetCustomAttribute<AutoInjectAttribute>() != null);
            //    }
            //});
        }

        private static IEnumerable<Assembly> LoadAllAssembly()
        {
            var folder = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(folder);
            foreach (var file in files)
            {
                var filename = Path.GetFileName(file).Replace("\\", "/");
                var fileext = Path.GetExtension(file);
                if (filename.StartsWith("Project") && fileext == ".dll")
                {
                    var asm = Assembly.LoadFrom(file);
                    if (asm != null)
                        yield return asm;
                }
            }
        }
    }
}
