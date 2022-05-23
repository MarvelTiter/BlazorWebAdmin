using LogAopCodeGenerator;
using Project.AppCore.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Services
{
    [Aspectable(AspectHandleType = typeof(LogAop))]
    public interface IForTest
    {
        [LogInfo(Action = "有Async,返回Task<T>", Module = "测试")]
        Task<int> ReturnTaskValueAsync();
        [LogInfo(Action = "无Async,返回Task<T>", Module = "测试")]
        Task<int> ReturnTaskValue();
        [LogInfo(Action = "有Async,返回Task", Module = "测试")]
        Task ReturnTaskAsync();
        [LogInfo(Action = "无Async,返回Task", Module = "测试")]
        Task ReturnTask();
        [LogInfo(Action = "同步无返回", Module = "测试")]
        void ReturnVoid();
        [LogInfo(Action = "异步有返回", Module = "测试")]
        int ReturnInt();
    }
}
