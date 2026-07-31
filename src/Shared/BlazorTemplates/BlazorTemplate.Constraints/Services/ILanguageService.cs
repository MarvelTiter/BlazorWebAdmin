using System.Globalization;

namespace BlazorTemplate.Constraints.Services;

public interface ILanguageService
{
    event Action<CultureInfo>? LanguageChanged;
    CultureInfo CurrentCulture { get; }
    void SetLanguage(string name);
}