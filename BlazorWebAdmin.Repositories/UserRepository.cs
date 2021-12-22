using BlazorWebAdmin.IRepositories;
using BlazorWebAdmin.Models.Entities;
using BlazorWebAdmin.Models.Request;

namespace BlazorWebAdmin.Repositories
{
    public class UserRepository : IUserRepository
    {      
        public Task<User> InsertAsync(User item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(User item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(User item)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetSingleAsync(UserReq req)
        {
            return Task.FromResult(new User
            {
                UserId = "Admin",
                UserName = "管理员",
            });
        }

        public Task<IEnumerable<User>> GetListAsync(UserReq req)
        {
            throw new NotImplementedException();
        }
    }
}
