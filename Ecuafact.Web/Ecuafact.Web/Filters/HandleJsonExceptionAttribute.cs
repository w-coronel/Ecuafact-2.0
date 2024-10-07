using System.Web.Mvc;

namespace Ecuafact.Web
{
    public class HandleJsonExceptionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception != null)
            {
                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                filterContext.Result = new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        filterContext.Exception.Message,
                        filterContext.Exception.StackTrace,
                        filterContext.Exception.Data 
                    }
                };
                filterContext.ExceptionHandled = true;
            }
        }
        
    }

}