using AutoGenMapperGenerator;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorTemplate.ClientCore.Models.Dictionary;

/// <summary>
/// 字典项：某个字典类型下的具体条目，通过 <see cref="ParentId"/> 形成多级树。
/// <para>约定：<see cref="ParentId"/> 为 null 或空表示根节点（直接挂在某个字典类型下）。</para>
/// </summary>
[LangName("DictItem")]
[SupplyColumnDefinition]
public interface IDictItem
{
    [ColumnDefinition(Readonly = true)]
    [NotNull] string? ItemId { get; set; }

    [ColumnDefinition(Readonly = true)]
    [NotNull] string? TypeCode { get; set; }

    [ColumnDefinition]
    [NotNull] string? ItemName { get; set; }

    [ColumnDefinition]
    [NotNull] string? ItemValue { get; set; }

    [ColumnDefinition(Visible = false, Readonly = true)]
    string? ParentId { get; set; }

    IEnumerable<IDictItem>? Children { get; set; }
}

[LightTable(Name = "DICT_ITEM")]
[GenMapper]
public partial class TemplateDictItem : IDictItem
{
    [LightColumn(Name = "ITEM_ID", PrimaryKey = true)]
    [NotNull]
    public string? ItemId { get; set; }

    [LightColumn(Name = "TYPE_CODE")]
    [NotNull]
    public string? TypeCode { get; set; }

    [LightColumn(Name = "ITEM_NAME")]
    [NotNull]
    public string? ItemName { get; set; }

    [LightColumn(Name = "ITEM_VALUE")]
    [NotNull]
    public string? ItemValue { get; set; }

    [LightColumn(Name = "PARENT_ID")]
    public string? ParentId { get; set; }

    [NotMapped]
    public IEnumerable<TemplateDictItem>? Children { get; set; } = [];

    IEnumerable<IDictItem>? IDictItem.Children { get => Children; set => Children = value?.Cast<TemplateDictItem>(); }
}
