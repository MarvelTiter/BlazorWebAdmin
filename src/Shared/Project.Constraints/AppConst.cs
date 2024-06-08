using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Project.Constraints.Models.Permissions;
using Project.Constraints.UI.Extensions;
using System.Reflection;

namespace Project.Constraints;
public class AppInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Company { get; set; }
    public string Version { get; set; }
}

public class DbTableType
{
    Type userType = typeof(User);
    public Type UserType => userType;
    Type roleType = typeof(Role);
    public Type RoleType => roleType;
    Type powerType = typeof(Power);
    public Type PowerType => powerType;
    Type rolepowerType = typeof(RolePower);
    public Type RolePowerType => rolepowerType;
    Type userroleType = typeof(UserRole);
    public Type UserRoleType => userroleType;
    Type logType = typeof(RunLog);
    public Type RunlogType => logType;

    public void SetUserType<TUser>() where TUser : IUser
    {
        userType = typeof(TUser);
    }
    public void SetRoleType<TRole>() where TRole : IRole
    {
        roleType = typeof(TRole);
    }
    public void SetPowerType<TPower>() where TPower : IPower
    {
        powerType = typeof(TPower);
    }
    public void SetRolePowerType<TRolePower>() where TRolePower : IRolePower
    {
        rolepowerType = typeof(TRolePower);
    }
    public void SetUserRoleType<TUserRole>() where TUserRole : IUserRole
    {
        userroleType = typeof(TUserRole);
    }
    public void SetRunlogType<TRunLog>() where TRunLog : IRunLog
    {
        logType = typeof(TRunLog);
    }
}
public static class AppConst
{
    public static AppInfo App { get; set; } = new AppInfo();
    public static DbTableType TypeInfo { get; set; } = new DbTableType();

    static List<Assembly> PageAssemblies = new List<Assembly>();

    public static RenderFragment Footer { get; set; }
    public static List<Assembly> Pages => PageAssemblies;
    public static void SetFooter(string html)
    {
        Footer = html.AsMarkupString();
    }
    public static void AddAssembly(params Type[] types)
    {
        foreach (var item in types)
        {
            PageAssemblies.Add(item.Assembly);
        }
    }

    public static void AddAssembly(params Assembly[] asms)
    {
        foreach (var item in asms)
        {
            PageAssemblies.Add(item);
        }
    }

    public static readonly string TempFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempfile");
    public static string GetPath(params string[] paths)
    {
        if (paths?.Length == 0)
        {
            return TempFilePath;
        }
        return Path.Combine([AppDomain.CurrentDomain.BaseDirectory, ..paths]);
    }
    public static IHostEnvironment? Environment { get; set; }
    public static IServiceProvider? Services { get; set; }

    static AppConst()
    {
        if (!Directory.Exists(TempFilePath))
            Directory.CreateDirectory(TempFilePath);
    }
}
