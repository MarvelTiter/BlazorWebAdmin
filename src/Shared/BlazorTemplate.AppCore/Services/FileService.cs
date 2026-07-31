using AutoInjectGenerator;
using BlazorTemplate.Constraints.Common;

namespace BlazorTemplate.AppCore.Services;

[AutoInject(Group = "SERVER", LifeTime = InjectLifeTime.Singleton)]
[AutoInject(Group = AutoInjectGroups.Hybrid, LifeTime = InjectLifeTime.Singleton)]
public class FileService : IFileService
{
    public string GetStaticFileWithVersion(string path)
    {
        var file = Path.Combine("wwwroot", path);
        var fi = new FileInfo(file);
        if (!fi.Exists)
        {
            return path;
        }
        return $"{path}?v={fi.LastWriteTimeUtc:yyMMddHHmmssfff}";
    }

    public Task<string> GetStaticFileWithVersionAsync(string path)
    {
        return Task.FromResult(GetStaticFileWithVersion(path));
    }
}