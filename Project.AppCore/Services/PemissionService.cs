using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Project.Constraints.Models;
using Project.Constraints.Models.Permissions;
using Project.Constraints.Models.Request;
using Project.Constraints.Services;

namespace Project.AppCore.Services
{
    [IgnoreAutoInject]
    public partial class PemissionService<TPower, TRole, TRolePower, TUserRole> : IPermissionService<TPower, TRole>
        where TPower : class, IPower, new()
        where TRole : class, IRole, new()
        where TRolePower : class, IRolePower, new()
        where TUserRole : class, IUserRole, new()
    {
        private readonly IExpressionContext context;

        public PemissionService(IExpressionContext context)
        {
            this.context = context;
        }

        public async Task<IQueryCollectionResult<TPower>> GetPowerListAsync(GenericRequest<TPower> req)
        {
            var list = await context.Repository<TPower>().GetListAsync(req.Expression, out var total, req.PageIndex, req.PageSize, p => p.Sort);
            return list.CollectionResult((int)total);
        }

        public async Task<IQueryCollectionResult<TPower>> GetPowerListAsync()
        {
            var list = await context.Repository<TPower>().GetListAsync(e => true, e => e.Sort);
            return list.CollectionResult();
        }

        public async Task<IQueryCollectionResult<TRole>> GetRoleListAsync(GenericRequest<TRole> req)
        {
            var list = await context.Repository<TRole>().GetListAsync(req.Expression, out var total, req.PageIndex, req.PageSize);
            return list.CollectionResult((int)total);
        }

        public async Task<IQueryCollectionResult<TRole>> GetRoleListAsync()
        {
            var list = await context.Repository<TRole>().GetListAsync(e => true);
            return list.CollectionResult();
        }

        public async Task<IQueryCollectionResult<TPower>> GetPowerListByUserIdAsync(string usrId)
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

        public async Task<IQueryCollectionResult<TPower>> GetPowerListByRoleIdAsync(string roleId)
        {
            var powers = await context.Select<TPower, TRolePower>()
                                      .InnerJoin<TRolePower>((r, p) => p.PowerId == r.PowerId)
                                      .Where((r, rp) => rp.RoleId == roleId)
                                      .ToListAsync();
            return powers.CollectionResult();
        }

        public async Task<IQueryCollectionResult<TRole>> GetUserRolesAsync(string usrId)
        {
            var roles = await context.Select<TRole, TUserRole>()
                                     .InnerJoin<TUserRole>((r, ur) => r.RoleId == ur.RoleId)
                                     .Where<TUserRole>(ur => ur.UserId == usrId)
                                     .ToListAsync();
            return roles.CollectionResult();
        }

        public async Task<IQueryResult<bool>> SaveUserRole(string usrId, params string[] roles)
        {
            var db = context.BeginTransaction();
            db.Delete<TUserRole>().Where(u => u.UserId == usrId).AttachTransaction();
            foreach (var r in roles)
            {
                var ur = new TUserRole() { UserId = usrId, RoleId = r };
                db.Insert<TUserRole>().AppendData(ur).AttachTransaction();
            }
            var n = await db.CommitTransactionAsync();
            return n.Result();
        }

        public async Task<IQueryResult<bool>> SaveRolePower(string roleId, params string[] powers)
        {
            var db = context.BeginTransaction();
            db.Delete<TRolePower>().Where(r => r.RoleId == roleId).AttachTransaction();
            foreach (var p in powers)
            {
                var rp = new TRolePower() { RoleId = roleId, PowerId = p };
                db.Insert<TRolePower>().AppendData(rp).AttachTransaction();
            }
            var n = await db.CommitTransactionAsync();
            return n.Result();
        }

        public async Task<IQueryResult<bool>> UpdatePowerAsync(TPower power)
        {
            var n = await context.Repository<TPower>().UpdateAsync(power, p => p.PowerId == power.PowerId);
            return (n > 0).Result();
        }

        public async Task<IQueryResult<bool>> InsertPowerAsync(TPower power)
        {
            var n = await context.Repository<TPower>().InsertAsync(power);
            return (n != null).Result();
        }

        public async Task<IQueryResult<bool>> UpdateRoleAsync(TRole role)
        {
            var n = await context.Repository<TRole>().UpdateAsync(role, r => r.RoleId == role.RoleId);
            return (n > 0).Result();
        }

        public async Task<IQueryResult<bool>> InsertRoleAsync(TRole role)
        {
            var n = await context.Repository<TRole>().InsertAsync(role);
            return (n != null).Result();
        }

        public async Task<IQueryResult<bool>> DeleteRoleAsync(TRole role)
        {
            var trans = context.BeginTransaction();
            trans.Delete<TRole>().Where(r => r.RoleId == role.RoleId).AttachTransaction();
            trans.Delete<TUserRole>().Where(ur => ur.RoleId == role.RoleId).AttachTransaction();
            trans.Delete<TRolePower>().Where(rp => rp.RoleId == role.RoleId).AttachTransaction();
            var flag = await trans.CommitTransactionAsync();
            return flag.Result();
        }

        public async Task<IQueryResult<bool>> DeletePowerAsync(TPower power)
        {
            var trans = context.BeginTransaction();
            trans.Delete<TPower>().Where(p => p.PowerId == power.PowerId || p.ParentId == power.PowerId).AttachTransaction();
            trans.Delete<TRolePower>().Where(p => p.PowerId == power.PowerId).AttachTransaction();
            var flag = await trans.CommitTransactionAsync();
            return flag.Result();
        }

        async Task<IQueryCollectionResult<IPower>> IPermissionService.GetPowerListByUserIdAsync(string usrId)
        {
            var powers = await GetPowerListByUserIdAsync(usrId);
            var ips = powers.Payload.Cast<IPower>();
            return ips.CollectionResult();
        }
    }
}
