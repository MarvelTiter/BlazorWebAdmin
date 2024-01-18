using Project.Services;
namespace Project.AppCore;

public class ProjectSetting : DbTableType
{
    Type settingProviderType = typeof(CustomSetting);
    public Type SettingProviderType => settingProviderType;

    public void ConfigureSettingProviderType<T>() where T : BasicCustomSetting, ICustomSettingProvider
    {
        settingProviderType = typeof(T);
    }

    public void OverrideTableEntity(Action<DbTableType> config)
    {
        config.Invoke(this);
    }

    public Action<AutoInjectFilter>? AutoInjectConfig { get; set; }
    public bool AddDefaultLogger { get; set; }
    public AppInfo App => Config.App;
}
