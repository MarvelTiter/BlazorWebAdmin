using DExpSql;
using MDbContext;
using Project.AppCore.Repositories;

namespace Project.AppCore
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
            return CreateDbContext().DbSet;
        }

        public IRepositoryBase<T> Table<T>()
        {
            var type = typeof(IRepositoryBase<>).MakeGenericType(typeof(T));
            var repo = provider.GetService(type) as IRepositoryBase<T>;
            return repo;
        }
        public DbContext Context()
        {
            return CreateDbContext();
        }
    }
}
