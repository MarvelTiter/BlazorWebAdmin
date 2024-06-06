using Project.Web.Shared.Utils;
using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Extensions;
using Project.Constraints.Store;
using Project.Constraints;
using Microsoft.Extensions.Options;
using Project.Constraints.Options;

namespace Project.Web.Shared.Components
{
    public partial class Camera : JsComponentBase, ICameraObject
    {
        [Inject] public IProtectedLocalStorage Storage { get; set; }
        [Inject] public IOptionsMonitor<AppSetting> AppOptions { get; set; }
        [Parameter] public bool AutoPlay { get; set; }
        [Parameter] public bool EnableClip { get; set; }
        [Parameter] public int Width { get; set; }
        [Parameter] public int Height { get; set; }
        [Parameter] public RenderFragment<ICameraObject> DeviceSelectorRender { get; set; }
        [Parameter] public EventCallback<CaptureInfo> OnCapture { get; set; }
        [Parameter] public bool AutoDownload { get; set; }
        [Parameter] public Resolution? CameraResolution { get; set; }
        [Parameter] public int? CameraWidth { get; set; }
        [Parameter] public int? CameraHeight { get; set; }
        [Parameter] public int RetryTimes { get; set; } = 3;
        [Parameter] public double Quality { get; set; } = 1;
        [Parameter] public int Rotate { get; set; }
        [Parameter] public bool PreviewRotate { get; set; }
        int InternalRotate => Rotate % 4;
        int WrapHeight => PreviewRotate ? (Rotate % 2 == 0 ? Height : Width) : Height;
        int WrapWidth => PreviewRotate ? (Rotate % 2 == 0 && PreviewRotate ? Width : Height) : Width;
        int ComWidth => Math.Max(Width, Height);
        int RotateDeg => PreviewRotate ? InternalRotate * 90 : 0;
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
        public SelectItem<Resolution> Resolutions { get; } = new SelectItem<Resolution>();
        public IEnumerable<DeviceInfo> Devices { get; set; } = Enumerable.Empty<DeviceInfo>();

        static void AddResolution(SelectItem<Resolution> items, Resolution item)
        {
            items.Add(item.Name, item);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            AddResolution(Resolutions, Resolution.QVGA);
            AddResolution(Resolutions, Resolution.VGA);
            AddResolution(Resolutions, Resolution.HD);
            AddResolution(Resolutions, Resolution.FullHD);
            AddResolution(Resolutions, Resolution.Television4K);
            AddResolution(Resolutions, Resolution.Cinema4K);
            AddResolution(Resolutions, Resolution.A4);

            foreach (var item in AppOptions.CurrentValue.CameraResolutions ?? [])
            {
                AddResolution(Resolutions, item);
            }
        }

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
                await ModuleInvokeVoidAsync("init", videoDom, canvasDom, Quality, clipDom, Width, Height);
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
                UI.Error(result.Message);
                return false;
            }
        }
        bool playButtonStatus = false;
        Resolution resolution = Resolution.FullHD;

        (int Width, int Height) GetSize(Resolution? resolution)
        {
            if (CameraWidth.HasValue && CameraHeight.HasValue)
            {
                return (CameraWidth.Value, CameraHeight.Value);
            }
            if (CameraResolution.HasValue)
            {
                resolution = CameraResolution.Value;
            }
            resolution ??= Resolution.FullHD;
            return (resolution.Value.Width, resolution.Value.Height);
        }

        public Task Start()
        {
            return Start(resolution);
        }

        public async Task Start(Resolution? resolution = null)
        {
            (int Width, int Height) res = GetSize(resolution);

            var result = await TryOpenCamera(res.Width, res.Height);
            if (result == null)
            {
                UI.Error("something wrong when try to open the camera");
                return;
            }
            if (result.Success && selectedDeviceId != null)
            {
                playButtonStatus = result.Success;
                await Storage.SetAsync("previousSelectedDevice", selectedDeviceId);
                StateHasChanged();
            }
            else
            {
                UI.Error(result.Message);
            }
        }

        async Task<JsActionResult?> TryOpenCamera(int width, int height)
        {
            int hadTried = 0;
            JsActionResult? result = default;
            while (hadTried < RetryTimes)
            {
                result = await ModuleInvokeAsync<JsActionResult>("loadUserMedia", selectedDeviceId, width, height);
                if (result == null || !result.Success)
                {
                    await Task.Delay(100);
                    hadTried++;
                }
                else
                {
                    break;
                }
            }
            return result;
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
                UI.Error(result.Message);
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
            var result = await ModuleInvokeAsync<JsActionResult<string>>("capture", InternalRotate);
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
                UI.Error(result.Message);
            }
            return info;
        }

        protected override async ValueTask DisposeAsync(bool disposing)
        {
            try
            {
                await Stop();
            }
            finally
            {
                await base.DisposeAsync(disposing);
            }

        }
    }
}
