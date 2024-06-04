using Project.Constraints.Options;
using System.Drawing;

namespace Project.Web.Shared.Components
{
    public partial class Camera
    {
        public struct Resolution
        {
            public static Resolution QVGA => new("QVGA(320×240)", 320, 240);
            public static Resolution VGA => new("VGA(640×380)", 640, 380);
            public static Resolution HD => new("HD(1280×720)", 1280, 720);
            public static Resolution FullHD => new("FullHD(1920×1080)", 1920, 1080);
            public static Resolution Television4K => new("Television4K(3840×2160)", 3840, 2160);
            public static Resolution Cinema4K => new("Cinema4K(4096×2160)", 4096, 2160);
            public static Resolution A4 => new("A4(1123×794)", 1123, 794);

            public int Width { get; set; }
            public int Height { get; set; }
            public string Name { get; set; }
            public Resolution(string name, int width, int height)
            {
                Name = name;
                Width = width;
                Height = height;
            }

            public static implicit operator Resolution((int width, int height) size) => new($"自定义({size.width}×{size.height})", size.width, size.height);

            public static implicit operator Resolution(CameraResolution c)
            {
                var name = string.IsNullOrEmpty(c.Name) ? $"自定义({c.Width}×{c.Height})" : c.Name;
                return new(name, c.Width, c.Height);
            }
        }
    }
}
