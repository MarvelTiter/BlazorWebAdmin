using MT.Toolkit.DateTimeExtension;

namespace Project.Constraints.Models.Request;

public class GenericRequest<T> : IRequest<T>
{
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool FixTime { get; set; } = true;

    private DateTime _start = DateTime.Now;
    public DateTime StartTime { get => FixTime ? _start.DayStart() : _start; set => _start = value; }
    private DateTime _end = DateTime.Now;
    public DateTime EndTime { get => FixTime ? _end.DayEnd() : _end; set => _end = value; }
    //[JsonIgnore]
    //public Expression<Func<T, bool>>? Expression { get; set; }
    public int From => (PageIndex - 1) * PageSize;
    public int To => PageIndex * PageSize;

    public ConditionUnit Condition { get; set; } = new();
    public SolveType ExpressionSolveType { get; set; } = SolveType.TopOnly;
    public ConditionUnit? AdditionalCondition { get; set; }
    //[JsonIgnore]
    //Expression? IRequest.Expression
    //{
    //    get
    //    {
    //        return Expression;
    //    }
    //    set
    //    {
    //        Expression = value as Expression<Func<T, bool>>;
    //    }
    //}
}
public class GenericRequest : GenericRequest<object> { }
