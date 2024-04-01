using Microsoft.Extensions.Localization;

namespace Project.Web.Shared.Basic
{
    public static class ServiceLocator
    {
        public static IServiceProvider Instance { get; set; }
        public static IStringLocalizer GetLocalizer() => Instance.GetService<IStringLocalizer>();
    }
}
