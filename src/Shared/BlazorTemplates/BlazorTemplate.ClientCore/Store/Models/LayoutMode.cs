using System.ComponentModel.DataAnnotations;

namespace BlazorTemplate.ClientCore.Store.Models;

public enum LayoutMode
{
    [Display(Name = "经典")]
    Classic,
    [Display(Name = "卡片式")]
    Card,
    [Display(Name = "流线型")]
    Line,
}

public enum AppRunMode
{
    Server,
    WebAssembly,
    Auto
}