#if !NET9_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ExcludeFromInteractiveRoutingAttribute : Attribute
{
}
#endif
