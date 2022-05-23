using Project.AppCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Services
{
    public class CodeGenTest : IForTest
    {
        public int ReturnInt()
        {
            return 0;
        }

        public Task ReturnTask()
        {
            return Task.CompletedTask;
        }

        public async Task ReturnTaskAsync()
        {
            await Task.CompletedTask;
        }

        public Task<int> ReturnTaskValue()
        {
            return Task.FromResult<int>(1);
        }

        public async Task<int> ReturnTaskValueAsync()
        {
            await Task.Delay(100);
            return 1;
        }

        public void ReturnVoid()
        {

        }
    }
}
