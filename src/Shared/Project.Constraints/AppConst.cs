using Microsoft.Extensions.Hosting;

namespace Project.Constraints;

public static class AppConst
{
    public static readonly string TempFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempfile");
    public static IHostEnvironment? Environment { get; set; }

    static AppConst()
    {
        if (!Directory.Exists(TempFilePath))
            Directory.CreateDirectory(TempFilePath);
    }
}
