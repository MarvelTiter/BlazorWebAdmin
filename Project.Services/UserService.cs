using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;
using Project.Repositories.interfaces;
using Project.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
    public partial class UserService : IUserService
    {
        private readonly IRepository repository;

        public UserService(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IQueryCollectionResult<User>> GetUserListAsync(GeneralReq<User> req)
        {
            var count = await repository.Table<User>().GetCountAsync(req.Expression);
            var list = await repository.Table<User>().GetListAsync(req.Expression, req.PageIndex, req.PageSize);
            return QueryResult.Success<User>().CollectionResult(list, count);
        }

        public async Task<User> InsertUserAsync(User user)
        {
            return await repository.Table<User>().InsertAsync(user);
        }

        public Task<int> UpdateUserAsync(User user)
        {
            return repository.Table<User>().UpdateAsync(user, u => u.UserId == user.UserId);
        }
    }
}
