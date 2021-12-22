using BlazorWebAdmin.IRepositories;
using BlazorWebAdmin.Models.Entities;
using BlazorWebAdmin.Models.Request;

namespace BlazorWebAdmin.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {

    }
}
