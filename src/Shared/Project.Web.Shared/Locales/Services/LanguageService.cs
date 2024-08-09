using AutoInjectGenerator;
using System.Globalization;

namespace Project.Web.Shared.Locales.Services
{
    [AutoInject]
    public class LanguageService : ILanguageService
    {
        private CultureInfo? culture;
        public CultureInfo CurrentCulture => culture ?? CultureInfo.CurrentCulture;

        public event Action<CultureInfo>? LanguageChanged;

        public void SetLanguage(string name)
        {
            var c = CultureInfo.GetCultureInfo(name);
            if (c.Equals(CultureInfo.CurrentCulture))
            {
                CultureInfo.CurrentCulture = c;
            }

            if (!(culture?.Equals(c) ?? false))
            {
                culture = c;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                LanguageChanged?.Invoke(culture);
            }
        }
    }
}
