using Microsoft.AspNetCore.Components;
using Project.AppCore.PageHelper;
using Project.Common;

namespace Project.AppCore.Routers
{
    public class TagRoute : RouterMeta
    {
        public bool Closable { get; set; } = true;
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime ActiveTime { get; set; }
        public RenderFragment? Body { get; set; }
        public RenderFragment? Title { get; set; }
        public object PageRef { get; set; }

        private bool isActive;
        public bool IsActive { get => isActive; set => SetActive(value); }

        public void SetActive(bool active)
        {
            if (active) ActiveTime = DateTime.Now;
#if DEBUG
            Console.WriteLine($"{PageRef?.GetType().Name} [Update Active Status] {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
#endif
            if (isActive != active && PageRef is ITagPage page)
            {
                if (active)
                {
                    _ = page.OnShowAsync();
                }
                else
                {
                    _ = page.OnHiddenAsync();
                }
            }
            isActive = active;
        }
    }
}
