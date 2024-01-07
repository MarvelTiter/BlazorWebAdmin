using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Project.Models.Entities.Permissions;

namespace Project.AppCore.Services
{
    public partial class UserService : IUserService
    {
        private readonly IExpressionContext context;

        public UserService(IExpressionContext context)
        {
            this.context = context;
        }

        public async Task<IQueryResult> DeleteUserAsync(User user)
        {
            var trans = context.BeginTransaction();
            trans.Delete<User>().Where(u => u.UserId == user.UserId).AttachTransaction();
            trans.Delete<UserRole>().Where(ur => ur.UserId == user.UserId).AttachTransaction();
            var result = await trans.CommitTransactionAsync();
            return result.Result();
        }

        public async Task<IQueryCollectionResult<User>> GetUserListAsync(GenericRequest<User> req)
        {
            var list = await context.Repository<User>().GetListAsync(req.Expression, out var count, req.PageIndex, req.PageSize);
            return list.CollectionResult((int)count);
        }

        public async Task<IQueryResult> InsertUserAsync(User user)
        {
            var u = await context.Repository<User>().InsertAsync(user);
            return u.Result();
        }

        public async Task<IQueryResult> ModifyUserPasswordAsync(string uid, string old, string pwd)
        {
            var flag = await context.Update<User>()
                .Set(u => u.Password, pwd)
                .Where(u => u.UserId == uid && u.Password == old)
                .ExecuteAsync();
            return flag.Result();
        }

        public async Task<IQueryResult> UpdateUserAsync(User user)
        {
            var flag = await context.Repository<User>().UpdateAsync(user, u => u.UserId == user.UserId);
            return flag.Result();
        }
        public async Task<User> GetUserAsync(string id)
        {
            var u = await context.Repository<User>().GetSingleAsync(u => u.UserId == id);
            return u ?? null;
        }
    }
}
