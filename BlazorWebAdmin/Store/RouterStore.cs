using Microsoft.AspNetCore.Components;
using Project.Common;
using Project.Models;
using Project.Services;

namespace BlazorWebAdmin.Store
{
    public class RouterMeta
    {
        public bool IsActive { get; set; }
        public string RouteLink { get; set; }
        public string IconName { get; set; }
        public string RouteName { get; set; }
        public bool HasChildren => Children != null && Children.Any();
        public string Redirect { get; set; } = "NoRedirect";
        public IEnumerable<RouterMeta> Children { get; set; }
    }

    public class TagRoute : RouterMeta
    {
        public bool Closable { get; set; } = true;
        public CacheItem? Content { get; set; }
        public string ItemClass => ClassHelper.Default
            .AddClass("main_content")
            .AddClass("active", () => IsActive).Class;

        public void SetActive(bool active)
        {
            IsActive = active;
            if (active && Content != null) Content.ActiveTime = DateTime.Now;
        }
    }

    public class RouterStore : StoreBase
    {
        public RouterStore()
        {
            Reset();
        }
        public List<TagRoute> TopLink { get; set; } = new List<TagRoute>();

        public List<RouterMeta> Routers { get; set; } = new List<RouterMeta>();

        public int Count { get; set; }

        public TagRoute Current => TopLink.FirstOrDefault(r => r.IsActive);

        public Task SetActive(string link)
        {
            if (link == "") link = "/";
            TopLink.ForEach(a => a.SetActive(a.IsActive = link == (a.RouteLink)));
            NotifyChanged();
            return Task.CompletedTask;
        }
        private RouterMeta FindRecursive(IEnumerable<RouterMeta> groups, string link)
        {
            if (!groups.Any()) return null;

            foreach (var item in groups)
            {
                if (!string.IsNullOrEmpty(item.RouteLink) && link == (item.RouteLink))
                {
                    return item;
                }
                if (item.HasChildren)
                {
                    var result = FindRecursive(item.Children, link);
                    if (result != null)
                        return result;
                    else
                        continue;
                }
            }
            return null;
        }
        public async Task TryAdd(string link)
        {
            //if (string.IsNullOrEmpty(link) || link == "/")
            //{
            //    TopLink.ForEach(a => a.IsActive = false);
            //    NotifyChanged();
            //    return;
            //}
            if (link == "") link = "/";
            var route = FindRecursive(Routers, link);
            if (!TopLink.Any(x => link == (x.RouteLink)) && route != null)
            {
                TopLink.Add(new TagRoute
                {
                    RouteLink = route.RouteLink,
                    RouteName = route.RouteName,
                });
            }
            await SetActive(link);
        }

        public async Task Remove(string link)
        {
            var index = TopLink.FindIndex(rs => rs.RouteLink == link);
            TopLink.RemoveAt(index);
            if (index < TopLink.Count)
            {
                // 跳到后一个标签
                await SetActive(TopLink[index].RouteLink);
            }
            else
            {
                await SetActive(TopLink[index - 1].RouteLink);
            }
            NotifyChanged();
        }

        public Task RemoveOther(string link)
        {
            var removes = TopLink.Where(r => r.RouteLink != link).ToArray();
            foreach (var item in removes)
            {
                if (item.Closable)
                    TopLink.Remove(item);
            }
            NotifyChanged();
            return Task.CompletedTask;
        }

        public Task Reset()
        {
            TopLink.Clear();
            TopLink.Add(new TagRoute
            {
                RouteLink = "/",
                RouteName = "主页",
                Closable = false,
            });
            return Task.CompletedTask;
        }

        public Task InitRoutersAsync(UserInfo userInfo)
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
                                RouteLink = "counter/index",
                            },
                            new RouterMeta
                            {
                                RouteName = "表格",
                                IconName = "setting",
                                RouteLink = "table",
                            },
                            new RouterMeta
                            {
                                RouteName = "用户",
                                IconName = "setting",
                                RouteLink = "user/index",
                            }
                        }
                },
                new RouterMeta()
                {
                    RouteName = "权限配置",
                    IconName = "setting",
                    RouteLink = "rolepermission"
                },
                new RouterMeta()
                {
                    RouteName = "权限设置",
                    IconName = "setting",
                    RouteLink = "permission"
                }
            };
            return Task.CompletedTask;
        }
    }
}
