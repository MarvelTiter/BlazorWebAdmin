using MDbContext.ExpressionSql;
using MDbContext.Repository;
using Project.AppCore.Repositories;
using Project.AppCore.Services;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;

namespace Project.Services
{
    public partial class UserService : IUserService
    {
        private readonly IExpSql context;

        public UserService(IExpSql context)
        {
            this.context = context;
        }

        public async Task<IQueryCollectionResult<User>> GetUserListAsync(GenericRequest<User> req)
        {
            var list = await context.Repository<User>().GetListAsync(req.Expression, out var count, req.PageIndex, req.PageSize);
            return QueryResult.Success<User>().CollectionResult(list, (int)count);
        }

        public async Task<User> InsertUserAsync(User user)
        {
            return await context.Repository<User>().InsertAsync(user);
        }

        public Task<int> UpdateUserAsync(User user)
        {
            return context.Repository<User>().UpdateAsync(user, u => u.UserId == user.UserId);
        }
    }
}
