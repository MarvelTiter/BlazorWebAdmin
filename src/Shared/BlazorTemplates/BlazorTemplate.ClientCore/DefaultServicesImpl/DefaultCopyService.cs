using AutoGenMapperGenerator;

namespace BlazorTemplate.ClientCore.DefaultServicesImpl;

[AutoInject]
internal class DefaultCopyService : ICopyable
{
    public T Copy<T>(T obj, bool usefallback = false)
    {
        return GMapper.Map<T, T>(obj);
    }
}
