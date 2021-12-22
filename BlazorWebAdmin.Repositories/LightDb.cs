using MDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebAdmin.Repositories
{
    public class LightDb
    {
        public static readonly string ConnectString = "";

        public DbContext Db => CreateDbContext();

        private DbContext CreateDbContext()
        {
            throw new NotImplementedException();
        }
        ~LightDb()
        {
            Db.Dispose();
        }
    }
}
