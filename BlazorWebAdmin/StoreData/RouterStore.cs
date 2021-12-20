using Microsoft.AspNetCore.Components;

namespace BlazorWebAdmin.StoreData
{
    public class StoreBase
    {
        public event Action DataChangedEvent;
        protected void NotifyChanged()
        {
            DataChangedEvent?.Invoke();
        }
    }
    public class RouterInfo
    {
        public bool IsActive { get; set; }
        public string Link { get; set; }
        public string IconName { get; set; }
        public string PageName { get; set; }
    }
    public class RouterStore : StoreBase
    {
        public List<RouterInfo> AllLink { get; set; } = new List<RouterInfo>();
      
        public int Count { get; set; }
        public Task SetActive(string link)
        {
            AllLink.ForEach(a => a.IsActive = a.Link == link);
            NotifyChanged();
            return Task.CompletedTask;
        }

        public async Task TryAdd(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                AllLink.ForEach(a => a.IsActive = false);
                NotifyChanged();
                return;
            }
            if (!AllLink.Any(x => x.Link == link))
            {
                AllLink.Add(new RouterInfo
                {
                    Link = link,
                });
            }
            await SetActive(link);
        }

        public Task Remove(string link)
        {
            var index = AllLink.FindIndex(rs => rs.Link == link);
            AllLink.RemoveAt(index);
            NotifyChanged();
            return Task.CompletedTask;
        }

        public Task Reset()
        {
            AllLink.Clear();
            return Task.CompletedTask;
        }
    }
}
