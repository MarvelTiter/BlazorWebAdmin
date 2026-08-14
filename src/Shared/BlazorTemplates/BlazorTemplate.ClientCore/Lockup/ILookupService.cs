using System.Collections.Frozen;

namespace BlazorTemplate.ClientCore.Lockup;

public readonly record struct LookupEntry(string Code, string DisplayName);

public interface ILookupService
{
    string GetDisplayString(string type, string? key);
    ICollection<LookupEntry> GetEntries(string type);
}

public interface ILookupProvider
{
    string TypeName { get; }
    IEnumerable<LookupEntry> LoadData();
    string GetDisplayString(string key);
    ICollection<LookupEntry> GetEntries();
}
