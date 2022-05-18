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
    public class PemissionService : IPermissionService
    {
        private readonly IRepository repository;

        public PemissionService(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IQueryCollectionResult<Power>> GetPowerListAsync(GeneralReq<Power> req)
        {
            var count = await repository.Table<Power>().GetCountAsync(req.Expression);
            var list = await repository.Table<Power>().GetListAsync(req.Expression, req.PageIndex, req.PageSize);
            return QueryResult.Success<Power>().CollectionResult(list, count);
        }

        public async Task<IQueryCollectionResult<Power>> GetPowerListAsync()
        {
            var list = await repository.Table<Power>().GetListAsync(e => true);
            return QueryResult.Success<Power>().CollectionResult(list);
        }

        public async Task<IQueryCollectionResult<Role>> GetRoleListAsync(GeneralReq<Role> req)
        {
            var count = await repository.Table<Role>().GetCountAsync(req.Expression);
            var list = await repository.Table<Role>().GetListAsync(req.Expression, req.PageIndex, req.PageSize);
            return QueryResult.Success<Role>().CollectionResult(list, count);
        }

        public async Task<IQueryCollectionResult<Role>> GetRoleListAsync()
        {
            var list =await repository.Table<Role>().GetListAsync(e => true);
            return QueryResult.Success<Role>().CollectionResult(list);
        }

        public async Task<IQueryCollectionResult<Power>> GetPowerListByUserIdAsync(string usrId)
        {
            var powers = await repository.Query().Select<RolePower, Power>(distinct: true)
                                .InnerJoin<Power>((r, p) => p.PowerId == r.PowerId)
                                .InnerJoin<UserRole>((u, r) => u.RoleId == r.RoleId)
                                .Where<UserRole>(u => u.UserId == usrId)
                                .OrderByAsc<Power>(p => p.Sort)
                                .ToListAsync<Power>();
            return QueryResult.Success<Power>().CollectionResult(powers);
        }

        public async Task<IQueryCollectionResult<Power>> GetPowerListByRoleIdAsync(string roleId)
        {
            var powers = await repository.Query().Select<RolePower, Power>()
                                 .InnerJoin<Power>((r, p) => p.PowerId == r.PowerId)
                                 .Where(r => r.RoleId == roleId)
                                 .ToListAsync<Power>();
            return QueryResult.Success<Power>().CollectionResult(powers);
        }

        public async Task<IQueryCollectionResult<Role>> GetUserRolesAsync(string usrId)
        {
            var roles =await repository.Query().Select<Role>()
                .InnerJoin<UserRole>((r, ur) => r.RoleId == ur.RoleId)
                .Where<UserRole>(ur => ur.UserId == usrId)
                .ToListAsync<Role>();
            return QueryResult.Success<Role>().CollectionResult(roles);
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

        public Task<int> UpdatePowerAsync(Power power)
        {
            return repository.Table<Power>().UpdateAsync(power, p => p.PowerId == power.PowerId);
        }

        public Task<Power> InsertPowerAsync(Power power)
        {
            return repository.Table<Power>().InsertAsync(power);
        }

        public Task<int> UpdateRoleAsync(Role role)
        {
            return repository.Table<Role>().UpdateAsync(role, r => r.RoleId == role.RoleId);
        }

        public Task<Role> InsertRoleAsync(Role role)
        {
            return repository.Table<Role>().InsertAsync(role);
        }
    }
}
