namespace BlazorTemplate.ClientCore.Components;

/// <summary>
/// <para><see cref="RowTemplate"/>设置行模板</para>
/// <para><see cref="Empty"/>设置空数据模板</para>
/// <para><see cref="Items"/>设置数据源</para>
/// </summary>
/// <typeparam name="TData"></typeparam>
public partial class SimpleTable<TData>
{
    [Parameter] public string TableClass { get; set; } = string.Empty;
    [Parameter] public string HeaderClass { get; set; } = string.Empty;
    [Parameter] public string RowClass { get; set; } = string.Empty;
    [Parameter] public string[] Headers { get; set; } = [];
    [Parameter] public IEnumerable<TData>? Items { get; set; }
    [Parameter] public RenderFragment? Empty { get; set; }
    [Parameter] public RenderFragment<TData>? RowTemplate { get; set; }
}
