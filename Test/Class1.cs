using Project.Constraints.Models;
using Project.Constraints.Services;

namespace Test
{
    public class Class1 : IAddtionalTnterceptor
    {
        public Task LoginSuccessAsync(IQueryResult<UserInfo> result)
        {
            Console.WriteLine(result.Payload.UserName);
            return Task.CompletedTask;
        }
    }
}
