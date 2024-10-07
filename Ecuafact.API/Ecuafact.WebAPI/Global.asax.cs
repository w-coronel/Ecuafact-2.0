using Sentry;
using System.Web;
using System.Web.Configuration;
using System.Web.Http; 
using System.Web.Routing;
using System.Web.SessionState;

namespace Ecuafact.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            SentrySdk.Init(Constants.SentryToken);

            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AutofacConfig.Initialize(GlobalConfiguration.Configuration);
            // Configuro el appSettings a utilizar : web.config
            Constants.SetAppSettings(WebConfigurationManager.AppSettings);
            Constants.SetPath(Server.MapPath("~/"));
        }

        protected void Session_Start()
        {
            string sessionID = Session.SessionID;
        }

        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        } 
    }
}
