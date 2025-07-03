using System.Buffers;
using System.Diagnostics;

namespace Project.Constraints.Utils;
/// <summary>
/// 异步事件处理器
/// </summary>
public class AsyncHandlerManager
{
    private readonly List<Func<Task>> handlers = [];
    private CancellationTokenSource? cancellationTokenSource;

    public IDisposable RegisterHandler(Func<Task> handler)
    {
        handlers.Add(handler);
        return new HandlerRegistration(handler, this);
    }

    public async Task NotifyInvokeHandlers()
    {
        if (cancellationTokenSource is not null)
            await cancellationTokenSource.CancelAsync();
        cancellationTokenSource = null;

        var handlerCount = handlers.Count;

        if (handlerCount == 0)
        {
            return;
        }

        var cts = new CancellationTokenSource();

        cancellationTokenSource = cts;

        var cancellationToken = cts.Token;

        try
        {
            if (handlerCount == 1)
            {
                var handlerTask = InvokeHandlerAsync(handlers[0]);

                if (handlerTask.IsFaulted)
                {
                    await handlerTask;
                    return; // Unreachable because the previous line will throw.
                }

                if (!handlerTask.IsCompletedSuccessfully)
                {
                    await handlerTask.WaitAsync(cancellationToken);
                }
            }
            else
            {
                var handlersCopy = ArrayPool<Func<Task>>.Shared.Rent(handlerCount);
                try
                {
                    handlers.CopyTo(handlersCopy);
                    var tasks = new HashSet<Task>();
                    for (var i = 0; i < handlerCount; i++)
                    {
                        var handlerTask = InvokeHandlerAsync(handlersCopy[i]);
                        if (handlerTask.IsFaulted)
                        {
                            await handlerTask;
                            return; // Unreachable because the previous line will throw.
                        }

                        tasks.Add(handlerTask);
                    }

                    while (tasks.Count != 0)
                    {
                        var completedHandlerTask = await Task.WhenAny(tasks).WaitAsync(cancellationToken);

                        if (completedHandlerTask.IsFaulted)
                        {
                            await completedHandlerTask;
                            return; // Unreachable because the previous line will throw.
                        }

                        tasks.Remove(completedHandlerTask);
                        
                    }
                }
                finally
                {
                    ArrayPool<Func<Task>>.Shared.Return(handlersCopy);
                }
            }
        }
        catch (TaskCanceledException ex)
        {
            if (ex.CancellationToken == cancellationToken)
            {
                // This navigation was in progress when a successive navigation occurred.
                // We treat this as a canceled navigation.
                return;
            }

            throw;
        }
        finally
        {
            await cts.CancelAsync();
            cts.Dispose();

            if (cancellationTokenSource == cts)
            {
                cancellationTokenSource = null;
            }
        }
    }

    private static async Task InvokeHandlerAsync(Func<Task> handler)
    {
        try
        {
             await handler();
        }
        catch (OperationCanceledException)
        {
            // Ignore exceptions caused by cancellations.
        }
    }

    private void RemoveRegistration(Func<Task> handler)
    {
        handlers.Remove(handler);
        Debug.WriteLine("handler had remove");
    }

    private sealed class HandlerRegistration(Func<Task> handler, AsyncHandlerManager manager) : IDisposable
    {
        public void Dispose()
        {
            Debug.WriteLine("HandlerRegistration Disposing");
            manager.RemoveRegistration(handler);
        }
    }
}
/// <summary>
/// 异步事件处理器
/// </summary>
/// <typeparam name="TArg"></typeparam>
public class AsyncHandlerManager<TArg>
{
    private readonly List<Func<TArg, Task>> handlers = [];
    private CancellationTokenSource? cancellationTokenSource;

    public IDisposable RegisterHandler(Func<TArg, Task> handler)
    {
        handlers.Add(handler);
        return new HandlerRegistration(handler, this);
    }

    public async Task NotifyInvokeHandlers(TArg arg)
    {
        if (cancellationTokenSource is not null)
            await cancellationTokenSource.CancelAsync();
        cancellationTokenSource = null;

        var handlerCount = handlers.Count;

        if (handlerCount == 0)
        {
            return;
        }

        var cts = new CancellationTokenSource();

        cancellationTokenSource = cts;

        var cancellationToken = cts.Token;

        try
        {
            if (handlerCount == 1)
            {
                var handlerTask = InvokeHandlerAsync(handlers[0], arg);

                if (handlerTask.IsFaulted)
                {
                    await handlerTask;
                    return; // Unreachable because the previous line will throw.
                }

                if (!handlerTask.IsCompletedSuccessfully)
                {
                    await handlerTask.WaitAsync(cancellationToken);
                }
            }
            else
            {
                var handlersCopy = ArrayPool<Func<TArg, Task>>.Shared.Rent(handlerCount);
                try
                {
                    handlers.CopyTo(handlersCopy);
                    var tasks = new HashSet<Task>();
                    for (var i = 0; i < handlerCount; i++)
                    {
                        var handlerTask = InvokeHandlerAsync(handlersCopy[i], arg);
                        if (handlerTask.IsFaulted)
                        {
                            await handlerTask;
                            return; // Unreachable because the previous line will throw.
                        }

                        tasks.Add(handlerTask);
                    }

                    while (tasks.Count != 0)
                    {
                        var completedHandlerTask = await Task.WhenAny(tasks).WaitAsync(cancellationToken);

                        if (completedHandlerTask.IsFaulted)
                        {
                            await completedHandlerTask;
                            return; // Unreachable because the previous line will throw.
                        }

                        tasks.Remove(completedHandlerTask);
                        
                    }
                }
                finally
                {
                    ArrayPool<Func<TArg, Task>>.Shared.Return(handlersCopy);
                }
            }
        }
        catch (TaskCanceledException ex)
        {
            if (ex.CancellationToken == cancellationToken)
            {
                // This navigation was in progress when a successive navigation occurred.
                // We treat this as a canceled navigation.
                return;
            }

            throw;
        }
        finally
        {
            await cts.CancelAsync();
            cts.Dispose();

            if (cancellationTokenSource == cts)
            {
                cancellationTokenSource = null;
            }
        }
    }

    private static async Task InvokeHandlerAsync(Func<TArg, Task> handler, TArg arg)
    {
        try
        {
             await handler(arg);
        }
        catch (OperationCanceledException)
        {
            // Ignore exceptions caused by cancellations.
        }
    }

    private void RemoveRegistration(Func<TArg, Task> handler)
    {
        handlers.Remove(handler);
        Debug.WriteLine("handler had remove");
    }

    private sealed class HandlerRegistration(Func<TArg, Task> handler, AsyncHandlerManager<TArg> manager) : IDisposable
    {
        public void Dispose()
        {
            Debug.WriteLine("HandlerRegistration Disposing");
            manager.RemoveRegistration(handler);
        }
    }
}
/// <summary>
/// 异步事件处理器
/// </summary>
/// <typeparam name="TArg"></typeparam>
/// <typeparam name="TResult"></typeparam>
public class AsyncHandlerManager<TArg, TResult>
{
    private readonly List<Func<TArg, Task<TResult>>> handlers = [];
    private CancellationTokenSource? cancellationTokenSource;

    public IDisposable RegisterHandler(Func<TArg, Task<TResult>> handler)
    {
        handlers.Add(handler);
        return new HandlerRegistration(handler, this);
    }

    public async Task NotifyInvokeHandlers(TArg arg, Func<TResult?, TResult?, bool> stepResultHandler)
    {
        if (cancellationTokenSource is not null)
            await cancellationTokenSource.CancelAsync();
        cancellationTokenSource = null;

        var handlerCount = handlers.Count;

        if (handlerCount == 0)
        {
            return;
        }

        var cts = new CancellationTokenSource();

        cancellationTokenSource = cts;

        var cancellationToken = cts.Token;

        try
        {
            if (handlerCount == 1)
            {
                var handlerTask = InvokeHandlerAsync(handlers[0], arg);

                if (handlerTask.IsFaulted)
                {
                    await handlerTask;
                    return; // Unreachable because the previous line will throw.
                }

                if (!handlerTask.IsCompletedSuccessfully)
                {
                    await handlerTask.WaitAsync(cancellationToken);
                }

                stepResultHandler(default, handlerTask.Result);
                return;
            }
            else
            {
                var handlersCopy = ArrayPool<Func<TArg, Task<TResult>>>.Shared.Rent(handlerCount);
                TResult? result = default;
                try
                {
                    handlers.CopyTo(handlersCopy);
                    var tasks = new HashSet<Task<TResult?>>();
                    for (var i = 0; i < handlerCount; i++)
                    {
                        var handlerTask = InvokeHandlerAsync(handlersCopy[i], arg);
                        if (handlerTask.IsFaulted)
                        {
                            await handlerTask;
                            return; // Unreachable because the previous line will throw.
                        }

                        tasks.Add(handlerTask);
                    }

                    while (tasks.Count != 0)
                    {
                        var completedHandlerTask = await Task.WhenAny(tasks).WaitAsync(cancellationToken);

                        if (completedHandlerTask.IsFaulted)
                        {
                            await completedHandlerTask;
                            return; // Unreachable because the previous line will throw.
                        }

                        tasks.Remove(completedHandlerTask);
                        var shouldContinue = stepResultHandler(result, completedHandlerTask.Result);
                        if (!shouldContinue)
                        {
                            break;
                        }

                        result = completedHandlerTask.Result;
                    }
                }
                finally
                {
                    ArrayPool<Func<TArg, Task<TResult>>>.Shared.Return(handlersCopy);
                }
            }
        }
        catch (TaskCanceledException ex)
        {
            if (ex.CancellationToken == cancellationToken)
            {
                // This navigation was in progress when a successive navigation occurred.
                // We treat this as a canceled navigation.
                return;
            }

            throw;
        }
        finally
        {
            await cts.CancelAsync();
            cts.Dispose();

            if (cancellationTokenSource == cts)
            {
                cancellationTokenSource = null;
            }
        }
    }

    private static async Task<TResult?> InvokeHandlerAsync(Func<TArg, Task<TResult>> handler, TArg arg)
    {
        try
        {
            return await handler(arg);
        }
        catch (OperationCanceledException)
        {
            // Ignore exceptions caused by cancellations.
            return default;
        }
    }

    private void RemoveRegistration(Func<TArg, Task<TResult>> handler)
    {
        handlers.Remove(handler);
        Debug.WriteLine("handler had remove");
    }

    private sealed class HandlerRegistration(Func<TArg, Task<TResult>> handler, AsyncHandlerManager<TArg, TResult> manager) : IDisposable
    {
        public void Dispose()
        {
            Debug.WriteLine("HandlerRegistration Disposing");
            manager.RemoveRegistration(handler);
        }
    }
}