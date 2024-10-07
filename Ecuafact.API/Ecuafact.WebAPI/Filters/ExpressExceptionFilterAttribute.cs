using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Ecuafact.WebAPI.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpressExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
            {

                if (actionExecutedContext.Exception is HttpResponseException)
                {
                    var responseEx = actionExecutedContext.Exception as HttpResponseException;

                    actionExecutedContext.Response = responseEx.Response;
                }
                else
                {
                    string exceptionMessage = string.Empty;

                    if (actionExecutedContext.Exception.InnerException == null)
                    {
                        exceptionMessage = actionExecutedContext.Exception.Message;
                    }
                    else
                    {
                        exceptionMessage = actionExecutedContext.Exception.InnerException.Message;
                    }

                    actionExecutedContext.Response = actionExecutedContext.Request?
                        .BuildHttpErrorResponse(HttpStatusCode.InternalServerError,
                        actionExecutedContext.Exception?.ToString(), exceptionMessage) ?? actionExecutedContext.Response;
                     
                }

                Logger.Log("ERROR.LOG", "[Excepcion]", actionExecutedContext.Exception?.ToString());
            }
        }
    }
}