using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Ecuafact.WebAPI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "swagger_root",
                routeTemplate: "",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler((message => message.RequestUri.ToString()), "home")
            );

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { controller = "Default", id = RouteParameter.Optional }
            );
             
        }
    }
}
