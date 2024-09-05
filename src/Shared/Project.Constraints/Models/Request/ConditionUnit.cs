using Project.Constraints.UI;
using System.ComponentModel.DataAnnotations;

namespace Project.Constraints.Models.Request
{
    public enum LinkType
    {
        None,
        AndAlso,
        OrElse
    }
    //public enum CompareType
    //{
    //    [Display(Name = "=")]
    //    Equal,
    //    [Display(Name = "!=")]
    //    NotEqual,
    //    [Display(Name = "包含")]
    //    Contains,
    //    [Display(Name = ">")]
    //    GreaterThan,
    //    [Display(Name = ">=")]
    //    GreaterThanOrEqual,
    //    [Display(Name = "<")]
    //    LessThan,
    //    [Display(Name = "<=")]
    //    LessThanOrEqual,
    //}

    public class ConditionUnit
    {
        public string Name { get; set; } = string.Empty;
        public CompareType CompareType { get; set; } = CompareType.Equal;
        public object? Value { get; set; }
        /// <summary>
        /// 同级节点的连接方式
        /// </summary>
        public LinkType LinkType { get; set; } = LinkType.AndAlso;
        /// <summary>
        /// 与子节点的连接方式
        /// </summary>
        public LinkType LinkChildren { get; set; } = LinkType.AndAlso;
        public IList<ConditionUnit> Children { get; set; } = [];
    }

    public class ConditionStorage
    {
        public string Name { get; set; } = string.Empty;
        public CompareType CompareType { get; set; } = CompareType.Equal;
    }
}
