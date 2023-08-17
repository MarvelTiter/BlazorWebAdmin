namespace Project.AppCore
{
    public static class AppConst
    {
        public static readonly string TempFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tempfile");

        static AppConst()
        {
            if (!Directory.Exists(TempFilePath))
                Directory.CreateDirectory(TempFilePath);
        }
    }
}
