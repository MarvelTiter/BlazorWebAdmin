using Microsoft.AspNetCore.Components;
using Project.Constraints.PageHelper;

namespace Project.Constraints.Store.Models
{
    public class TagRoute : RouterMeta
    {
        public bool Closable { get; set; } = true;
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime ActiveTime { get; set; }
        public RenderFragment? Body { get; set; }
        public RenderFragment? Title { get; set; }
        public object? PageRef { get; set; }

        private bool isActive;
        public bool IsActive { get => isActive; set => SetActive(value); }

        public async void SetActive(bool active)
        {
            if (active) ActiveTime = DateTime.Now;
            if (isActive != active && PageRef is IPageAction page)
            {
                if (active)
                {
                    await page.OnShowAsync();
                }
                else
                {
                    await page.OnHiddenAsync();
                }
            }
            isActive = active;
        }
    }
}
