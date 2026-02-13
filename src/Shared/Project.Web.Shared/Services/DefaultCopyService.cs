using AutoGenMapperGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Web.Shared.Services;

[AutoInject]
internal class DefaultCopyService : ICopyable
{
    public T Copy<T>(T obj, bool usefallback = false)
    {
        return GMapper.Map<T, T>(obj);
    }
}
