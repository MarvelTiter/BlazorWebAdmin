﻿using Microsoft.AspNetCore.Components;
using Project.Constraints.Models;

namespace Project.Web.Shared.Components;

public partial class Fetch<TData>
{
    [Parameter] public string? Url { get; set; }
    [Parameter] public string? Method { get; set; }
    [Parameter] public object? Body { get; set; }
    [Parameter] public bool FetchOnLoaded { get; set; }
    [Parameter] public EventCallback OnCompleted { get; set; }
    [Parameter] public EventCallback<TData> OnSuccessed { get; set; }
    [Parameter] public EventCallback<string> OnFailed { get; set; }
    protected override async ValueTask Init()
    {
        await InvokeVoidAsync("init");
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender || parametersChanged)
        {
            try
            {
                await Request();
            }
            catch
            {
                    
            }
            finally
            {
                parametersChanged = false;
            }
        }
    }
    bool parametersChanged;
    protected override void OnParametersSet()
    {
        parametersChanged = true;
        base.OnParametersSet();

    }

    public async Task<TData?> Request()
    {
        var result = await InvokeAsync<JsActionResult<TData>>("request", new
        {
            Url,
            Method,
            Body,
        });
        if (OnCompleted.HasDelegate)
        {
            await OnCompleted.InvokeAsync();
        }

        if (OnSuccessed.HasDelegate && result.IsSuccess)
        {
            await OnSuccessed.InvokeAsync(result.Payload);
            return result.Payload;
        }
        else if (OnFailed.HasDelegate)
        {
            await OnFailed.InvokeAsync(result.Message);
        }
        return default;
    }
}