using Project.Constraints;
using Project.Constraints.Models.Permissions;
namespace Project.Web.Shared;

public class ProjectSetting
{
    private Type settingProviderType = typeof(BasicSetting);
    private Type? authServiceType;
    public Type SettingProviderType => settingProviderType;
    public Type? AuthServiceType => authServiceType;
    public void ConfigureSettingProviderType<T>() where T : BasicSetting, IProjectSettingService
    {
        settingProviderType = typeof(T);
    }
    public void ConfigureAuthService<T>() where T : IAuthService
    {
        authServiceType = typeof(T);
    }
    public readonly List<Type> interceptorTypes = [];
    public void AddInterceotor<T>() where T : IAddtionalInterceptor
    {
        interceptorTypes.Add(typeof(T));
    }

    internal readonly IPageLocatorService locator = new PageLocatorService();

    public void ConfigurePage(Action<IPageLocatorService> pageLocator)
    {
        pageLocator.Invoke(locator);
    }

    //public void OverrideTableEntity(Action<DbTableType> config)
    //{
    //    config.Invoke(TypeInfo);
    //}

    public AppInfo App => AppConst.App;
    //public DbTableType TypeInfo => AppConst.TypeInfo;

    //public Type UserType => TypeInfo.UserType;
    //public Type RoleType => TypeInfo.RoleType;
    //public Type PowerType => TypeInfo.PowerType;
    //public Type RolePowerType => TypeInfo.RolePowerType;
    //public Type UserRoleType => TypeInfo.UserRoleType;
    //public Type RunlogType => TypeInfo.RunlogType;

}
