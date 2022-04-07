﻿using Project.Models;

namespace BlazorWebAdmin.Store
{
    public class RouterStore : StoreBase
    {
        public List<RouterMeta> TopLink { get; set; } = new List<RouterMeta>();

        public List<RouterMeta> Routers { get; set; } = new List<RouterMeta>();

        public int Count { get; set; }

        public Task SetActive(string link)
        {
            TopLink.ForEach(a => a.IsActive = a.RouteLink == link);
            NotifyChanged();
            return Task.CompletedTask;
        }

        public async Task TryAdd(string link, string name)
        {
            if (string.IsNullOrEmpty(link) || link == "/")
            {
                TopLink.ForEach(a => a.IsActive = false);
                NotifyChanged();
                return;
            }
            if (!TopLink.Any(x => x.RouteLink == link))
            {
                TopLink.Add(new RouterMeta
                {
                    RouteLink = link,
                    RouteName = name,
                });
            }
            await SetActive(link);
        }

        public Task Remove(string link)
        {
            var index = TopLink.FindIndex(rs => rs.RouteLink == link);
            TopLink.RemoveAt(index);
            NotifyChanged();
            return Task.CompletedTask;
        }

        public Task Reset()
        {
            TopLink.Clear();
            return Task.CompletedTask;
        }

        public Task InitRoutersAsync()
        {
            Routers = new List<RouterMeta>
            {
                new RouterMeta
                {
                    RouteName = "设置",
                    IconName = "setting",
                    Children = new List<RouterMeta>
                        {
                            new RouterMeta
                            {
                                RouteName = "计数器",
                                IconName = "setting",
                                RouteLink = "counter",
                            },
                            new RouterMeta
                            {
                                RouteName = "表格",
                                IconName = "setting",
                                RouteLink = "table",
                            }
                        }
                }
            };
            return Task.CompletedTask;
        }
    }
}
