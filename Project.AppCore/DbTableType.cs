using Project.Constraints.Models.Permissions;
namespace Project.AppCore;

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
