using Project.Constraints.UI.Table;
using System.Reflection;

namespace Project.Constraints.UI.Form
{
    public sealed class FormBuilder
    {
        internal FormBuilder()
        {
        }

        public static FormBuilder Create()
        {
            return new FormBuilder();
        }

        public static FormBuilder Create<TData>()
        {
            var builder = new FormBuilder
            {
                columns = typeof(TData).GenerateColumns()
            };

            // foreach (var item in props)
            // {
            //     var form = item.GetCustomAttribute<FormAttribute>();
            //     if (form?.Hide == true) continue;
            //     builder.AddField($"{typeof(TData).Name}.{item.Name}", item);
            // }

            return builder;
        }

        List<ColumnInfo> columns = new List<ColumnInfo>();

        public FormBuilder AddField(string label, PropertyInfo property)
        {
            var col = new ColumnInfo(property);
            col.Label = label;
            columns.Add(col);
            return this;
        }

        public FormOptions<TData> Build<TData>(IUIService ui, TData data) where TData : class, new()
        {
            return new FormOptions<TData>(ui, data, columns);
        }
    }
}