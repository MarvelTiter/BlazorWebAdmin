using Project.Models.Entities;
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
        public async Task Log(RunLog log)
        {
            await repository.Table<RunLog>().InsertAsync(log);
        }
    }
}
