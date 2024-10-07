using Ecuafact.WebAPI.Filters;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Ecuafact.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();


            // Web API configuration and services
            config.Filters.Add(new ExpressPagedListActionFilterAttribute());
            config.Filters.Add(new ExpressExceptionFilterAttribute());

            config.Formatters.Remove(config.Formatters.XmlFormatter);

        }
    }
}
