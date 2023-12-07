using AntDesign;
using BlazorWeb.Shared.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using Project.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorWeb.Shared.Components
{
    public partial class Camera : JsComponentBase, ICameraObject
    {
        [Inject] public ProtectedLocalStorage Storage { get; set; }
        [Inject] public MessageService MsgSrv { get; set; }
        [Parameter] public bool AutoPlay { get; set; }
        [Parameter] public bool EnableClip { get; set; }
        [Parameter] public int Width { get; set; }
        [Parameter] public int Height { get; set; }
        [Parameter] public RenderFragment<ICameraObject> DeviceSelectorRender { get; set; }
        [Parameter] public EventCallback<CaptureInfo> OnCapture { get; set; }
        [Parameter] public bool AutoDownload { get; set; }
        [Parameter] public Resolution? CameraResolution { get; set; }

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

        public enum Resolution
        {
            [Display(Name = "QVGA(320×240)")]
            QVGA,
            [Display(Name = "VGA(640×380)")]
            VGA,
            [Display(Name = "HD(1280×720)")]
            HD,
            [Display(Name = "FullHD(1920×1080)")]
            FullHD,
            [Display(Name = "Television4K(3840×2160)")]
            Television4K,
            [Display(Name = "Cinema4K(4096×2160)")]
            Cinema4K,
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

        public Task Start()
        {
            return Start(Resolution.FullHD);
        }

        public async Task Start(Resolution? resolution = null)
        {
            if (!resolution.HasValue)
            {
                resolution = CameraResolution ?? Resolution.FullHD;
            }
            (int Width, int Height) res = resolution switch
            {
                Resolution.QVGA => (320, 240),
                Resolution.VGA => (640, 380),
                Resolution.HD => (1280, 720),
                Resolution.FullHD => (1920, 1080),
                Resolution.Television4K => (3840, 2160),
                Resolution.Cinema4K => (4096, 2160),
                _ => throw new ArgumentException()
            };
            var result = await ModuleInvokeAsync<JsActionResult>("loadUserMedia", selectedDeviceId, res.Width, res.Height);
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
            if (result == null)
            {
                return;
            }
            if (result.Success)
            {
                playButtonStatus = !result.Success;
            }
            else
            {
                _ = MsgSrv.Error(result.Message, 0);
            }
        }

        public async Task SwitchCamera(string deviceId, Resolution? resolution = null)
        {
            await Stop();
            selectedDeviceId = deviceId;
            await Start(resolution);
        }

        public async Task<CaptureInfo> Capture()
        {
            CaptureInfo info = new();
            var result = await ModuleInvokeAsync<JsActionResult<string>>("capture");
            if (result.Success)
            {
                var filename = $"CameraCapture_{DateTime.Now:yyyyMMddHHmmss}";
                info = new CaptureInfo
                {
                    Filename = filename,
                    Content = result.Payload,
                };
                if (OnCapture.HasDelegate)
                {
                    await OnCapture.InvokeAsync(info);
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
            return info;
        }

        protected override async ValueTask DisposeAsync(bool disposing)
        {
            await Stop();
            await base.DisposeAsync(disposing);
        }
    }
}
