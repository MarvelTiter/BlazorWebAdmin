using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Services;

public interface ICopyable
{
    T Copy<T>(T obj);
}
