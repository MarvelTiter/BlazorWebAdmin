namespace Project.Constraints;

public interface IDashboardContentProvider
{
    Type? GetComponentType();
    void SetComponentType(Type? type);
}
