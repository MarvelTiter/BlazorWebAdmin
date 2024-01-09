using Project.Constraints.UI;

namespace Project.UI.AntBlazor
{
    public static class Extensions
    {
        public static void AddAntDesignUI(this IServiceCollection services)
        {
            services.AddAntDesign();
            services.AddScoped<IUIService, UIService>();
        }
    }
}
