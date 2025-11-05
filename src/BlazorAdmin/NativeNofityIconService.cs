using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Runtime.InteropServices;

namespace BlazorAdmin;

#if(UseNotifyIcon || DEBUG)
public class NativeNofityIconService
{
    //System.Windows.Forms.NotifyIcon? notifyIcon;
}
#endif
