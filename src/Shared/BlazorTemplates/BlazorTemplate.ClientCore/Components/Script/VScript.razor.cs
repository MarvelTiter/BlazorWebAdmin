namespace BlazorTemplate.ClientCore.Components;

partial class VScript
{
    [Parameter, NotNull] public string? Src { get; set; }
#if NET8_0
     [Inject, NotNull] IFileService? FileService { get; set; }
#endif

    private string GetSrc()
    {
#if NET8_0
        return FileService.GetStaticFileWithVersion(Src);
#else
        return Assets[Src];
#endif
    }
}
