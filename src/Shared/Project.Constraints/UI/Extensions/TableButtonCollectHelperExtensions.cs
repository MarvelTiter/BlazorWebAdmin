using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Table;
using System.Collections.Concurrent;
using System.Reflection;

namespace Project.Constraints.UI.Extensions;

public static class TableButtonCollectHelperExtensions
{
    const BindingFlags BINDING_ATTRS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private static readonly ConcurrentDictionary<Type, TableButtonContext> buttonMethodsCache = new();
    private sealed class TableButtonContext(Type type)
    {
        public Type Type { get; set; } = type;
        public (MethodInfo Method, TableButtonAttribute Attr)[] Methods { get; set; } = [.. ScanMethods(type)];
        public ConcurrentDictionary<string, MethodInfo> ExpressionMethods { get; set; } = [];
    }
    private static IEnumerable<(MethodInfo Method, TableButtonAttribute Attr)> ScanMethods(Type t)
    {
        foreach (var item in t.GetMethods(BINDING_ATTRS))
        {
            var attr = item.GetCustomAttribute<TableButtonAttribute>();
            if (attr is null)
                continue;
            yield return (item, attr);
        }
    }
    public static List<TableButton<TModel>> CollectButtons<TModel>(this ComponentBase instance)
    {
        List<TableButton<TModel>> buttons = [];
        var type = instance.GetType();
        var context = buttonMethodsCache.GetOrAdd(type, static t =>
        {
            return new(t);
        });

        foreach (var (Method, Attr) in context.Methods)
        {
            var method = Method;
            var btnOptions = Attr;
            ArgumentNullException.ThrowIfNull(btnOptions.Label ?? btnOptions.LabelExpression);
            var btn = new TableButton<TModel>(btnOptions)
            {
                Callback = CreateMethodDelegate<Func<TModel, Task<IQueryResult?>>>(method, instance)
            };

            // 处理 LabelExpression
            if (btnOptions.LabelExpression is not null)
            {
                btn.LabelExpression = CreateExpressionDelegate<Func<TableButtonContext<TModel>, string>>(
                    btnOptions.LabelExpression, context, instance, BINDING_ATTRS);
            }

            // 处理 VisibleExpression
            if (btnOptions.VisibleExpression is not null)
            {
                btn.VisibleExpression = CreateExpressionDelegate<Func<TableButtonContext<TModel>, bool>>(
                    btnOptions.VisibleExpression, context, instance, BINDING_ATTRS);
            }

            buttons.Add(btn);
        }
        return buttons;
    }

    private static TDelegate CreateMethodDelegate<TDelegate>(MethodInfo method, object? instance = null)
        where TDelegate : Delegate
    {
        return method.IsStatic
            ? method.CreateDelegate<TDelegate>()
            : method.CreateDelegate<TDelegate>(instance);
    }

    private static TDelegate? CreateExpressionDelegate<TDelegate>(
        string methodName, TableButtonContext context, object instance, BindingFlags flags)
        where TDelegate : Delegate
    {
        var method = context.ExpressionMethods.GetOrAdd(methodName, name =>
        {
            return context.Type.GetMethod(name, flags) ?? throw new MissingMethodException($"未找到{methodName}方法");
        });
        return CreateMethodDelegate<TDelegate>(method, instance);
    }
}