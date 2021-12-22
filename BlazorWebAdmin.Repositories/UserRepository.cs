using BlazorWebAdmin.Models.Entities;
using BlazorWebAdmin.Models.Request;

namespace BlazorWebAdmin.IRepositories
{
    public class UserRepository : IUserRepository
    {
        public bool Delete(User item)
        {
            throw new NotImplementedException();
        }

        public User Insert(User item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetList(UserReq req)
        {
            throw new NotImplementedException();
        }

        public bool Update(User item)
        {
            throw new NotImplementedException();
        }

        public User GetSingle(UserReq req)
        {
            return new User
            {
                UserId = "Admin",
                UserName = "管理员",
            };
        }
    }
}
