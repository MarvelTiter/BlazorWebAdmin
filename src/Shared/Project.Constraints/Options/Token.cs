namespace Project.Constraints.Options;

public sealed class Token
{
    public int MaxFreeTime { get; set; } = 900;
    public bool NeedAuthentication { get; set; } = true;
    public TimeSpan Expire { get; set; } = TimeSpan.FromDays(15);
    public int LimitedFreeTime => MaxFreeTime < 300 ? 300 : MaxFreeTime;

}
