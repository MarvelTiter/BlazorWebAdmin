using DExpSql;
using MDbContext;
using Project.Repositories.interfaces;

namespace Project.Repositories
{
    public class Reporitory : LightDb, IRepository
    {
        private readonly IServiceProvider provider;

        public Reporitory(IServiceProvider provider)
        {
            this.provider = provider;
        }
        public ExpressionSql Query()
        {
            return Db.DbSet;
        }

        public IRepositoryBase<T> Table<T>()
        {
            var type = typeof(IRepositoryBase<>).MakeGenericType(typeof(T));
            var repo = provider.GetService(type) as IRepositoryBase<T>;
            return repo;
        }
        public DbContext Context()
        {
            return Db;
        }
    }
}
