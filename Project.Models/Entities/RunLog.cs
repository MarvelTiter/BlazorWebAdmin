using MDbEntity.Attributes;
using Project.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Entities
{
    [Table(Name ="RUN_LOG")]
    public class RunLog
    {
        [Column(Name = "LOG_ID")]
        [ColumnDefinition("日志ID")]
        [PrimaryKey]
        public int? LogId { get; set; }
        [Column(Name = "USER_ID")]
        [ColumnDefinition("操作用户")]
        public string UserId { get; set; }
        [Column(Name = "ACTION_MODULE")]
        [ColumnDefinition("操作模块")]
        public string ActionModule { get; set; }
        [Column(Name = "ACTION_NAME")]
        [ColumnDefinition("操作动作")]
        public string ActionName { get; set; }
        [Column(Name = "ACTION_TIME")]
        [ColumnDefinition("操作时间")]
        public DateTime ActionTime { get; init; } = DateTime.Now;
        [Column(Name = "ACTION_RESULT")]
        [ColumnDefinition("操作结果")]
        public string ActionResult { get; set; }
        [Column(Name = "ACTION_MESSAGE")]
        [ColumnDefinition("操作信息")]
        public string ActionMessage { get; set; }
    }
}
