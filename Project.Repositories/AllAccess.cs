using Project.IRepositories;
using System.Linq.Expressions;

namespace Project.Repositories
{
    public sealed class AllAccess : RepositoryBase<object>, IAllAccess
    {
        public override Task<int> DeleteAsync(Expression<Func<object, bool>> whereExpression)
        {
            throw new Exception("AllAccess请调用Request");
        }
        public override Task<IEnumerable<object>> GetListAsync(Expression<Func<object, bool>> whereExpression, int from, int to)
        {
            throw new Exception("AllAccess请调用Request");
        }
        public override Task<object> GetSingleAsync(Expression<Func<object, bool>> whereExpression)
        {
            throw new Exception("AllAccess请调用Request");
        }
        public override Task<object> InsertAsync(object item)
        {
            throw new Exception("AllAccess请调用Request");
        }
        public override Task<int> UpdateAsync(Expression<Func<object>> updateExpression, Expression<Func<object, bool>> whereExpression)
        {
            throw new Exception("AllAccess请调用Request");
        }
        public override Task<int> UpdateAsync(object item, Expression<Func<object, bool>> whereExpression)
        {
            throw new Exception("AllAccess请调用Request");
        }
    }
}
