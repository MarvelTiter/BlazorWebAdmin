using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Project.Constraints.UI;
public enum CompareType
{
    [Display(Name = "=")]
    Equal,
    [Display(Name = "!=")]
    NotEqual,
    [Display(Name = "包含")]
    Contains,
    [Display(Name = ">")]
    GreaterThan,
    [Display(Name = ">=")]
    GreaterThanOrEqual,
    [Display(Name = "<")]
    LessThan,
    [Display(Name = "<=")]
    LessThanOrEqual,
}
public class ConditionInfo
{
    public ConditionInfo(string Name, CompareType Type, object? Value, Type ValueType, bool Legal)
    {
        this.Name = Name;
        this.Type = Type;
        this.Value = Value;
        this.ValueType = ValueType;
        this.Legal = Legal;
    }
    public ConditionInfo()
    {
        //ValueType
        //System.Type
    }
    [JsonIgnore]
    [NotNull]
    public Type? ValueType { get; set; }
    public ExpressionType? LinkType { get; set; }
    [NotNull]
    public string? Name { get; set; }
    public CompareType Type { get; set; }
    public object? Value { get; set; }
    public bool Legal { get; set; }
}
