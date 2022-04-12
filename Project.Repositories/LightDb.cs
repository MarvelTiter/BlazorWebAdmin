using MDbContext;
using Project.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Repositories
{
    [IgnoreAutoInject]
    public class LightDb
    {
        public static readonly string ConnectString = "";
        public LightDb()
        {
            DbContext.Init(DbBaseType.Oracle);
            CreateDbContext();
        }
        public DbContext Db => CreateDbContext();

        private DbContext CreateDbContext()
        {
            throw new NotImplementedException();
        }
        ~LightDb()
        {
            Db?.Dispose();
        }
    }
}
