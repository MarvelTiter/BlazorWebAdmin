namespace BlazorTemplate.ClientCore.Options;

public record CameraResolution
{
    public string? Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
