using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Project.Web.Shared.Template.Tables.Setting
{
    public class ConditionInfo
    {
        public ConditionInfo(string Name, CompareType Type, object Value, Type ValueType, bool Legal)
        {
            this.Name = Name;
            this.Type = Type;
            this.Value = Value;
            this.ValueType = ValueType;
            this.Legal = Legal;
        }
        public ConditionInfo()
        {

        }
        [JsonIgnore]
        public Type ValueType { get; set; }
        public ExpressionType? LinkType { get; set; }
        public string Name { get; set; }
        public CompareType Type { get; set; }
        public object Value { get; set; }
        public bool Legal { get; set; }
    }
}
