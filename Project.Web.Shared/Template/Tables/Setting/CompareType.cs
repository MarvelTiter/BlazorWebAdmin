using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Project.Web.Shared.Template.Tables.Setting
{
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
}
