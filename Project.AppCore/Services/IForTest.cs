using LogAopCodeGenerator;
using Project.AppCore.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Services
{
    //[Aspectable(AspectHandleType =typeof(LogAop))]
    public interface IForTest
    {
        Task<int> ReturnTaskValueAsync();
        Task<int> ReturnTaskValue();
        Task ReturnTaskAsync();
        Task ReturnTask();
        void ReturnVoid();
        int ReturnInt();
    }
}
