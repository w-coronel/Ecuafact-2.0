using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EcuafactExpress.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
             name: "Carrier_auth",
             url: "carrier",
             defaults: new { controller = "Establishment", action = "Index" },
             namespaces: new string[] { "Ecuafact.Web.Areas.Carrier.Controllers" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Ecuafact.Web.Controllers" }
            );

        }
    }
}
