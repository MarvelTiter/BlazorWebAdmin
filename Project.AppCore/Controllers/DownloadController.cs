using Microsoft.AspNetCore.Mvc;
using Project.AppCore.Auth;
using Project.Constraints.Services;

namespace Project.AppCore.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/{controller}")]
    public class DownloadController : ControllerBase
    {
        public class DownloadRequest
        {
            public string Filename { get; set; }
            public string Token { get; set; }
        }
        [HttpPost]
        public IActionResult DownloadTempFile([FromForm] DownloadRequest request)
        {
            var token = JwtTokenHelper.ReadToken(request.Token);
            if (string.IsNullOrEmpty(token.Uid))
            {
                return Forbid();
            }
            var file = Path.Combine(AppConst.TempFilePath, request.Filename);
            if (System.IO.File.Exists(file))
            {
                var ext = Path.GetExtension(request.Filename);
                return PhysicalFile(file, GetContentType(ext ?? ""), request.Filename);
            }
            return NotFound();
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
        //static readonly ConcurrentDictionary<string, string> ContentTypeMapDic = new()
        //{
        //    [""] = "",
        //};
        string GetContentType(string ext)
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
