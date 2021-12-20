using Microsoft.AspNetCore.Components;

namespace BlazorWebAdmin.StoreData
{
    public class RouterState
    {
        public bool IsActive { get; set; }
        public string? Link { get; set; }
    }
    public class RouterStore
    {
        public event Action RouteChangedEvent;
        public List<RouterState> AllLink { get; set; } = new List<RouterState>();
        public int Count { get; set; }
        public Task SetActive(string link)
        {
            //foreach (var item in AllLink)
            //{
            //             item.IsActive = item.Link == link;
            //}
            AllLink.ForEach(a => a.IsActive = a.Link == link);
            RouteChangedEvent?.Invoke();
            return Task.CompletedTask;
        }

        public async Task TryAdd(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                return;
            }
            if (!AllLink.Any(x => x.Link == link))
            {
                AllLink.Add(new RouterState
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
            RouteChangedEvent?.Invoke();
            return Task.CompletedTask;
        }
    }
}
