using BlazorWebAdmin.IRepositories;
using BlazorWebAdmin.Models.Entities;
using BlazorWebAdmin.Models.Request;
using System.Linq.Expressions;

namespace BlazorWebAdmin.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public override Task<User> GetSingleAsync(Expression<Func<User, bool>> whereExpression)
        {
            return Task.FromResult(new User
            {
                UserId = "Admin",
                UserName = "管理员",
            });
        }
    }
}
