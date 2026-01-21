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
    public T Copy<T>(T obj)
    {
        if (obj is null)
        {
            return default!;
        }
        if (obj is IAutoMap map)
        {
            return map.MapTo<T>();
        }
        return obj;
        //return GMapper.Map<T, T>(obj);
    }
}
