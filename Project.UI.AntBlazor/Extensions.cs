using AntDesign;
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

        public static CheckboxOption[] ConvertToCheckBoxOptions<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, string> label, Func<TItem, TValue> value)
        {
            return items.Select(item =>
             {
                 return new CheckboxOption() { Label = label.Invoke(item), Value = $"{value.Invoke(item)}" };
             }).ToArray();
        }

        public static RadioOption<TValue>[] ConvertToRadioOptions<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, string> label, Func<TItem, TValue> value)
        {
            return items.Select(item =>
            {
                return new RadioOption<TValue>() { Label = label.Invoke(item), Value = value.Invoke(item) };
            }).ToArray();
        }
    }
}
