using EcuafactExpress.Web.Models;
using Owin;
using Microsoft.Owin;
[assembly: OwinStartup(typeof(EcuafactExpress.Web.Startup))]

namespace EcuafactExpress.Web
{
 
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
     
}