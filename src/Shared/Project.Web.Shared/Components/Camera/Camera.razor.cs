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
        [Inject, NotNull] public IProtectedLocalStorage? Storage { get; set; }
        [Inject, NotNull] public IOptionsMonitor<AppSetting>? AppOptions { get; set; }
        [Inject, NotNull] public ICameraOptions? CameraOptions { get; set; }
        [Parameter] public bool AutoPlay { get; set; }
        [Parameter] public bool EnableClip { get; set; }
        [Parameter] public int Width { get; set; }
        [Parameter] public int Height { get; set; }
        [Parameter] public RenderFragment<ICameraObject>? DeviceSelectorRender { get; set; }
        [Parameter] public EventCallback<CaptureInfo> OnCapture { get; set; }
        [Parameter] public Resolution? CameraResolution { get; set; }
        [Parameter] public int? CameraWidth { get; set; }
        [Parameter] public int? CameraHeight { get; set; }
        [Parameter] public int RetryTimes { get; set; } = 3;
        [Parameter] public double Quality { get; set; } = 1;
        [Parameter] public int Rotate { get; set; }
        [Parameter] public bool PreviewRotate { get; set; }
        /// <summary>
        /// 默认值 image/png , 可选值 image/jpeg
        /// <para>
        /// https://developer.mozilla.org/zh-CN/docs/Web/API/HTMLCanvasElement/toDataURL
        /// </para>
        /// </summary>
        [Parameter] public string PhotoFormat { get; set; } = "image/png";
        int InternalRotate => Rotate % 4;
        int WrapHeight => PreviewRotate ? (Rotate % 2 == 0 ? Height : Width) : Height;
        int WrapWidth => PreviewRotate ? (Rotate % 2 == 0 && PreviewRotate ? Width : Height) : Width;
        int ComWidth => Math.Max(Width, Height);
        int RotateDeg => PreviewRotate ? InternalRotate * 90 : 0;
        private ElementReference? videoDom;
        private ElementReference? clipDom;
        private string? selectedDeviceId = null;
        public class DeviceInfo
        {
            [NotNull] public string? DeviceId { get; set; }
            [NotNull] public string? Label { get; set; }
            public string? GroupId { get; set; }
            /// <summary>
            /// videoinput | audioouput | audioinput
            /// </summary>
            public string? Kind { get; set; }
        }

        public struct CaptureInfo
        {
            public string Filename { get; set; }
            public string Content { get; set; }
        }
        private SelectItem<string> dropdownDevices = new SelectItem<string>();
        // [Resolution.QVGA, Resolution.VGA,Resolution.HD,Resolution.FullHD, Resolution.Television4K, Resolution.Cinema4K, Resolution.A4]
        public IEnumerable<DeviceInfo> Devices { get; set; } = Enumerable.Empty<DeviceInfo>();

       

        //protected override void OnInitialized()
        //{
        //    base.OnInitialized();
        //    TryAddCustomResolution(AppOptions.CurrentValue.CameraResolutions ?? []);
        //}

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
            //if (EnableClip)
            //    await ModuleInvokeVoidAsync("init", videoDom, Quality, clipDom, Width, Height);
            //else
            //    await ModuleInvokeVoidAsync("init", videoDom);

            await InvokeInit(new
            {
                video = videoDom,
                quality = Quality,
                clip = EnableClip ? clipDom : null,
                width = Width,
                height = Height,
                format = PhotoFormat
            });

            var result = await Storage.GetAsync<string>("previousSelectedDevice");
            if (result.Success)
            {
                selectedDeviceId = result.Value ?? "";
                StateHasChanged();
                if (AutoPlay)
                    await Start();
            }
        }

        public async Task ToggleClipBoxAsync()
        {
            if (EnableClip)
            {
                await InvokeVoidAsync("disableClipBox");
                EnableClip = false;
            }
            else
            {
                await InvokeVoidAsync("useClipBox", new
                {
                    clip = clipDom,
                    width = Width,
                    height = Height
                });
                EnableClip = true;
            }
        }

        private async Task<bool> InitDevices()
        {
            var result = await InvokeAsync<JsActionResult<IEnumerable<DeviceInfo>>>("enumerateDevices");
            if (result.IsSuccess)
            {
                Devices = result.Payload ?? [];
                dropdownDevices.Clear();
                dropdownDevices.AddRange(Devices.Where(d => d.Kind == "videoinput").Select(d => new Options<string>(d.Label, d.DeviceId)));
                StateHasChanged();
                return true;
            }
            else
            {
                UI.Error(result.Message ?? "an error occured while enumerate devices");
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
            if (result.IsSuccess && selectedDeviceId != null)
            {
                playButtonStatus = result.IsSuccess;
                await Storage.SetAsync("previousSelectedDevice", selectedDeviceId);
                StateHasChanged();
            }
            else
            {
                UI.Error(result.Message ?? "something wrong when try to open the camera");
            }
        }

        async Task<JsActionResult?> TryOpenCamera(int width, int height)
        {
            int hadTried = 0;
            JsActionResult? result = default;
            while (hadTried < RetryTimes)
            {
                result = await InvokeAsync<JsActionResult>("loadUserMedia", selectedDeviceId, width, height);
                if (result == null || !result.IsSuccess)
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
            var result = await InvokeAsync<JsActionResult>("closeUserMedia");
            if (result == null)
            {
                return;
            }
            if (result.IsSuccess)
            {
                playButtonStatus = !result.IsSuccess;
            }
            else
            {
                UI.Error(result.Message ?? "an error occured while closing the camera");
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
            var result = await InvokeAsync<JsActionResult<string>>("capture", InternalRotate);
            if (result.IsSuccess)
            {
                var filename = $"CameraCapture_{DateTime.Now:yyyyMMddHHmmss}";
                info = new CaptureInfo
                {
                    Filename = filename,
                    Content = result.Payload!,
                };
                if (OnCapture.HasDelegate)
                {
                    await OnCapture.InvokeAsync(info);
                }
            }
            else
            {
                UI.Error(result.Message ?? "an error occured while capture photo");
            }
            return info;
        }

        protected override async ValueTask OnDisposeAsync()
        {
            await Stop();
            await base.OnDisposeAsync();
        }
    }
}
