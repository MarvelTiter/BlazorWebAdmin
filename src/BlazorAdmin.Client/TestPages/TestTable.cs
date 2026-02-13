using AutoInjectGenerator;
using BlazorAdmin.Client.TestPages.Components;
using LightORM;
using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Constraints.Models;
using Project.Constraints.Models.Request;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Table;
using Project.Web.Shared.Basic;
using System.Diagnostics.CodeAnalysis;

namespace BlazorAdmin.Client.TestPages;

public enum Vip
{
    None,
    Vip,
    SVip
}

[LightORM.LightTable(Name = "TEST_ENTITY")]
public class TestEntity
{
    [LightORM.LightColumn(Name = "INT_VALUE", PrimaryKey = true)]
    [ColumnDefinition]
    public int IntValue { get; set; }
    [LightORM.LightColumn(Name = "STRING_VALUE")]
    [ColumnDefinition]
    public string? StringValue { get; set; }
    [LightORM.LightColumn(Name = "BOOL_VALUE")]
    [ColumnDefinition]
    public bool BoolValue { get; set; }
    [LightORM.LightColumn(Name = "DATETIME_VALUE")]
    [ColumnDefinition]
    public DateTime? DateTimeValue { get; set; }
    [LightORM.LightColumn(Name = "ENUM_VALUE")]
    [ColumnDefinition]
    public Vip EnumValue { get; set; }
    [LightORM.LightColumn(Name = "CUSTOM_VALUE")]
    [ColumnDefinition]
    public string? CustomDisplay { get; set; }
}
[AutoInject]
public class TestService(IExpressionContext context) : CrudBase<TestEntity>(context)
{
    public async Task CreateTableAsync()
    {
        using var scope = context.CreateMainDbScoped();
        await scope.CreateTableAsync<TestEntity>();
    }

    public async Task<QueryResult> UpdatePropertyAsync(TestEntity entity, string porperty)
    {
        var e = await context.Update(entity).UpdateByName(porperty).ExecuteAsync();
        return e > 0;
    }
}
#if DEBUG
[Route("/testtable")]
[PageInfo(Title = "表格测试", Icon = "fa fa-question-circle-o", GroupId = "test")]
//[Layout(typeof(NotAuthorizedLayout))]
#endif
public class TestTable : ModelPage<TestEntity, GenericRequest<TestEntity>>
{
    [Inject, NotNull] TestService? Test { get; set; }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Options.EnableRowEdit = true;
        AdditionalHeaderButtons = UI.BuildButton(this).Text("创建Table").OnClick(BuildTable).Render();
        Options[nameof(TestEntity.CustomDisplay)].FormTemplate = ctx => b => b.Component<TestCustomDisplay>().SetComponent(c => c.Context, ctx).Build();
    }
    private Task BuildTable() => Test.CreateTableAsync();
    protected override Task<QueryCollectionResult<TestEntity>> OnQueryAsync(GenericRequest<TestEntity> query)
    {
        return Test.QueryListAsync(query);
    }

    protected override async Task<IQueryResult?> OnAddItemAsync()
    {
        var e = await this.ShowAddFormAsync("添加测试实体", "50%");
        return await Test.InsertAsync(e);
    }

    protected override async Task<IQueryResult?> OnCellUpdateAsync(TestEntity model, ColumnInfo col)
    {
        var r = await Test.UpdatePropertyAsync(model, col.PropertyOrFieldName);
        return r;
    }
}
