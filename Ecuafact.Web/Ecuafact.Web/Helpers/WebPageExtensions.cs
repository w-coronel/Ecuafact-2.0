using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace System.Web.WebPages
{
    public static class WebPageExtensions
    { 
        public static CultureInfo GetCultureInfo(this WebViewPage page, string cultureUI)
        {
            return CultureInfo.GetCultureInfo(cultureUI);
        }

        public static PageOptions PageOptions(this WebViewPage page, Action<PageOptions> predicate)
        {
            var options = PageOptions(page);

            predicate?.Invoke(options);

            return options;
        }

        public static TProperty GetOption<TProperty>(this WebViewPage page, Func<PageOptions, TProperty> predicate)
        {
            var options = PageOptions(page);
            if (predicate != null)
            {
                return predicate.Invoke(options);
            }
            else
            {
                return default;
            }
        }

        public static PageOptions PageOptions(this WebViewPage page)
        {
            PageOptions options = page.ViewData["WebViewPage.PageOptions"] as PageOptions;

            return options ?? new PageOptions(page);
        }

        public static MenuOption Menu(this WebViewPage page, string title, string url = "", string icon = "", string onClick = "", bool enabled = true, long badgeCount = 0, string badgeType = "")
        {
            var options = Menu(page);
            return options.Add(title, url, icon, onClick, enabled, badgeCount, badgeType);
        }

        public static WebViewPage Menu(this WebViewPage page, Action<MenuOptionCollection> menu)
        {
            var options = Menu(page);
            menu(options);
            return page;
        }

        public static MenuOptionCollection Menu(this WebViewPage page)
        {
            var options = page.ViewData["WebViewPage.MainMenu"] as MenuOptionCollection;

            return options ?? new MenuOptionCollection(page);
        }



    }


    public class MenuOptionCollection : List<MenuOption>
    {
        private int __active = 0;
        public int Active
        {
            get
            {
                return __active;
            }
        }

        /// <summary>
        /// Agrega un nuevo item
        /// </summary>
        /// <param name="title">titulo</param>
        /// <param name="url">direccion url</param>
        /// <param name="icon">icono</param>
        /// <returns></returns>
        public MenuOption Add(string title, string url = "", string icon = "", string onClick = "", bool enabled = true, long badgeCount = 0, string badgeType = "")
        {
            int id = this.Count() + 1;
            var option = new MenuOption(id, title, url, icon, onClick, enabled, badgeCount, badgeType);
            this.Add(option);
            return option;
        }

        internal MenuOptionCollection() { }

        public MenuOptionCollection(WebViewPage page)
        {
            page.ViewData["WebViewPage.MainMenu"] = this;
        }

    }


    public class MenuOption : QuickNav
    {
        public string OnClick { get; set; }
        public bool Enabled { get; set; } = true;
        public long BadgeCount { get; set; }
        public string BadgeType { get; set; }

        public MenuOptionCollection Items { get; private set; } = new MenuOptionCollection();

        public MenuOption(int id, string title, string url = "", string icon = "", string onClick = "", bool enabled = true, long badgeCount = 0, string badgeType = "")
            : base(title, url, icon)
        {
            this.Id = id;
            this.OnClick = onClick;
            this.Enabled = enabled;
            this.BadgeCount = badgeCount;
            this.BadgeType = badgeType;
        }

        public MenuOption AddItem(string title, string url = "", string icon = "", string onClick = "", bool enabled = true, long badgeCount = 0, string badgeType = "")
        {
            var menuItem = new MenuOption(this.Items.Count() + 1, title, url, icon, onClick, enabled, badgeCount, badgeType);
            this.Items.Add(menuItem);
            return this;
        }
    }

    public static class QuickNavHelpers
    {
        public static QuickNavCollection QuickNav(this WebViewPage page, IEnumerable<QuickNav> items)
        {
            var quicknavs = QuickNav(page);

            foreach (var item in items)
            {
                quicknavs.Add(item);
            }

            return quicknavs;
        }


        public static QuickNavCollection QuickNav(this WebViewPage page)
        {
            QuickNavCollection quicknav = page.ViewData["WebViewPage.QuickNav"] as QuickNavCollection;

            return quicknav ?? new QuickNavCollection(page);
        }
    }

    public class QuickNav
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string URL { get; set; }
        public string Icon { get; set; }
        public QuickNav() { }
        public QuickNav(string title, string url = "", string icon = "")
        {
            this.Title = title;
            this.URL = url;
            this.Icon = icon;
        }
    }

    public class QuickNavCollection : List<QuickNav>
    {
        /// <summary>
        /// Agrega un nuevo item
        /// </summary>
        /// <param name="title">titulo</param>
        /// <param name="url">direccion url</param>
        /// <param name="icon">icono</param>
        /// <returns></returns>
        public QuickNavCollection Add(string title, string url, string icon)
        {
            this.Add(new QuickNav(title, url, icon));
            return this;
        }

        public QuickNavCollection(WebViewPage page)
        {
            page.ViewData["WebViewPage.QuickNav"] = this;
        }
    }

    public class PageOptions
    {
        WebViewPage __page;
        public PageOptions(WebViewPage page)
        {
            page.ViewData["WebViewPage.PageOptions"] = this;
            __page = page;
        }

        public bool EnableChat
        {
            get
            {
                var enable = __page.Request.Cookies["EnableChat"];

                if (enable != null && enable.Value.ToLower() == "true")
                {
                    return true;
                }
                return false;
            }
            set
            {
                __page.Response.Cookies.Add(new HttpCookie("EnableChat", value.ToString()));
            }
        }

        public bool EnablePage { get; set; }
        public bool CounterUp { get; set; } = false;
        public bool Charts { get; set; } = false;
        public bool Flot { get; set; } = false;
        public bool FullCalendar { get; set; } = false;
        public bool HorizontalTimeline { get; set; } = false;
        public bool Maps { get; set; } = false;
        public bool SparkLines { get; set; } = false;
        public bool DateRangePicker { get; set; } = false;
        public bool DatePicker { get; set; } = false;
        public bool Select2 { get; set; } = false;
        public bool DataTables { get; set; } = false;
        public bool PDFMaker { get; set; } = false;
        public bool JsZip { get; set; } = false;
        public bool JsTree { get; set; } = false;
        public bool SidebarSearch { get; set; } = false;
        public bool DateRangeTool { get; set; } = false;
        public bool DropZone { get; set; }
        public bool InputMask { get; set; }
        public bool RichTextEditor { get; set; }
    }
}