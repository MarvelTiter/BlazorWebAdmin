using Project.Common.Attributes;

namespace Project.Constraints;

[AutoInject]
public interface IDashboardContentProvider
{
    Type? GetComponentType();
    void SetComponentType(Type? type);
}
