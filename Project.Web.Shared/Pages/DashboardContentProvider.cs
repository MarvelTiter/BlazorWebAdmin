using Project.Constraints;

namespace Project.Web.Shared.Pages;

public class DashboardContentProvider : IDashboardContentProvider
{
    Type? componentType;
    public Type? GetComponentType()
    {
        return componentType;
    }

    public void SetComponentType(Type? type)
    {
        componentType = type;
    }
}
