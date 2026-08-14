namespace BlazorTemplate.ClientCore;

public interface IDashboardContentProvider
{
    Type? GetComponentType();
    void SetComponentType(Type? type);
}
