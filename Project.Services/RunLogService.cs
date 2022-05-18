using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;
using Project.Repositories.interfaces;
using Project.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
    public class RunLogService : IRunLogService
    {
        private readonly IRepository repository;

        public RunLogService(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IQueryCollectionResult<RunLog>> GetRunLogsAsync(GeneralReq<RunLog> req)
        {
            var total =await repository.Table<RunLog>().GetCountAsync(req.Expression);
            var list = await repository.Table<RunLog>().GetListAsync(req.Expression,req.PageIndex, req.PageSize);
            return QueryResult.Success<RunLog>().CollectionResult(list, total);
        }

        public async Task Log(RunLog log)
        {
            await repository.Table<RunLog>().InsertAsync(log);
        }
    }
}
