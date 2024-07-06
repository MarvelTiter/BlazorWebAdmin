
namespace Project.Constraints.UI.Tree
{
    public sealed class TreeData<TData>(TData data)
    {
        public TData Data { get; set; } = data;
        public List<TreeData<TData>>? Children { get; set; }
    }
    public sealed class TreeOptions<TData>(IEnumerable<TreeData<TData>> datas)
    {
        public bool Readonly { get; set; }
        public bool IncludeIndeterminate { get; set; }
        public List<TreeData<TData>> Datas { get; set; } = datas.ToList();
        public Func<TData, string>? KeyExpression { get; set; }
        public Func<TData, string>? TitleExpression { get; set; }
        public Func<TData,bool> DisableExpression { get; set; } = d => false;
        public Func<TData, string>? IconExpression { get; set; }
        public Func<string[], Task>? OnCheckedChanged {  get; set; }
        public Func<TreeData<TData>, IEnumerable<TreeData<TData>>?> ChildrenExpression => node => node.Children;
    }
}
