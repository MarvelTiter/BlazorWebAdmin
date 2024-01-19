using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Project.Constraints.Models.Permissions;

namespace Project.AppCore.Services
{
    [IgnoreAutoInject]
    public partial class UserService<TUser> : IUserService<TUser> where TUser : IUser
    {
        private readonly IExpressionContext context;

        public UserService(IExpressionContext context)
        {
            this.context = context;
        }

        public async Task<IQueryResult> DeleteUserAsync(TUser user)
        {
            var trans = context.BeginTransaction();
            trans.Delete<TUser>().Where(u => u.UserId == user.UserId).AttachTransaction();
            trans.Delete<UserRole>().Where(ur => ur.UserId == user.UserId).AttachTransaction();
            var result = await trans.CommitTransactionAsync();
            return result.Result();
        }

        public async Task<IQueryCollectionResult<TUser>> GetUserListAsync(GenericRequest<TUser> req)
        {
            var list = await context.Repository<TUser>().GetListAsync(req.Expression, out var count, req.PageIndex, req.PageSize);
            return list.CollectionResult((int)count);
        }

        public async Task<IQueryResult> InsertUserAsync(TUser user)
        {
            var u = await context.Repository<TUser>().InsertAsync(user);
            return u.Result();
        }

        public async Task<IQueryResult> ModifyUserPasswordAsync(string uid, string old, string pwd)
        {
            var flag = await context.Update<TUser>()
                .Set(u => u.Password, pwd)
                .Where(u => u.UserId == uid && u.Password == old)
                .ExecuteAsync();
            return flag.Result();
        }

        public async Task<IQueryResult> UpdateUserAsync(TUser user)
        {
            var flag = await context.Repository<TUser>().UpdateAsync(user, u => u.UserId == user.UserId);
            return flag.Result();
        }
        public async Task<TUser> GetUserAsync(string id)
        {
            var u = await context.Repository<TUser>().GetSingleAsync(u => u.UserId == id);
            return u;
        }
    }
}
