using MDbContext;
using MDbContext.ExpressionSql;
using Project.AppCore.Locales.Extensions;

namespace BlazorWebAdmin
{
    public static class CustomSetup
    {
        public static void SetupCustomServices(IServiceCollection services)
        {
            services.AddJsonLocales();
        }

        public static void SetupLightOrm(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddLightOrm(option =>
            option.SetDatabase(DbBaseType.Sqlite, Project.AppCore.LightDb.CreateConnection)
    .SetWatcher(sql =>
    {
        sql.BeforeExecute = e =>
        {
#if DEBUG
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Sql => \n{e.Sql}\n");
#endif
        };
    })
			);
        }

        public static void RegisterBlazorViewAssembly()
        {
            Config.AddAssembly(typeof(BlazorWeb.Shared.Program));
        }

        public static void SetupCustomAppUsage(WebApplication app)
        {

        }
    }
}
