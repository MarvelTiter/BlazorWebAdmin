using AutoGenMapperGenerator;

namespace BlazorTemplate.UI.Shared.Services;

[AutoInject]
internal class DefaultCopyService : ICopyable
{
    public T Copy<T>(T obj, bool usefallback = false)
    {
        return GMapper.Map<T, T>(obj);
    }
}
