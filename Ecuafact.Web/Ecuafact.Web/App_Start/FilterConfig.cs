using Ecuafact.Web;
using System.Web;
using System.Web.Mvc;

namespace EcuafactExpress.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleJsonExceptionAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
        }
    }
}
