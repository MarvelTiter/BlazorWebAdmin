using LightORM.Extension;
using AutoInjectGenerator;
using LightORM;
namespace Project.Web.Shared.Services
{
    public class PermissionService<TPower, TRole, TRolePower, TUserRole>
        where TPower : class, IPower, new()
        where TRole : class, IRole, new()
        where TRolePower : class, IRolePower, new()
        where TUserRole : class, IUserRole, new()
    {
        private readonly IExpressionContext context;

        public PermissionService(IExpressionContext context)
        {
            this.context = context;
        }
        public async Task<QueryCollectionResult<TPower>> GetPowerListAsync(GenericRequest<TPower> req)
        {
            var list = await context.Repository<TPower>().GetListAsync(req.Expression(), out var total, req.PageIndex, req.PageSize, p => p.Sort);
            return list.CollectionResult((int)total);
        }

        public async Task<QueryCollectionResult<TPower>> GetAllPowerAsync()
        {
            var list = await context.Select<TPower>().OrderBy(e => e.Sort).ToListAsync();
            return list.CollectionResult();
        }

        public async Task<QueryCollectionResult<TRole>> GetRoleListAsync(GenericRequest<TRole> req)
        {
            var list = await context.Repository<TRole>().GetListAsync(req.Expression(), out var total, req.PageIndex, req.PageSize);
            return list.CollectionResult((int)total);
        }

        public async Task<QueryCollectionResult<TRole>> GetAllRoleAsync()
        {
            var list = await context.Repository<TRole>().GetListAsync(e => true);
            return list.CollectionResult();
        }

        public async Task<QueryCollectionResult<TPower>> GetPowerListByUserIdAsync(string usrId)
        {
            var powers = await context.Select<TPower, TRolePower, TUserRole>(w => new { w.Tb1.PowerId, w.Tb1.PowerName, w.Tb1.ParentId, w.Tb1.PowerType, w.Tb1.PowerLevel, w.Tb1.Icon, w.Tb1.Path, w.Tb1.Sort })
                                      .Distinct()
                                      .InnerJoin<TRolePower>(w => w.Tb1.PowerId == w.Tb2.PowerId)
                                      .InnerJoin<TUserRole>(w => w.Tb2.RoleId == w.Tb3.RoleId)
                                      .Where(w => w.Tb3.UserId == usrId)
                                      .OrderBy(w => w.Tb1.Sort)
                                      .ToListAsync();
            return powers.CollectionResult();
        }

        public async Task<QueryCollectionResult<TPower>> GetPowerListByRoleIdAsync(string roleId)
        {
            var powers = await context.Select<TPower, TRolePower>()
                                      .InnerJoin<TRolePower>((r, p) => p.PowerId == r.PowerId)
                                      .Where((r, rp) => rp.RoleId == roleId)
                                      .ToListAsync();
            return powers.CollectionResult();
        }

        public async Task<QueryCollectionResult<TRole>> GetUserRolesAsync(string usrId)
        {
            var roles = await context.Select<TRole, TUserRole>()
                                     .InnerJoin<TUserRole>((r, ur) => r.RoleId == ur.RoleId)
                                     .Where<TUserRole>(ur => ur.UserId == usrId)
                                     .ToListAsync();
            return roles.CollectionResult();
        }

        public async Task<QueryResult<bool>> SaveUserRoleAsync(KeyRelations<string, string> relations)
        {
            try
            {
                await context.BeginTranAsync();
                var usrId = relations.Main;
                var roles = relations.Slaves ?? [];
                await context.Delete<TUserRole>().Where(u => u.UserId == usrId).ExecuteAsync();
                foreach (var r in roles)
                {
                    var ur = new TUserRole() { UserId = usrId, RoleId = r };
                    await context.Insert(ur).ExecuteAsync();
                }
                await context.CommitTranAsync();
                return true.Result();
            }
            catch (Exception ex)
            {
                await context.RollbackTranAsync();
                return false.Result().SetMessage(ex.Message);
            }
        }

        public async Task<QueryResult<bool>> SaveRolePowerAsync(KeyRelations<string, string> relations)
        {
            try
            {
                await context.BeginTranAsync();
                var roleId = relations.Main;
                var powers = relations.Slaves ?? [];
                var n = await context.Delete<TRolePower>().Where(r => r.RoleId == roleId).ExecuteAsync();
                foreach (var p in powers)
                {
                    var rp = new TRolePower() { RoleId = roleId, PowerId = p };
                    var ef = await context.Insert<TRolePower>(rp).ExecuteAsync();
                    n += ef;
                }
                await context.CommitTranAsync();
                return true.Result();
            }
            catch (Exception ex)
            {
                await context.RollbackTranAsync();
                return false.Result().SetMessage(ex.Message);
            }
        }

        public async Task<QueryResult<bool>> UpdatePowerAsync(TPower power)
        {
            var n = await context.Repository<TPower>().UpdateAsync(power, p => p.PowerId == power.PowerId);
            return (n > 0).Result();
        }

        public async Task<QueryResult<bool>> InsertPowerAsync(TPower power)
        {
            var n = await context.Repository<TPower>().InsertAsync(power);
            return (n > 0).Result();
        }

        public async Task<QueryResult<bool>> UpdateRoleAsync(TRole role)
        {
            var n = await context.Repository<TRole>().UpdateAsync(role, r => r.RoleId == role.RoleId);
            return (n > 0).Result();
        }

        public async Task<QueryResult<bool>> InsertRoleAsync(TRole role)
        {
            var n = await context.Repository<TRole>().InsertAsync(role);
            return (n > 0).Result();
        }

        public async Task<QueryResult<bool>> DeleteRoleAsync(TRole role)
        {
            try
            {
                await context.BeginTranAsync();
                await context.Delete<TRole>().Where(r => r.RoleId == role.RoleId).ExecuteAsync();
                await context.Delete<TUserRole>().Where(ur => ur.RoleId == role.RoleId).ExecuteAsync();
                await context.Delete<TRolePower>().Where(rp => rp.RoleId == role.RoleId).ExecuteAsync();
                await context.CommitTranAsync();
                return true.Result();
            }
            catch (Exception ex)
            {
                await context.RollbackTranAsync();
                return false.Result().SetMessage(ex.Message);
            }

        }

        public async Task<QueryResult<bool>> DeletePowerAsync(TPower power)
        {
            try
            {
                await context.BeginTranAsync();
                await context.Delete<TPower>().Where(p => p.PowerId == power.PowerId || p.ParentId == power.PowerId).ExecuteAsync();
                await context.Delete<TRolePower>().Where(p => p.PowerId == power.PowerId).ExecuteAsync();
                await context.CommitTranAsync();
                return true.Result();
            }
            catch (Exception ex)
            {
                await context.RollbackTranAsync();
                return false.Result().SetMessage(ex.Message);
            }

        }
    }

    [AutoInject(Group = "SERVER")]
    public class PermissionService : IPermissionService

    {
        private readonly IExpressionContext context;

        public PermissionService(IExpressionContext context)
        {
            this.context = context;
        }
        public async Task<QueryCollectionResult<IPower>> GetPowerListByUserIdAsync(string usrId)
        {
            var powers = await context.Select<Power, RolePower, UserRole>(w => new { w.Tb1.PowerId, w.Tb1.PowerName, w.Tb1.ParentId, w.Tb1.PowerType, w.Tb1.PowerLevel, w.Tb1.Icon, w.Tb1.Path, w.Tb1.Sort })
                                      .Distinct()
                                      .InnerJoin<RolePower>(w => w.Tb1.PowerId == w.Tb2.PowerId)
                                      .InnerJoin<UserRole>(w => w.Tb2.RoleId == w.Tb3.RoleId)
                                      .Where(w => w.Tb3.UserId == usrId)
                                      .OrderBy(w => w.Tb1.Sort)
                                      .ToListAsync();
            return powers.Cast<IPower>().CollectionResult();
        }
    }


    [AutoInject(ServiceType = typeof(IStandardPermissionService), Group = "SERVER")]
    public class StandardPermissionService : PermissionService<Power, Role, RolePower, UserRole>, IStandardPermissionService
    {
        public StandardPermissionService(IExpressionContext context) : base(context)
        {

        }
    }
}