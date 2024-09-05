namespace Project.Constraints.Options;

public record CameraResolution
{
    public string? Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
