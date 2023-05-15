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
        [PrimaryKey]
        public int? LogId { get; set; }
        [Column(Name = "USER_ID")]
        [ColumnDefinition]
        public string UserId { get; set; }
        [Column(Name = "ACTION_MODULE")]
        [ColumnDefinition]
        public string ActionModule { get; set; }
        [Column(Name = "ACTION_NAME")]
        [ColumnDefinition]
        public string ActionName { get; set; }
        [Column(Name = "ACTION_TIME")]
        [ColumnDefinition]
        public DateTime ActionTime { get; init; } = DateTime.Now;
        [Column(Name = "ACTION_RESULT")]
        [ColumnDefinition]
        public string ActionResult { get; set; }
        [Column(Name = "ACTION_MESSAGE")]
        [ColumnDefinition]
        public string ActionMessage { get; set; }
    }
}
