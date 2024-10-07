using Ecuafact.Web;
using EcuafactExpress.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EcuafactExpress.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {         
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MenuConfig.RegisterMenus(MenuTable.MenuItems);
            Constants.SetAppSettings(WebConfigurationManager.AppSettings);
            MvcHandler.DisableMvcResponseHeader = true;
            
        }
        
    }
}
