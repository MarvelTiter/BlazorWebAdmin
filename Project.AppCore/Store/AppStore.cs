using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Store
{
    public enum LayoutMode
    {
        Classic,
        Card,
        Line,
    }
    public class AppStore : StoreBase
    {
        public LayoutMode Mode { get; set; } = LayoutMode.Classic;
    }
}
