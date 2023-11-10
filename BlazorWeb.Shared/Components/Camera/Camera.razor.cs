using AntDesign;
using BlazorWeb.Shared.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Project.Models;
using System;

namespace BlazorWeb.Shared.Components
{
    public partial class Camera : JsComponentBase
    {
        [Inject] public ProtectedLocalStorage Storage { get; set; }
        [Inject] public MessageService MsgSrv { get; set; }
        [Parameter] public bool AutoPlay { get; set; }
        [Parameter] public bool EnableClip { get; set; }
        [Parameter] public int Width { get; set; }
        [Parameter] public int Height { get; set; }
        [Parameter] public RenderFragment<IEnumerable<DeviceInfo>> DeviceSelectorRender { get; set; }
        [Parameter] public EventCallback<CaptureInfo> OnCapture { get; set; }
        [Parameter] public bool AutoDownload { get; set; }

        private ElementReference? videoDom;
        private ElementReference? clipDom;
        private ElementReference? canvasDom;
        private string? selectedDeviceId = null;
        public class DeviceInfo
        {
            public string DeviceId { get; set; }
            public string Label { get; set; }
            public string GroupId { get; set; }
            /// <summary>
            /// videoinput | audioouput | audioinput
            /// </summary>
            public string Kind { get; set; }
        }
        public struct CaptureInfo
        {
            public string Filename { get; set; }
            public string Content { get; set; }
        }
        private SelectItem<string> dropdownDevices = new SelectItem<string>();
        public IEnumerable<DeviceInfo> Devices { get; set; } = Enumerable.Empty<DeviceInfo>();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Width < 200) Width = 200;
            if (Height < 100) Height = 100;
        }

        //protected override async Task LoadJsAsync()
        //{
        //    var path = IsLibrary
        //        ? $"./_content/{ProjectName}/{RelativePath}/{ModuleName}/{ModuleName}.razor.js"
        //        : $"./{RelativePath}/{ModuleName}/{ModuleName}.razor.js";
        //    Module = await Js.InvokeAsync<IJSObjectReference>("import", path);
        //}

        protected override async ValueTask Init()
        {
            if (!await InitDevices())
                return;
            if (EnableClip)
                await ModuleInvokeVoidAsync("init", videoDom, canvasDom, clipDom, Width, Height);
            else
                await ModuleInvokeVoidAsync("init", videoDom, canvasDom);
            var result = await Storage.GetAsync<string>("previousSelectedDevice");
            if (result.Success)
            {
                selectedDeviceId = result.Value ?? "";
                StateHasChanged();
                if (AutoPlay)
                    await Start();
            }
        }

        private async Task<bool> InitDevices()
        {
            var result = await ModuleInvokeAsync<JsActionResult<IEnumerable<DeviceInfo>>>("enumerateDevices");
            if (result.Success)
            {
                Devices = result.Payload;
                dropdownDevices.Clear();
                dropdownDevices.AddRange(Devices.Where(d => d.Kind == "videoinput").Select(d => new Options<string>(d.Label, d.DeviceId)));
                StateHasChanged();
                return true;
            }
            else
            {
                _ = MsgSrv.Error(result.Message, 0);
                return false;
            }
        }
        bool playButtonStatus = false;
        public async Task Start()
        {
            var result = await ModuleInvokeAsync<JsActionResult>("loadUserMedia", selectedDeviceId, 1920, 1080);
            if (result.Success && selectedDeviceId != null)
            {
                playButtonStatus = result.Success;
                await Storage.SetAsync("previousSelectedDevice", selectedDeviceId);
                StateHasChanged();
            }
            else
            {
                _ = MsgSrv.Error(result.Message, 0);
            }
        }

        public async Task Stop()
        {
            var result = await ModuleInvokeAsync<JsActionResult>("closeUserMedia");
            if (result.Success)
            {
                playButtonStatus = !result.Success;
            }
            else
            {
                _ = MsgSrv.Error(result.Message, 0);
            }
        }

        public async Task SwitchCamera(string deviceId)
        {
            await Stop();
            selectedDeviceId = deviceId;
            await Start();
        }

        public async Task Capture()
        {
            var result = await ModuleInvokeAsync<JsActionResult<string>>("capture");
            if (result.Success)
            {
                var filename = $"CameraCapture_{DateTime.Now:yyyyMMddHHmmss}";
                if (OnCapture.HasDelegate)
                {
                    await OnCapture.InvokeAsync(new CaptureInfo
                    {
                        Filename = filename,
                        Content = result.Payload,
                    });
                }
                if (AutoDownload)
                {
                    using var fs = File.Open(Path.Combine(AppConst.TempFilePath, $"{filename}.jpeg"), FileMode.Create, FileAccess.Write);
                    fs.Write(Convert.FromBase64String(result.Payload));
                    await fs.FlushAsync();
                    _ = Js.DownloadFile(filename, "jpeg");
                }
            }
            else
            {
                _ = MsgSrv.Error(result.Message);
            }
        }
    }
}
