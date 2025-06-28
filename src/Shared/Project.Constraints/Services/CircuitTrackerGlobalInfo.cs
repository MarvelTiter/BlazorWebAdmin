using System.Collections.Concurrent;
using System.Net;

namespace Project.Constraints.Services;

public static class CircuitTrackerGlobalInfo
{
    public static ConcurrentDictionary<string, ClientInfo> CircuitClients { get; set; } = new();

    public static string ToIpString(this IPAddress? address)
    {
        address = address ?? IPAddress.IPv6Loopback;
        var ipstr = address.ToString();
        return ipstr.StartsWith("::ffff:") ? address.MapToIPv4().ToString() : ipstr;
    }
}