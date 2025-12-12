using AutoInjectGenerator;
using Project.Constraints.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Services;

[AutoInject(Group = AutoInjectGroups.SERVER, LifeTime = InjectLifeTime.Singleton)]
public class ConsoleOutputService : IConsoleOutputService
{
    class BlazorConsoleWriter : TextWriter
    {
        private readonly TextWriter originalWriter;
        public BlazorConsoleWriter()
        {
            originalWriter = Console.Out;
            Console.SetOut(this);
        }
        public override void Write(string? value)
        {
            if (value is null)
            {
                return;
            }

        }
        public override Encoding Encoding => Console.OutputEncoding;
    }
    public ConsoleOutputService()
    {

    }
    public Task<string> GetConsoleOutputAsync()
    {
        throw new NotImplementedException();
    }
}
