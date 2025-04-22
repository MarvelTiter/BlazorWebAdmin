using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Utils;

public static class AsyncEventInvoke
{
    public static async Task<bool> InvokeAsync<TP>(this Func<TP, Task<bool>>? @delegate, TP param)
    {
        if (@delegate is null) return true;
        foreach (var item in @delegate.GetInvocationList().Cast<Func<TP, Task<bool>>>())
        {
            var b = await item.Invoke(param);
            if (!b) return false;
        }
        return true;
    }

    public static async Task InvokeAsync<TP>(this Func<TP, Task>? @delegate, TP param)
    {
        if (@delegate is null) return ;
        foreach (var item in @delegate.GetInvocationList().Cast<Func<TP, Task>>())
        {
            await item.Invoke(param);
        }
    }

    public static async Task InvokeAsync(this Func<Task>? @delegate)
    {
        if (@delegate is null) return;
        foreach (var item in @delegate.GetInvocationList().Cast<Func<Task>>())
        {
            await item.Invoke();
        }
    }
}
