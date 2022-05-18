using Project.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services.interfaces
{
    public interface IRunLogService
    {
        Task Log(RunLog log);
    }
}
