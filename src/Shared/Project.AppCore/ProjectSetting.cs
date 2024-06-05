using Project.Constraints;
using Project.Constraints.Models.Permissions;
using Project.Services;
namespace Project.AppCore;

public class ProjectSetting
{
    Type settingProviderType = typeof(CustomSetting);
    public Type SettingProviderType => settingProviderType;

    public void ConfigureSettingProviderType<T>() where T : BasicSetting, IProjectSettingService
    {
        settingProviderType = typeof(T);
    }
    public List<Type> interceptorTypes = [];
    public void AddInterceotor<T>() where T : IAddtionalInterceptor
    {
        interceptorTypes.Add(typeof(T));
    }

    internal IPageLocatorService locator = new PageLocatorService();
    
    public void ConfigurePage(Action<IPageLocatorService> pageLocator)
    {
        pageLocator.Invoke(locator);
    }

    public void OverrideTableEntity(Action<DbTableType> config)
    {
        config.Invoke(TypeInfo);
    }

    public Action<AutoInjectFilter>? AutoInjectConfig { get; set; }
    public bool AddFileLogger { get; set; }
    public bool AddDefaultProjectServices { get; set; } = true;
    public AppInfo App => AppConst.App;
    public DbTableType TypeInfo => AppConst.TypeInfo;

    public Type UserType => TypeInfo.UserType;
    public Type RoleType => TypeInfo.RoleType;
    public Type PowerType => TypeInfo.PowerType;
    public Type RolePowerType => TypeInfo.RolePowerType;
    public Type UserRoleType => TypeInfo.UserRoleType;
    public Type RunlogType => TypeInfo.RunlogType;

}
