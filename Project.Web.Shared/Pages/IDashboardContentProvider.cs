using System.ComponentModel;

namespace Project.Web.Shared.Pages
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
