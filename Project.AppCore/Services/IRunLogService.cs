using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Services
{
    public interface IRunLogService
    {
        Task<IQueryCollectionResult<RunLog>> GetRunLogsAsync(GeneralReq<RunLog> runLog);
        Task Log(RunLog log);
    }
}
