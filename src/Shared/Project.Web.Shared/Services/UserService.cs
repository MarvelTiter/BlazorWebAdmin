﻿using AutoInjectGenerator;
using AutoWasmApiGenerator;
using LightORM;

namespace Project.Web.Shared.Services
{
    public class UserService<TUser> : IUserService<TUser> where TUser : IUser
    {
        private readonly IExpressionContext context;

        public UserService(IExpressionContext context)
        {
            this.context = context;
        }

        public async Task<QueryResult> DeleteUserAsync(TUser user)
        {
            await context.BeginTranAsync();
            await context.Delete<TUser>().Where(u => u.UserId == user.UserId).ExecuteAsync();
            await context.Delete<UserRole>().Where(ur => ur.UserId == user.UserId).ExecuteAsync();
            await context.CommitTranAsync();
            return true.Result();
        }

        public async Task<QueryCollectionResult<TUser>> GetUserListAsync(GenericRequest<TUser> req)
        {
            var list = await context.Repository<TUser>().GetListAsync(req.Expression(), out var count, req.PageIndex, req.PageSize);
            return list.CollectionResult((int)count);
        }

        public async Task<QueryResult> InsertUserAsync(TUser user)
        {
            var u = await context.Repository<TUser>().InsertAsync(user);
            return u.Result();
        }

        public async Task<QueryResult> ModifyUserPasswordAsync(string uid, string old, string pwd)
        {
            var flag = await context.Update<TUser>()
                .Set(u => u.Password, pwd)
                .Where(u => u.UserId == uid && u.Password == old)
                .ExecuteAsync();
            return flag.Result();
        }

        public async Task<QueryResult> UpdateUserAsync(TUser user)
        {
            var flag = await context.Repository<TUser>().UpdateAsync(user, u => u.UserId == user.UserId);
            return flag.Result();
        }
        public async Task<TUser?> GetUserAsync(string id)
        {
            var u = await context.Repository<TUser>().GetSingleAsync(u => u.UserId == id);
            return u;
        }
    }


#if(ExcludeDefaultService)
#else
    // [AutoInject(Group = "SERVER")]
    // public class UserService(IExpressionContext context) : IUserService
    // {
    //     public async Task<QueryResult> CheckUserPasswordAsync(string oldPassword)
    //     {
    //         throw new NotImplementedException();
    //     }
    //
    //     public async Task<QueryResult> ModifyUserPasswordAsync(UserPwd pwd)
    //     {
    //         var r = await context.Update<User>().Set(u => u.Password, pwd.Password).ExecuteAsync();
    //         return (r > 0).Result();
    //     }
    // }


    [AutoInject(Group = "SERVER", ServiceType = typeof(IStandardUserService))]
    public class StandardUserService : UserService<User>, IStandardUserService
    {
        public StandardUserService(IExpressionContext context) : base(context)
        {
        }
    }
#endif
}
