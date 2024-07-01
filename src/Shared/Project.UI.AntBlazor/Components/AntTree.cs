using AntDesign;
using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Tree;
using System.Reflection;

namespace Project.UI.AntBlazor.Components
{
    public class AntTree<TData> : Tree<TreeData<TData>>
    {
        [Parameter] public TreeOptions<TData> Options { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Checkable = true;
            DisabledExpression = node => Options.DisableExpression(node.DataItem.Data) || Options.Readonly;
            KeyExpression = node => Options.KeyExpression(node.DataItem.Data);
            TitleExpression = node => Options.TitleExpression(node.DataItem.Data);
            if (Options.IconExpression != null)
            {
                IconExpression = node => Options.IconExpression(node.DataItem.Data);
            }
            ChildrenExpression = node => Options.ChildrenExpression(node.DataItem);
            IsLeafExpression = node => !(Options.ChildrenExpression(node.DataItem)?.Any() ?? false);
            DataSource = Options.Datas;
        }
    }
}
