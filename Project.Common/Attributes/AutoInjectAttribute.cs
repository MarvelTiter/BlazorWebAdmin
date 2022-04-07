using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common.Attributes
{
    public enum DIType
    {
        Transient,
        Scope,
        Singleton
    }
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface)]
    public class AutoInjectAttribute : Attribute
    {
        public DIType TimeLife { get; set; }
        public AutoInjectAttribute(DIType type = DIType.Scope)
        {
            TimeLife = type;
        }
    }

    public class IgnoreAutoInjectAttribute : Attribute { }
}
