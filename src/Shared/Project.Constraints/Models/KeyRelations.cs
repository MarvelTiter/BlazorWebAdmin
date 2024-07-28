using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Models
{
    public class KeyRelations<TM, TS>
    {
        public KeyRelations(TM? main, IEnumerable<TS>? values)
        {
            Main = main;
            Slaves = [.. values];
        }
        public KeyRelations()
        {
            
        }
        public TM? Main { get; set; }
        public TS[]? Slaves { get; set; }

        public static implicit operator KeyRelations<TM, TS>((TM?, IEnumerable<TS>?) v) => new(v.Item1, v.Item2);
    }
}
