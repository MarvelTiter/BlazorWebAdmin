using AutoInjectGenerator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Constraints.Services;

public interface ILanguageService
{
    event Action<CultureInfo>? LanguageChanged;
    CultureInfo CurrentCulture { get; }
    void SetLanguage(string name);
}