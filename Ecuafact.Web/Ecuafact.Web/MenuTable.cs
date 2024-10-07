using EcuafactExpress.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EcuafactExpress.Web
{
    public static class MenuTable
    {
        static MenuTable()
        {
            MenuItems = NavMenuItemCollection.Default;
        }

        public static NavMenuItemCollection MenuItems { get; private set; }
    }
}