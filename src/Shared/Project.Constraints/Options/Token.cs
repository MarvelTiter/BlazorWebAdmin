namespace Project.Constraints.Options;

public sealed class Token
{
    public int MaxFreeTime { get; set; }
    public bool NeedAuthentication { get; set; }
    public int Expire { get; set; }
    public int LimitedFreeTime => MaxFreeTime < 300 ? 300 : MaxFreeTime;

}
