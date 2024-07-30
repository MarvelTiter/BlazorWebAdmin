using System.ComponentModel.DataAnnotations;

namespace Project.Constraints.Models.Request
{
    public enum LinkType
    {
        AndAlso,
        OrElse
    }
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
    public class ConditionUnit
    {
        public ConditionUnit(string name, CompareType compareType, object? value)
        {
            Name = name;
            CompareType = compareType;
            Value = value;
        }
        public ConditionUnit()
        {

        }
        //public ConditionUnit()
        //{
        //    IsRoot = 
        //}
        //public bool IsRoot { get; set; }
        public string? Name { get; }
        public CompareType? CompareType { get; }
        public object? Value { get; }
        public LinkType? LinkType { get; set; }
        public IList<ConditionUnit>? Children { get; set; }
    }
    public class ConditionAggregation
    {
        public IList<ConditionUnit> Conditions { get; set; } = [];
    }
}
