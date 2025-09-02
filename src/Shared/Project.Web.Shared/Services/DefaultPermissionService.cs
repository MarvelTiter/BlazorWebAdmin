using LightORM.Extension;
using AutoInjectGenerator;
using LightORM;
using AutoAopProxyGenerator;
using Project.Constraints.Models.Permissions;
namespace Project.Web.Shared.Services;

public class DefaultPermissionService<TPower, TRole, TRolePower, TUserRole>
    where TPower : class, IPermission, new()
    where TRole : class, IRole, new()
    where TRolePower : class, IRolePermission, new()
    where TUserRole : class, IUserRole, new()
{
    private readonly IExpressionContext context;

    public DefaultPermissionService(IExpressionContext context)
    {
        this.context = context;
    }
    public virtual async Task<QueryCollectionResult<TPower>> GetPermissionListAsync(GenericRequest<TPower> req)
    {
        var list = await context.Select<TPower>()
            .Where(req.Expression())
            .Count(out var total)
            .OrderBy(p => p.Sort)
            .ToListAsync();
        return list.CollectionResult((int)total);
    }

    public virtual async Task<QueryCollectionResult<TPower>> GetAllPermissionAsync()
    {
        var list = await context.Select<TPower>().OrderBy(e => e.Sort).ToListAsync();
        return list.CollectionResult();
    }

    public virtual async Task<QueryCollectionResult<TRole>> GetRoleListAsync(GenericRequest<TRole> req)
    {
        var list = await context.Select<TRole>()
            .Where(req.Expression())
            .Count(out var total)
            .Paging(req.PageIndex, req.PageSize)
            .ToListAsync();
        return list.CollectionResult((int)total);
    }

    public virtual async Task<QueryCollectionResult<TRole>> GetAllRoleAsync()
    {
        var list = await context.Select<TRole>().ToListAsync();
        return list.CollectionResult();
    }

    public virtual async Task<QueryCollectionResult<TPower>> GetPermissionListByUserIdAsync(string usrId)
    {
        var powers = await context.Select<TPower, TRolePower, TUserRole>()
            .Distinct()
            .InnerJoin<TRolePower>(w => w.Tb1.PermissionId == w.Tb2.PermissionId)
            .InnerJoin<TUserRole>(w => w.Tb2.RoleId == w.Tb3.RoleId)
            .Where(w => w.Tb3.UserId == usrId)
            .OrderBy(w => w.Tb1.Sort)
            .ToListAsync(w => w.Tb1);
        return powers.CollectionResult();
    }

    public virtual async Task<QueryCollectionResult<TPower>> GetPermissionListByRoleIdAsync(string roleId)
    {
        var powers = await context.Select<TPower>()
            .InnerJoin<TRolePower>((r, p) => p.PermissionId == r.PermissionId)
            .Where((r, rp) => rp.RoleId == roleId)
            .ToListAsync();
        return powers.CollectionResult();
    }

    public virtual async Task<QueryCollectionResult<TRole>> GetUserRolesAsync(string usrId)
    {
        var roles = await context.Select<TRole>()
            .InnerJoin<TUserRole>((r, ur) => r.RoleId == ur.RoleId)
            .Where((r, ur) => ur.UserId == usrId)
            .ToListAsync();
        return roles.CollectionResult();
    }

    public virtual async Task<QueryResult> SaveRoleWithPermissionsAsync(TRole role)
    {
        using var scoped = context.CreateMainDbScoped();
        try
        {
            await scoped.BeginTransactionAsync();
            var n = await scoped.Update(role)
                .Where(r => r.RoleId == role.RoleId)
                .ExecuteAsync();

            var roleId = role.RoleId;
            string[] powers = [.. role.Permissions?.Distinct()];
            var d = await scoped.Delete<TRolePower>().Where(r => r.RoleId == roleId).ExecuteAsync();
            var i = 0;
            foreach (var p in powers)
            {
                var rp = new TRolePower() { RoleId = roleId, PermissionId = p };
                var ef = await scoped.Insert<TRolePower>(rp).ExecuteAsync();
                i += ef;
            }
            if (n == 1 && i == powers.Length)
            {
                await scoped.CommitTransactionAsync();
                return QueryResult.Success();
            }
            else
            {
                await scoped.RollbackTransactionAsync();
                return QueryResult.Fail();
            }
        }
        catch (Exception ex)
        {
            await scoped.RollbackTransactionAsync();
            return QueryResult.Fail().SetMessage(ex.Message);
        }
    }

    public virtual async Task<QueryResult> UpdatePermissionAsync(TPower power)
    {
        var n = await context.Update(power)
            .Where(p => p.PermissionId == power.PermissionId)
            .ExecuteAsync();
        return QueryResult.Return(n > 0);
    }

    public virtual async Task<QueryResult> InsertPermissionAsync(TPower power)
    {
        var n = await context.Insert(power).ExecuteAsync();
        return QueryResult.Return(n > 0);
    }

    public virtual async Task<QueryResult> UpdateRoleAsync(TRole role)
    {
        var n = await context.Update(role)
            .Where(r => r.RoleId == role.RoleId)
            .ExecuteAsync();
        return n > 0;
    }

    public virtual async Task<QueryResult> InsertRoleAsync(TRole role)
    {
        using var scoped = context.CreateMainDbScoped();
        try
        {
            await scoped.BeginTransactionAsync();
            var n = await context.Insert(role).ExecuteAsync();
            var roleId = role.RoleId;
            string[] powers = [.. role.Permissions?.Distinct()];
            var i = 0;
            foreach (var p in powers)
            {
                var rp = new TRolePower() { RoleId = roleId, PermissionId = p };
                var ef = await scoped.Insert(rp).ExecuteAsync();
                i += ef;
            }
            if (n == 1 && i == powers.Length)
            {
                await scoped.CommitTransactionAsync();
                return QueryResult.Success();
            }
            else
            {
                await scoped.RollbackTransactionAsync();
                return QueryResult.Fail();
            }
        }
        catch (Exception ex)
        {
            await scoped.RollbackTransactionAsync();
            return QueryResult.Fail().SetMessage(ex.Message);
        }
    }

    public virtual async Task<QueryResult> DeleteRoleAsync(TRole role)
    {
        using var scoped = context.CreateMainDbScoped();
        try
        {
            await scoped.BeginTransactionAsync();
            await scoped.Delete<TRole>().Where(r => r.RoleId == role.RoleId).ExecuteAsync();
            await scoped.Delete<TUserRole>().Where(ur => ur.RoleId == role.RoleId).ExecuteAsync();
            await scoped.Delete<TRolePower>().Where(rp => rp.RoleId == role.RoleId).ExecuteAsync();
            await scoped.CommitTransactionAsync();
            return QueryResult.Success();
        }
        catch (Exception ex)
        {
            await scoped.RollbackTransactionAsync();
            return QueryResult.Fail().SetMessage(ex.Message);
        }

    }

    public virtual async Task<QueryResult> DeletePermissionAsync(TPower power)
    {
        using var scoped = context.CreateMainDbScoped();
        try
        {
            await scoped.BeginTransactionAsync();
            await scoped.Delete<TPower>().Where(p => p.PermissionId == power.PermissionId || p.ParentId == power.PermissionId).ExecuteAsync();
            await scoped.Delete<TRolePower>().Where(p => p.PermissionId == power.PermissionId).ExecuteAsync();
            await scoped.CommitTransactionAsync();
            return QueryResult.Success();
        }
        catch (Exception ex)
        {
            await scoped.RollbackTransactionAsync();
            return QueryResult.Fail().SetMessage(ex.Message);
        }
    }
    public virtual async Task<QueryCollectionResult<MinimalPermission>> GetUserPermissionsAsync(string usrId)
    {
        var powers = await context.Select<TPower>()
            .Distinct()
            .InnerJoin<TRolePower>(w => w.Tb1.PermissionId == w.Tb2.PermissionId)
            .InnerJoin<TUserRole>(w => w.Tb2.RoleId == w.Tb3.RoleId)
            .Where(w => w.Tb3.UserId == usrId)
            .OrderBy(w => w.Tb1.Sort)
            .ToListAsync(w => new MinimalPermission
            {
                PermissionId = w.Tb1.PermissionId,
                PermissionName = w.Tb1.PermissionName,
                ParentId = w.Tb1.ParentId,
                PermissionType = w.Tb1.PermissionType,
                PermissionLevel = w.Tb1.PermissionLevel,
                Icon = w.Tb1.Icon,
                //Path = w.Tb1.Path,
                Sort = w.Tb1.Sort
            });
        return powers.CollectionResult();
    }
}

#if (ExcludeDefaultService)
#else
[AutoInject(ServiceType = typeof(IStandardPermissionService), Group = "SERVER")]
[AutoInject(ServiceType = typeof(IPermissionService), Group = "SERVER")]
[GenAspectProxy]
public class StandardPermissionService : DefaultPermissionService<Permission, Role, RolePermission, UserRole>, IStandardPermissionService
{
    public StandardPermissionService(IExpressionContext context) : base(context)
    {

    }
}
#endif