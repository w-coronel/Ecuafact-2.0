using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace EcuafactExpress.Web.Models
{
    public class NavMenuItem
    {
        public Guid UID { get; private set; } = Guid.NewGuid();
        public string Title { get; set; } = "";
        public string Url { get; set; } = null;
        public string Icon { get; set; } = null;
        public UserRoleEnum Role { get; set; } = UserRoleEnum.None;
        public NavTypeEnum BadgeType { get; set; } =  NavTypeEnum.Info;
        public long BadgeCount
        {
            get
            {
                if (__getBadgeCount != null)
                    return __getBadgeCount();
                else
                    return 0;
            }
        }

        private Func<long> __getBadgeCount { get; set; }
        public NavMenuItem Parent { get; private set; }

        public NavMenuItemCollection Items { get; set; }
         
        public NavMenuItem() { this.Items = new NavMenuItemCollection(this); }
        public NavMenuItem(NavMenuItem parent, Func<long> getBadgeCount = null) : this()
        {
            Parent = parent;
            __getBadgeCount = getBadgeCount;
        }
         
    }

    public class NavMenuItemCollection : List<NavMenuItem>, ICollection<NavMenuItem>
    {
        public NavMenuItem Parent { get; private set; }
        
        public NavMenuItemCollection(NavMenuItem parent)
        {
            Parent = parent;
        }

        private NavMenuItemCollection() { }

        public static NavMenuItemCollection Default { get; private set; }

        static NavMenuItemCollection()
        {
            Default = new NavMenuItemCollection();
        }

        public NavMenuItem Add(string title, string url = null, string icon = null, UserRoleEnum role = UserRoleEnum.None,
            Func<long> badgeCount = null, NavTypeEnum badgeType =  NavTypeEnum.Info)
        {
            var menu = new NavMenuItem(Parent, badgeCount) { Title = title, Url = url, Icon = icon, Role = role, BadgeType = badgeType };

            this.Add(menu);

            return menu;
        }

        public void AddDivider()
        {
            this.Add(new NavMenuItem(Parent)); 
        }
    }

    public enum NavTypeEnum
    {
        Primary, Secondary, Success, Danger, Warning, Info, Light, Dark
    }

    public enum UserRoleEnum
    {
        None,
        User,
        Issuer,
        Admin
    }
}