using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Services
{
    public partial interface IUserService
    {
        Task<IQueryCollectionResult<User>> GetUserListAsync(GeneralReq<User> req);
        Task<User> InsertUserAsync(User user);
        Task<int> UpdateUserAsync(User user);
    }
}
