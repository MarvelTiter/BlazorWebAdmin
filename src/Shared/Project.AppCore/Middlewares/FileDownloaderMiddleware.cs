using AutoInjectGenerator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders.Physical;
using Project.AppCore.Auth;
using Project.Constraints;
using System.Text;

namespace Project.AppCore.Middlewares
{
    [AutoInject(ServiceType = typeof(FileDownloaderMiddleware), LifeTime = InjectLifeTime.Singleton)]
    public class FileDownloaderMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _ = context.Request.Form.TryGetValue("Token", out var t);
            _ = context.Request.Form.TryGetValue("Filename", out var filename);
            _ = context.Request.Form.TryGetValue("Path", out var path);

            var token = JwtTokenHelper.ReadToken(t!);
            if (string.IsNullOrEmpty(token.Uid))
            {
                context.Response.StatusCode = 403;
                return;
            }

            var file = Path.Combine(path!, filename!);
            if (System.IO.File.Exists(file))
            {
                var ext = Path.GetExtension(filename);
                var contentType = GetContentType(ext ?? "");
                var fileinfo = new FileInfo(file);
                var pf = new PhysicalFileInfo(fileinfo);
                context.Response.StatusCode = 200;
                context.Response.Headers.Append("Content-Type", contentType);
                context.Response.Headers.Append("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(pf.Name, Encoding.UTF8));
                await context.Response.SendFileAsync(pf, context.RequestAborted);
            }
            else
            {
                context.Response.Redirect("/download/notfound");
            }
            //await next(context);
        }
        /* text/html ： HTML格式
           text/plain ：纯文本格式
           text/xml ： XML格式
           image/gif ：gif图片格式
           image/jpeg ：jpg图片格式
           image/png：png图片格式
        ================================
            application/xhtml+xml ：XHTML格式
            application/xml： XML数据格式
            application/atom+xml ：Atom XML聚合格式
            application/json： JSON数据格式
            application/pdf：pdf格式
            application/msword ： Word文档格式
            application/octet-stream ： 二进制流数据（如常见的文件下载）
            .doc	application/msword
            .docx	application/vnd.openxmlformats-officedocument.wordprocessingml.document
            .xls	application/vnd.ms-excel
            .xlsx	application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
            .ppt	application/vnd.ms-powerpoint
            .pptx	application/vnd.openxmlformats-officedocument.presentationml.presentation
         */
        static string GetContentType(string ext)
        {
            return ext switch
            {
                ".gif" => "image/gif",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".xml" => "image/xml",
                ".txt" => "image/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }
    }
}
