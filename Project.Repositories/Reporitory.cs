using DExpSql;
using Project.IRepositories;
using Project.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Repositories
{
    public class Reporitory : IRepository
    {
        private readonly IServiceProvider provider;

        public Reporitory(IServiceProvider provider)
        {
            this.provider = provider;
        }
        public ExpressionSql Query()
        {
            throw new NotImplementedException();
        }

        public IRepositoryBase<T> Table<T>()
        {
            var type = typeof(IRepositoryBase<>).MakeGenericType(typeof(T));
            var repo= provider.GetService(type) as IRepositoryBase<T>;
            return repo;
        }
    }
}
