using MDbContext;
using MDbContext.Context.Extension;
using Project.Models;
using Project.Models.Permissions;
using Project.Models.Request;
using Project.Repositories.interfaces;
using Project.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
    public class PemissionService : IPemissionService
    {
        private readonly IRepository repository;

        public PemissionService(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<QueryResult<PagingResult<Power>>> GetPowerListAsync(GeneralReq<Power> req)
        {
            var count = await repository.Table<Power>().GetCountAsync(req.Expression);
            var list = await repository.Table<Power>().GetListAsync(req.Expression, req.PageIndex, req.PageSize);
            return QueryResult<Power>.PagingResult(list, count);
        }

        public Task<IEnumerable<Power>> GetPowerListAsync()
        {
            return repository.Table<Power>().GetListAsync(e => true);
        }

        public async Task<QueryResult<PagingResult<Role>>> GetRoleListAsync(GeneralReq<Role> req)
        {
            var count = await repository.Table<Role>().GetCountAsync(req.Expression);
            var list = await repository.Table<Role>().GetListAsync(req.Expression, req.PageIndex, req.PageSize);
            return QueryResult<Role>.PagingResult(list, count);
        }

        public Task<IEnumerable<Role>> GetRoleListAsync()
        {
            return repository.Table<Role>().GetListAsync(e => true);
        }

        public async Task<QueryResult<IEnumerable<Power>>> GetPowerListByUserIdAsync(string usrId)
        {
            var powers = await repository.Query().Select<RolePower, Power>(distinct: true)
                                .InnerJoin<Power>((r, p) => p.PowerId == r.PowerId)
                                .InnerJoin<UserRole>((u, r) => u.RoleId == r.RoleId)
                                .Where<UserRole>(u => u.UserId == usrId)
                                .ToListAsync<Power>();
            return QueryResult<IEnumerable<Power>>.SuccessResult(powers);
        }

        public async Task<QueryResult<IEnumerable<Power>>> GetPowerListByRoleIdAsync(string roleId)
        {
            var powers = await repository.Query().Select<RolePower, Power>()
                                 .InnerJoin<Power>((r, p) => p.PowerId == r.PowerId)
                                 .Where(r => r.RoleId == roleId)
                                 .ToListAsync<Power>();
            return QueryResult<IEnumerable<Power>>.SuccessResult(powers);
        }

        public Task<bool> SaveUserRole(string usrId, params string[] roles)
        {
            var db = repository.Context();
            db.DbSet.Delete<UserRole>().Where(u => u.UserId == usrId);
            db.AddTrans();
            foreach (var r in roles)
            {
                var ur = new UserRole() { UserId = usrId, RoleId = r };
                db.DbSet.Insert(ur);
                db.AddTrans();
            }
            return db.ExecuteTransAsync();
        }

        public Task<bool> SaveRolePower(string roleId, params string[] powers)
        {
            var db = repository.Context();
            db.DbSet.Delete<RolePower>().Where(r => r.RoleId == roleId);
            db.AddTrans();
            foreach (var p in powers)
            {
                var rp = new RolePower() { RoleId = roleId, PowerId = p };
                db.DbSet.Insert(rp);
                db.AddTrans();
            }
            return db.ExecuteTransAsync();
        }

        public Task<int> UpdatePower(Power power)
        {
            return repository.Table<Power>().UpdateAsync(power, p => p.PowerId == power.PowerId);
        }

    }
}
