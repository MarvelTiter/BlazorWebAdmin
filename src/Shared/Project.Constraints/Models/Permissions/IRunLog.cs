
namespace Project.Constraints.Models.Permissions;

public interface IRunLog
{
    int? LogId { get; set; }
    string UserId { get; set; }
    string ActionModule { get; set; }
    string ActionName { get; set; }
    DateTime ActionTime { get; init; }
    string ActionResult { get; set; }
    string ActionMessage { get; set; }
}

[LightTable(Name = "RUN_LOG")]
public class RunLog : IRunLog
{
    [LightColumn(Name = "LOG_ID", PrimaryKey = true)]
    public int? LogId { get; set; }

    [LightColumn(Name = "USER_ID")]
    [ColumnDefinition]
    public string UserId { get; set; }

    [LightColumn(Name = "ACTION_MODULE")]
    [ColumnDefinition(UseTag = true)]
    public string ActionModule { get; set; }

    [LightColumn(Name = "ACTION_NAME")]
    [ColumnDefinition]
    public string ActionName { get; set; }

    [LightColumn(Name = "ACTION_TIME")]
    [ColumnDefinition]
    public DateTime ActionTime { get; init; } = DateTime.Now;

    [LightColumn(Name = "ACTION_RESULT")]
    [ColumnDefinition(UseTag = true)]
    [ColumnTag("成功", "Green")]
    [ColumnTag("失败", "Red")]
    public string ActionResult { get; set; }

    [LightColumn(Name = "ACTION_MESSAGE")]
    [ColumnDefinition]
    public string ActionMessage { get; set; }
}
