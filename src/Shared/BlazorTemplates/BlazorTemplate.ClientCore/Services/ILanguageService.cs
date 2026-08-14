using System.Globalization;

namespace BlazorTemplate.ClientCore.Services;

public interface ILanguageService
{
    event Action<CultureInfo>? LanguageChanged;
    CultureInfo CurrentCulture { get; }
    void SetLanguage(string name);
}