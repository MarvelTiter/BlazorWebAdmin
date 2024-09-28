using AutoAopProxyGenerator;
using AutoInjectGenerator;
using AutoWasmApiGenerator;
using LightORM;

namespace Project.Web.Shared.Services
{
    public class DefaultUserService<TUser, TUserRole>
        where TUser : IUser
        where TUserRole : IUserRole
    {
        private readonly IExpressionContext context;

        public DefaultUserService(IExpressionContext context)
        {
            this.context = context;
        }

        public async Task<QueryResult> DeleteUserAsync(TUser user)
        {
            await context.BeginTranAsync();
            await context.Delete<TUser>().Where(u => u.UserId == user.UserId).ExecuteAsync();
            await context.Delete<TUserRole>().Where(ur => ur.UserId == user.UserId).ExecuteAsync();
            await context.CommitTranAsync();
            return true.Result();
        }

        public async Task<QueryCollectionResult<TUser>> GetUserListAsync(GenericRequest<TUser> req)
        {
            var list = await context.Select<TUser>()
                .Where(req.Expression())
                .Count(out var count)
                .Paging(req.PageIndex, req.PageSize)
                .ToListAsync();
            return list.CollectionResult((int)count);
        }

        public async Task<QueryResult> InsertUserAsync(TUser user)
        {
            var u = await context.Insert(user).ExecuteAsync();
            return u > 0;
        }

        public async Task<QueryResult> ModifyUserPasswordAsync(string uid, string old, string pwd)
        {
            var flag = await context.Update<TUser>()
                .Set(u => u.Password, pwd)
                .Where(u => u.UserId == uid && u.Password == old)
                .ExecuteAsync();
            return flag > 0;
        }

        public async Task<QueryResult> UpdateUserAsync(TUser user)
        {
            var flag = await context.Update(user).Where( u => u.UserId == user.UserId).ExecuteAsync();
            return flag > 0;
        }
        public async Task<TUser?> GetUserAsync(string id)
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
