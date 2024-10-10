using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.UI.Extensions;

public static class UIServiceExtensions
{
    public static bool ShowResult(this IUIService ui, IQueryResult result)
    {
        if (result.IsSuccess)
        {
            ui.Success(result.Message ?? "操作成功");
        }
        else
        {
            ui.Error(result.Message ?? "出错");
        }
        return result.IsSuccess;
    }

    public static bool ShowError(this IUIService ui, IQueryResult result)
    {
        if (!result.IsSuccess)
        {
            ui.Error(result.Message ?? "出错");
        }
        return result.IsSuccess;
    }
}
