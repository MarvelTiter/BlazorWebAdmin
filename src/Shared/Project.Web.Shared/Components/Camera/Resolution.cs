using Project.Constraints.Options;
using System.Drawing;

namespace Project.Web.Shared.Components;

public partial class Camera
{
    public struct Resolution : IEquatable<Resolution>
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
        [NotNull] public string? Name { get; set; }
        public Resolution() { }
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

        public readonly bool Equals(Resolution other)
        {
            return other.Name == Name && other.Width == Width && other.Height == Height;
        }

        public override bool Equals(object? obj)
        {
            return obj is Resolution other && Equals(other);
        }

        public override readonly int GetHashCode() => Name.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();

        public static bool operator ==(Resolution left, Resolution right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Resolution left, Resolution right)
        {
            return !(left == right);
        }
    }

    //public static SelectItem<Resolution> Resolutions => resolutions.Value;
    //private static void AddResolution(SelectItem<Resolution> items, Resolution item)
    //{
    //    items.Add(item.Name, item);
    //}
    //private static readonly Lazy<SelectItem<Resolution>> resolutions = new(() =>
    //{
    //    var resolution = new SelectItem<Resolution>();
    //    AddResolution(resolution, Resolution.QVGA);
    //    AddResolution(resolution, Resolution.VGA);
    //    AddResolution(resolution, Resolution.HD);
    //    AddResolution(resolution, Resolution.FullHD);
    //    AddResolution(resolution, Resolution.Television4K);
    //    AddResolution(resolution, Resolution.Cinema4K);
    //    AddResolution(resolution, Resolution.A4);
    //    return resolution;
    //});
    //private static SemaphoreSlim slim = new(1, 1);
    //private static bool hadAddCustomResolution = false;
    //public static void TryAddCustomResolution(IEnumerable<CameraResolution> customs)
    //{
    //    slim.Wait();
    //    if (!hadAddCustomResolution)
    //    {
    //        foreach (var item in customs)
    //        {
    //            //var name = item.Name ?? $"自定义({item.Width}×{item.Height})";
    //            //var exits = resolutions.Value.Contains(name, v => v.Height == item.Height && v.Width == item.Width);
    //            //if (exits)
    //            //    continue;
    //            //else
    //            //{
    //            //    AddResolution(resolutions.Value, item);
    //            //}
    //            AddResolution(resolutions.Value, item);
    //        }
    //        hadAddCustomResolution = true;
    //    }
    //    slim.Release();
    //}
}
