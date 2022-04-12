using DExpSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.IRepositories
{
    public interface IRepository
    {
        IRepositoryBase<T> Table<T>();
        ExpressionSql Query();
    }
}
