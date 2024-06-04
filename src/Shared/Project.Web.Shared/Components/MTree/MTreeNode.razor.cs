using Microsoft.AspNetCore.Components;
using Project.Constraints.Common;

namespace Project.Web.Shared.Components
{
    public partial class MTreeNode<TNode>
    {
        [Parameter]
        public TNode NodeValue { get; set; }
        [CascadingParameter]
        public MTree<TNode> Root { get; set; }
        [CascadingParameter]
        public MTreeNode<TNode> Parent { get; set; }
        [Parameter]
        public int Deepth { get; set; }

        public bool check;

        public bool Checked
        {
            get { return check; }
            set
            {
                check = value;
                Root.CheckChildren(this);
            }
        }

        public string Key => Root.KeyExpression(NodeValue);

        public bool Indeterminate { get; set; }

        public IEnumerable<TNode> Children => Root.ChildrenExpression(NodeValue) ?? Enumerable.Empty<TNode>();

        public bool HasChild => Children.Any();

        protected override void OnInitialized()
        {
            Root.AddNode(this);
            collapse = Deepth > 0 && Root.CollapseAll;
            ResetState(Root.CheckedKeys);
            base.OnInitialized();
        }

        public string CheckState
        {
            get
            {
                if (Indeterminate)
                {
                    return CheckBoxState.Indeterminate;
                }
                else
                {
                    return Checked ? CheckBoxState.Checked : CheckBoxState.Normal;
                }
            }
        }

        void AfterChildNodeStateChangedAsync()
        {
            Indeterminate = Root.CheckIndeterminate(this);
            check = Root.CheckCheckedAll(this);
            StateHasChanged();
            NotifyParent();
        }

        void NotifyParent()
        {
            if (Parent is null)
            {
                return;
            }
            Parent.AfterChildNodeStateChangedAsync();
        }

        public async Task OnNodeClickAsync()
        {
            Checked = !Checked;
            Indeterminate = false;
            NotifyParent();
            StateHasChanged();
            await Root.UpdateCheckedValues();
        }
        bool collapse;
        ClassHelper NarrowClass => ClassHelper.Default
            .AddClass("node-narrow")
            .AddClass("collapse", () => collapse)
            .AddClass("disable", () => !HasChild);
        ClassHelper ChildrenContentClass => ClassHelper.Default.AddClass("childrens").AddClass("collapse", () => collapse);
        Task ToggleExpand()
        {
            if (!HasChild) return Task.CompletedTask;
            collapse = !collapse;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 用于绑定数据更改时，初始化树状态
        /// </summary>
        /// <param name="keys"></param>
        public void ResetState(IEnumerable<string> keys)
        {
            check = false;
            Indeterminate = false;
            if (HasChild || keys == null) return;
            Checked = keys.Contains(Key);
            NotifyParent();
        }
    }
}
