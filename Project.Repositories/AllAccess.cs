using DExpSql;
using Project.IRepositories;
using System.Data;
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
        public override Task<IEnumerable<M>> Query<M>()
        {
            throw new Exception("AllAccess请调用Request");
        }
        public override ExpressionSqlCore<object> Select()
        {
            throw new Exception("AllAccess请调用Request");
        }

        public override Task<M> Single<M>()
        {
            throw new Exception("AllAccess请调用Request");
        }

        public override Task<DataTable> Table()
        {
            throw new Exception("AllAccess请调用Request");
        }

        public override ExpressionSqlCore<object> Count()
        {
            throw new Exception("AllAccess请调用Request");
        }
    }
}
