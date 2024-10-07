using System.Web.Mvc;

namespace Ecuafact.Web.Areas.Carrier
{
    public class CarrierAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Carrier";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Carrier_default",
                "Carrier/{controller}/{action}/{id}",
                new { controller = "Establishment", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] {"Ecuafact.Web.Areas.Carrier.Controllers" }
            );
        }
    }
}