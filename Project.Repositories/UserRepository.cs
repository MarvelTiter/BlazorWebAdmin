using Project.IRepositories;
using Project.Models.Entities;
using System.Linq.Expressions;

namespace Project.Repositories
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
