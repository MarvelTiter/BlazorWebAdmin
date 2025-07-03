
namespace Project.Constraints.Models.Permissions;
[SupplyColumnDefinition]
public interface IRunLog
{
    [NotNull] int? LogId { get; set; }
    [ColumnDefinition]
    string? UserId { get; set; }
    [ColumnDefinition(UseTag = true)]
    string? ActionModule { get; set; }
    [ColumnDefinition]
    string? ActionName { get; set; }
    [ColumnDefinition]
    DateTime ActionTime { get; init; }
    [ColumnDefinition(UseTag = true)]
    [ColumnTag("成功", "Green")]
    [ColumnTag("失败", "Red")]
    string? ActionResult { get; set; }
    [ColumnDefinition]
    string? ActionMessage { get; set; }
}

public class MinimalLog
{
    public string? UserId { get; set; }
    public string? Module { get; set; }
    public string? Action { get; set; }
    public string? Result { get; set; }
    public string? Message { get; set; }
}
#if (ExcludeDefaultService)
#else
[LightTable(Name = "RUN_LOG")]
public class RunLog : IRunLog
{
    [LightColumn(Name = "LOG_ID", PrimaryKey = true)]
    [NotNull]
    public int? LogId { get; set; }

    [LightColumn(Name = "USER_ID")]
    public string? UserId { get; set; }

    [LightColumn(Name = "ACTION_MODULE")]
    public string? ActionModule { get; set; }

    [LightColumn(Name = "ACTION_NAME")]
    public string? ActionName { get; set; }

    [LightColumn(Name = "ACTION_TIME")]
    public DateTime ActionTime { get; init; } = DateTime.Now;

    [LightColumn(Name = "ACTION_RESULT")]
    public string? ActionResult { get; set; }

    [LightColumn(Name = "ACTION_MESSAGE")]
    public string? ActionMessage { get; set; }
}
#endif
