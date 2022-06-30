using MDbContext;
using Microsoft.Data.Sqlite;
using Project.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore
{
    [IgnoreAutoInject]
    public class LightDb
    {
        public static readonly string ConnectString = $"DataSource={DbPath}";
        private static string DbPath => Path.GetFullPath("../Demo.db");
        DbContext _db;

        public DbContext Db
        {
            get
            {
                if (_db == null)
                {
                    _db = CreateDbContext();
                }
                return _db;
            }
        }

        public static IDbConnection CreateConnection()
        {
            return new SqliteConnection(ConnectString);
        }

        protected DbContext CreateDbContext()
        {
            DbContext.Init(DbBaseType.Sqlite);
            SqliteConnection conn = new SqliteConnection(ConnectString);
            return new DbContext(conn);
        }
        ~LightDb()
        {
            Db?.Dispose();
        }
    }
}
