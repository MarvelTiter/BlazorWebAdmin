namespace Project.Constraints.Options;

public sealed class Token
{
    public int MaxFreeTime { get; set; }
    public bool NeedAuthentication { get; set; }
#if DEBUG
    public int Expire => 0;
    public int LimitedFreeTime => 300;
#else
    public int Expire { get; set; }
public int LimitedFreeTime => MaxFreeTime < 300 ? 300 : MaxFreeTime;
#endif
}
