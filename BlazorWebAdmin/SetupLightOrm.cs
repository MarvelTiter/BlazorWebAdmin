using MDbContext;
using MDbContext.ExpressionSql;

namespace BlazorWebAdmin
{
	public class CustomSetup
	{
		public static void SetupCustomServices(IServiceCollection services)
		{

		}

		public static void SetupLightOrm(ConfigurationManager configuration, ExpressionSqlOptions option)
		{
			option.SetDatabase(DbBaseType.Sqlite, Project.AppCore.LightDb.CreateConnection)
	.SetWatcher(sql =>
	{
		sql.BeforeExecute = e =>
		{
#if DEBUG
			Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Sql => \n{e.Sql}\n");
#endif
		};
	});
		}
	}
}
