using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.UI.AntBlazor;

internal class Utils
{
    public static AntDesign.ButtonType GetButtonType(string? buttonType) => buttonType switch
    {
        "primary" => AntDesign.ButtonType.Primary,
        "text" => AntDesign.ButtonType.Text,
        _ => AntDesign.ButtonType.Default
    };

    public static AntDesign.ColumnFixPlacement? GetColumnFix(string? fix) => fix switch
    {
        "left" => AntDesign.ColumnFixPlacement.Left,
        "right" => AntDesign.ColumnFixPlacement.Right,
        _ => null
    };

    public static AntDesign.ColumnAlign GetColumnAlign(string? align) => align switch
    {
        "left" => AntDesign.ColumnAlign.Left,
        "center" => AntDesign.ColumnAlign.Center,
        "right" => AntDesign.ColumnAlign.Right,
        _ => AntDesign.ColumnAlign.Left
    };
}