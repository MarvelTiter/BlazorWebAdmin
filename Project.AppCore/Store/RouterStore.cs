using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Project.AppCore.Services;
using Project.Common;
using Project.Models.Permissions;

namespace Project.AppCore.Store
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
        public CacheItem Content { get; set; } = new CacheItem();
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
        private readonly IPermissionService permissionService;
        private readonly IStringLocalizer<RouterStore> localizer;

        public RouterStore(IPermissionService permissionService, IStringLocalizer<RouterStore> localizer)
        {
            this.permissionService = permissionService;
            this.localizer = localizer;
            Reset();
        }
        public List<TagRoute> TopLink { get; set; } = new List<TagRoute>();

        public List<RouterMeta> Routers { get; set; } = new List<RouterMeta>();

        public int Count { get; set; }

        public TagRoute Current => TopLink.FirstOrDefault(r => r.IsActive);

        public void SetActive(string link)
        {
            if (link == "") link = "/";
            TopLink.ForEach(a => a.SetActive(a.IsActive = link == a.RouteLink));
            NotifyChanged();
            //return Task.CompletedTask;
        }
        private RouterMeta FindRecursive(IEnumerable<RouterMeta> groups, string link)
        {
            if (!groups.Any()) return null;

            foreach (var item in groups)
            {
                if (!string.IsNullOrEmpty(item.RouteLink) && link == item.RouteLink)
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
        public void TryAdd(string link)
        {
            //if (string.IsNullOrEmpty(link) || link == "/")
            //{
            //    TopLink.ForEach(a => a.IsActive = false);
            //    NotifyChanged();
            //    return;
            //}
            if (link == "") link = "/";
            var route = FindRecursive(Routers, link);
            if (!TopLink.Any(x => link == x.RouteLink) && route != null)
            {
                TopLink.Add(new TagRoute
                {
                    RouteLink = route.RouteLink,
                    RouteName = route.RouteName,
                });
            }
            SetActive(link);
        }

        public void Remove(string link)
        {
            var index = TopLink.FindIndex(rs => rs.RouteLink == link);
            TopLink.RemoveAt(index);
            if (index < TopLink.Count)
            {
                // 跳到后一个标签
                SetActive(TopLink[index].RouteLink);
            }
            else
            {
                SetActive(TopLink[index - 1].RouteLink);
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
                RouteName = localizer["Home"],
                Closable = false,
            });
            return Task.CompletedTask;
        }
        //TODO 获取权限列表
        public async Task InitRoutersAsync(UserInfo? userInfo)
        {
            if (userInfo == null) return;
            var result = await permissionService.GetPowerListByUserIdAsync(userInfo.UserId);
            var powers = result.Payload;
            var roots = powers.Where(p => p.ParentId == "ROOT");
            Routers = new List<RouterMeta>
            {
                new RouterMeta
                {
                    RouteLink = "/",
                    IconName = "home",
                    RouteName = localizer["Home"],
                    Children = new List<RouterMeta>()
                }
            };
            foreach (var item in roots)
            {
                var n = new RouterMeta();
                n.RouteName = localizer[item.PowerId];
                n.RouteLink = item.Path;
                n.IconName = item.Icon;
                n.Children = FindChildren(powers, item);
                Routers.Add(n);
            }

            List<RouterMeta> FindChildren(IEnumerable<Power> all, Power parent)
            {
                var children = all.Where(p => p.ParentId == parent.PowerId);
                List<RouterMeta> childNodes = new();
                foreach (var child in children)
                {
                    var n1 = new RouterMeta();
                    n1.RouteName = localizer[child.PowerId];
                    n1.RouteLink = child.Path;
                    n1.IconName = child.Icon;
                    n1.Children = FindChildren(all, child);
                    childNodes.Add(n1);
                }
                return childNodes;
            }
        }
    }
}
