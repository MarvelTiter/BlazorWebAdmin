using DExpSql;
using MDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Repositories
{
    public interface IRepository
    {
        IRepositoryBase<T> Table<T>();
        ExpressionSql Query();
        DbContext Context();
    }
}
