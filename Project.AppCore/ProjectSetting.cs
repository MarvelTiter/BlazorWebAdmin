using Project.Constraints.Models.Permissions;
using Project.Services;
namespace Project.AppCore;

public class ProjectSetting
{
    Type settingProviderType = typeof(CustomSetting);
    public Type SettingProviderType => settingProviderType;

    public void ConfigureSettingProviderType<T>() where T : BasicCustomSetting, ICustomSettingService
    {
        settingProviderType = typeof(T);
    }

    public void OverrideTableEntity(Action<DbTableType> config)
    {
        config.Invoke(TypeInfo);
    }

    public Action<AutoInjectFilter>? AutoInjectConfig { get; set; }
    public bool AddDefaultLogger { get; set; }
    public bool AddDefaultProjectServices { get; set; } = true;
    public AppInfo App => Config.App;
    public DbTableType TypeInfo => Config.TypeInfo;

    public Type UserType => TypeInfo.UserType;
    public Type RoleType => TypeInfo.RoleType;
    public Type PowerType => TypeInfo.PowerType;
    public Type RolePowerType => TypeInfo.RolePowerType;
    public Type UserRoleType => TypeInfo.UserRoleType;
    public Type RunlogType => TypeInfo.RunlogType;

}
