using MDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Repositories
{
    public class LightDb
    {
        public static readonly string ConnectString = "";
        private DbContext _dbContext;
        public LightDb()
        {
            DbContext.Init(DbBaseType.Oracle);
            CreateDbContext();
        }
        public DbContext Db => _dbContext;

        private void CreateDbContext()
        {
            //throw new NotImplementedException();
        }
        ~LightDb()
        {
            Db?.Dispose();
        }
    }
}
