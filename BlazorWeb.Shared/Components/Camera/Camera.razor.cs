using AntDesign;
using BlazorWeb.Shared.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Project.Models;
using System;

namespace BlazorWeb.Shared.Components
{
    public partial class Camera
    {
        [Inject] public IJSRuntime Js { get; set; }
        [Inject] public ProtectedLocalStorage Storage { get; set; }
        [Inject] public MessageService MsgSrv { get; set; }
        [Parameter] public bool AutoPlay { get; set; }
        [Parameter] public bool EnableClip { get; set; }
        [Parameter] public int Width { get; set; }
        [Parameter] public int Height { get; set; }
        [Parameter] public RenderFragment<IEnumerable<DeviceInfo>> DeviceSelectorRender { get; set; }
        private IJSObjectReference cameraHelper;
        private ElementReference? videoDom;
        private ElementReference? clipDom;
        private ElementReference? canvasDom;
        private string selectedDeviceId = "";
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
        private SelectItem<string> dropdownDevices = new SelectItem<string>();
        public IEnumerable<DeviceInfo> Devices { get; set; } = Enumerable.Empty<DeviceInfo>();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Width < 200) Width = 200;
            if (Height < 100) Height = 100;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                cameraHelper = await Js.InvokeAsync<IJSObjectReference>("import", "./camera.js");
                await InitDevices();
                await cameraHelper.InvokeVoidAsync("init", videoDom, canvasDom);
                var result = await Storage.GetAsync<string>("previousSelectedDevice");
                if (result.Success)
                {
                    selectedDeviceId = result.Value ?? "";
                    StateHasChanged();
                    if (AutoPlay)
                        await Start();
                }
                if (EnableClip)
                    await cameraHelper.InvokeVoidAsync("initClipBox", clipDom, Width, Height);
            }
        }

        private async Task InitDevices()
        {
            Devices = await cameraHelper.InvokeAsync<IEnumerable<DeviceInfo>>("enumerateDevices");
            if (Devices.Any())
            {
                dropdownDevices.Clear();
                dropdownDevices.AddRange(Devices.Where(d => d.Kind == "videoinput").Select(d => new Options<string>(d.Label, d.DeviceId)));
                StateHasChanged();
            }
            else
            {
                _ = MsgSrv.Error("获取设备失败！请检查设备连接或者浏览器配置！", 0);
            }
        }
        bool playButtonStatus = false;
        public async Task Start()
        {
            playButtonStatus = await cameraHelper.InvokeAsync<bool>("loadUserMedia", selectedDeviceId, 1920, 1080);

            await Storage.SetAsync("previousSelectedDevice", selectedDeviceId);
            StateHasChanged();
        }

        public async Task Stop()
        {
            playButtonStatus = !await cameraHelper.InvokeAsync<bool>("closeUserMedia");
        }

        public async Task SwitchCamera(string deviceId)
        {
            await Stop();
            selectedDeviceId = deviceId;
            await Start();
        }

        public async Task Capture()
        {
            var base64 = await cameraHelper.InvokeAsync<string>("capture");
            return;
            var filename = $"CameraCapture_{DateTime.Now:yyyyMMddHHmmss}";
            using var fs = File.Open(Path.Combine(AppConst.TempFilePath, $"{filename}.jpeg"), FileMode.Create, FileAccess.Write);
            fs.Write(Convert.FromBase64String(base64));
            await fs.FlushAsync();
            _ = Js.DownloadFile(filename, "jpeg");
        }
    }
}
