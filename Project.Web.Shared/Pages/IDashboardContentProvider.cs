using System.ComponentModel;

namespace BlazorWeb.Shared.Pages
{
    public interface IDashboardContentProvider
    {
        Type? GetComponentType();
        void SetComponentType(Type? type);
    }

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
}
