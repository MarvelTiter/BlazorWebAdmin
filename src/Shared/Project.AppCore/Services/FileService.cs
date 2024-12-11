using AutoInjectGenerator;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.AppCore.Services
{
    [AutoInject(Group = "SERVER", LifeTime = InjectLifeTime.Singleton)]
    public class FileService : IFileService
    {
        public string GetStaticFileWithVersion(string path)
        {
            //if (environment.IsDevelopment())
            //{
            //    return path;
            //}
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
}
