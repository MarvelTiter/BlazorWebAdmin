using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace BlazorWeb.Shared.Components.MTree
{
    public partial class MTree<TNode>
    {
        [Parameter]
        [NotNull]
        public IEnumerable<TNode> DataSource { get; set; }
        [Parameter]
        public Func<TNode, string> KeyExpression { get; set; }
        [Parameter]
        public Func<TNode, IEnumerable<TNode>> ChildrenExpression { get; set; }
        [Parameter]
        public Func<TNode, string> TitleExpression { get; set; }
        [Parameter]
        public int Indent { get; set; } = 20;
        [Parameter]
        public string[] CheckedKeys { get; set; }
        [Parameter]
        public EventCallback<string[]> CheckedKeysChanged { get; set; }
        [Parameter]
        public bool IncludeIndeterminate { get; set; }
        [Parameter]
        public RenderFragment<TNode> TitleTemplate { get; set; }

        private List<MTreeNode<TNode>> nodes = new List<MTreeNode<TNode>>();

        public void AddNode(MTreeNode<TNode> node)
        {
            if (!nodes.Any(n => n.Key == node.Key))
            {
                nodes.Add(node);
            }
        }

        public void CheckChildren(MTreeNode<TNode> parent)
        {
            var children = nodes.Where(n => n.Parent != null && n.Parent.Key == parent.Key);
            foreach (var item in children)
            {
                item.Checked = parent.Checked;
            }
        }

        public bool CheckIndeterminate(MTreeNode<TNode> node)
        {
            var all = nodes.Count(n => n.Parent != null && n.Parent.Key == node.Key);
            var checkedCount = nodes.Count(n => n.Parent != null && n.Parent.Key == node.Key && n.Checked);
            var anyIndeter = nodes.Any(n => n.Parent != null && n.Parent.Key == node.Key && n.Indeterminate);
            return checkedCount > 0 && checkedCount < all || anyIndeter;
        }

        public bool CheckCheckedAll(MTreeNode<TNode> node)
        {
            return !nodes.Any(n => n.Parent != null && n.Parent.Key == node.Key && !n.Checked);
        }

        public async Task UpdateCheckedValues()
        {
            var checkedKeys = nodes.Where(n => n.Checked).Select(n => n.Key);
            var indeterminate = nodes.Where(n => n.Indeterminate && IncludeIndeterminate).Select(n => n.Key);
            var total = checkedKeys.Concat(indeterminate).ToArray();
            if (CheckedKeysChanged.HasDelegate)
            {
                await CheckedKeysChanged.InvokeAsync(total);
            }
            //Console.WriteLine("==============Checked===============");
            //foreach (var item in checkedKeys)
            //{
            //    Console.WriteLine(item);
            //}
            //Console.WriteLine("==============Indeterminate===============");
            //foreach (var item in indeterminate)
            //{
            //    Console.WriteLine(item);
            //}
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (CheckedKeys is null) return;
            nodes.ForEach(n => n.ResetState(CheckedKeys));
            //foreach (var item in CheckedKeys)
            //{
            //    var n = nodes.FirstOrDefault(n => n.Key == item);
            //    if (n is null)
            //        continue;
            //    if (n.Checked) continue;
            //    if (n.HasChild) continue;
            //    await n.OnNodeClickAsync();
            //}
        }

        public bool HasChecked(string key)
        {
            return CheckedKeys?.Contains(key) ?? false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

        }
    }
}
