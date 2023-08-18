using MDbContext;
using MDbContext.ExpressionSql;
using MT.Toolkit.LogTool.LogExtension;
using Project.AppCore;
using Project.AppCore.Locales.Extensions;

namespace BlazorWeb.Shared
{
    public static class DefaultSetup
    {
        public static void Setup(WebApplicationBuilder builder)
        {
            SetupCustomServices(builder);
            SetupLightOrm(builder);
        }
        public static void SetupCustomServices(WebApplicationBuilder builder)
        {
            builder.Logging.AddSimpleLogger(config =>
            {
                config.EnabledLogType = MT.Toolkit.LogTool.LogType.Console | MT.Toolkit.LogTool.LogType.File;
                config.LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            });
        }

        public static void SetupLightOrm(WebApplicationBuilder builder)
        {
            builder.Services.AddLightOrm(option =>
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

        public static IEnumerable<Type> RegisterBlazorViewAssembly()
        {
            return Enumerable.Empty<Type>();
        }

        public static void SetupCustomAppUsage(WebApplication app)
        {

        }
    }
}
