using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Services;

public interface ICopyable
{
    /// <summary>
    /// 复制对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="usefallback">使用回退机制，强制返回</param>
    /// <returns></returns>
    T Copy<T>(T obj, bool usefallback = false);
}
