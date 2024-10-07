using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using X.PagedList;

namespace Ecuafact.WebAPI.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpressPagedListActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(
            HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            var objectContent = actionExecutedContext?.Response?.Content as ObjectContent;

            if (objectContent != null)
            {
                var pagedList = objectContent.Value as IPagedList;

                if (pagedList != null)
                {
                    // Agregamos la informacion requerida por los servicios de paginacion.
                    actionExecutedContext.Response.Headers.Add("X-Page-Number", pagedList.PageNumber.ToString(CultureInfo.InvariantCulture));
                    actionExecutedContext.Response.Headers.Add("X-Page-Size", pagedList.PageSize.ToString(CultureInfo.InvariantCulture));
                    actionExecutedContext.Response.Headers.Add("X-Page-Pages", pagedList.PageCount.ToString(CultureInfo.InvariantCulture));
                    actionExecutedContext.Response.Headers.Add("X-Page-Count", pagedList.TotalItemCount.ToString(CultureInfo.InvariantCulture));
                }
            }
        }
    } 

}