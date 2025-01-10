using AutoAopProxyGenerator;
using AutoInjectGenerator;
using AutoWasmApiGenerator;
using LightORM;
using Project.Constraints.Models.Permissions;

namespace Project.Web.Shared.Services
{
    public class DefaultUserService<TUser, TUserRole>
        where TUser : IUser
        where TUserRole : IUserRole, new()
    {
        private readonly IExpressionContext context;

        public DefaultUserService(IExpressionContext context)
        {
            this.context = context;
        }

        public virtual async Task<QueryResult> DeleteUserAsync(TUser user)
        {
            context.BeginTran();
            await context.Delete<TUser>().Where(u => u.UserId == user.UserId).ExecuteAsync();
            await context.Delete<TUserRole>().Where(ur => ur.UserId == user.UserId).ExecuteAsync();
            await context.CommitTranAsync();
            return true.Result();
        }

        public virtual async Task<QueryCollectionResult<TUser>> GetUserListAsync(GenericRequest<TUser> req)
        {
            var list = await context.Select<TUser>()
                .Where(req.Expression())
                .Count(out var count)
                .Paging(req.PageIndex, req.PageSize)
                .ToListAsync();
            return list.CollectionResult((int)count);
        }

        public virtual async Task<QueryResult> InsertUserAsync(TUser user)
        {
            using var scoped = context.CreateScoped();
            try
            {
                var u = await context.Insert(user).ExecuteAsync();
                var roles = user.Roles ?? [];
                var usrId = user.UserId;
                await scoped.Delete<TUserRole>().Where(u => u.UserId == usrId).ExecuteAsync();
                foreach (var r in roles)
                {
                    var ur = new TUserRole() { UserId = usrId, RoleId = r };
                    await scoped.Insert(ur).ExecuteAsync();
                }
                await scoped.CommitTranAsync();
                return QueryResult.Success();
            }
            catch (Exception ex)
            {
                await scoped.RollbackTranAsync();
                return QueryResult.Fail().SetMessage(ex.Message);
            }
        }

        public virtual async Task<QueryResult> ModifyUserPasswordAsync(string uid, string old, string pwd)
        {
            var flag = await context.Update<TUser>()
                .Set(u => u.Password, pwd)
                .Where(u => u.UserId == uid && u.Password == old)
                .ExecuteAsync();
            return flag > 0;
        }
        public virtual async Task<QueryResult> SaveUserWithRolesAsync(TUser user)
        {
            using var scoped = context.CreateScoped();
            try
            {
                //context.BeginTran();
                await scoped.Update(user).Where(u => u.UserId == user.UserId).ExecuteAsync();
                var usrId = user.UserId;
                var roles = user.Roles ?? [];
                await scoped.Delete<TUserRole>().Where(u => u.UserId == usrId).ExecuteAsync();
                foreach (var r in roles)
                {
                    var ur = new TUserRole() { UserId = usrId, RoleId = r };
                    await scoped.Insert(ur).ExecuteAsync();
                }
                await scoped.CommitTranAsync();
                return QueryResult.Success();
            }
            catch (Exception ex)
            {
                await scoped.RollbackTranAsync();
                return QueryResult.Fail().SetMessage(ex.Message);
            }
        }
        public virtual async Task<QueryResult> UpdateUserAsync(TUser user)
        {
            var n = await context.Update(user).Where(u => u.UserId == user.UserId).ExecuteAsync();
            return n > 0;
        }
        public virtual async Task<TUser?> GetUserAsync(string id)
        {
            var u = await context.Select<TUser>().Where(u => u.UserId == id).FirstAsync();
            return u;
        }
    }


#if (ExcludeDefaultService)
#else
    [AutoInject(Group = "SERVER", ServiceType = typeof(IStandardUserService))]
    [GenAspectProxy]
    public class StandardUserService : DefaultUserService<User, UserRole>, IStandardUserService
    {
        public StandardUserService(IExpressionContext context) : base(context)
        {
        }
    }
#endif
}
