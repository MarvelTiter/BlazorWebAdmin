using AutoInjectGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorAdmin.Wpf;
[AutoInjectSelf]
internal class LoadingControl
{
    public bool HeaderLoaded { get; set; }
    public Action? Update { get; set; }
    public void Reset()
    {
        HeaderLoaded = false;
    }
}
