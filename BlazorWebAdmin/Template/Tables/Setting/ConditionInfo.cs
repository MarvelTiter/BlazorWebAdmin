using System.Linq.Expressions;

namespace BlazorWebAdmin.Template.Tables.Setting
{
    public record ConditionInfo(string Name, CompareType Type, object Value, Type ValueType, bool Legal)
    {
        public ExpressionType? LinkType { get; set; }
    }
}
