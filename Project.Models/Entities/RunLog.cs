using MDbEntity.Attributes;
using Project.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Entities
{
    [TableName("RUN_LOG")]
    public class RunLog
    {
        [ColumnName("LOG_ID")]
        [TableHeader("日志ID")]
        [PrimaryKey]
        public int? LogId { get; set; }
        [ColumnName("USER_ID")]
        [TableHeader("操作用户")]
        public string UserId { get; set; }
        [ColumnName("ACTION_MODULE")]
        [TableHeader("操作模块")]
        public string ActionModule { get; set; }
        [ColumnName("ACTION_NAME")]
        [TableHeader("操作动作")]
        public string ActionName { get; set; }
        [ColumnName("ACTION_TIME")]
        [TableHeader("操作时间")]
        public DateTime ActionTime { get; init; } = DateTime.Now;
        [ColumnName("ACTION_RESULT")]
        [TableHeader("操作结果")]
        public string ActionResult { get; set; }
        [ColumnName("ACTION_MESSAGE")]
        [TableHeader("操作信息")]
        public string ActionMessage { get; set; }
    }
}
