namespace BlazorTemplate.ClientCore.Components;

partial class VLink
{
    [Parameter, NotNull] public string? Href { get; set; }
    [Parameter] public string Rel { get; set; } = "stylesheet";
#if NET8_0
     [Inject, NotNull] IFileService? FileService { get; set; }
#endif
    private string GetHref()
    {
#if NET8_0
        return FileService.GetStaticFileWithVersion(Href);
#else
        return Assets[Href];
#endif
    }
}
