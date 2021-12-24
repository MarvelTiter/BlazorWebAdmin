using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Common
{
    public static class ValueBoxes
    {
        public static object TrueBox = true;

        public static object FalseBox = false;

        public static object BooleanBox(bool value) => value ? TrueBox : FalseBox;
    }

}
