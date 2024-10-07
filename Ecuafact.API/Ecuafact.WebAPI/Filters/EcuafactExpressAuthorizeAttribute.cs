using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models;

namespace Ecuafact.WebAPI.Filters
{
    /// <summary>
    /// EcuafactExpress Authorization Attribute:
    /// Used by the authorization methods for the API.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class EcuafactExpressAuthorizeAttribute : AuthorizeAttribute
    {
        public bool AdminAccess { get; set; }
        
        /// <summary>
        /// Override OnAuthorization method
        /// </summary>
        /// <param name="ActionContext">Controller context</param>
        public override void OnAuthorization(HttpActionContext ActionContext)
        {
            try
            { 
                // Buscamos el token enviado a traves del encabezado
                var token = ActionContext.Request.Headers
                    .TryGetValues("TokenId", out IEnumerable<string> headerValues)
                        ? headerValues.FirstOrDefault()
                        : ActionContext.Request.Headers?.Authorization?.Parameter;

                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("No se especificó el token de seguridad");
                }
                

                if (AdminAccess && Constants.ServiceToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                {
                    HttpContext.Current.Session["IsAdmin"] = true;
                    return;
                }

                var requestSession = ActionContext.GetRequestSession(token);

                if (requestSession == null)
                {
                    throw new Exception("El token es inválido o ha expirado");
                }

                HttpContext.Current.Session["issuer"] = ActionContext.GetAuthenticatedIssuer(requestSession.IssuerId);
            }
            catch (Exception ex)
            {
                ActionContext.Response = ActionContext.Request.BuildHttpErrorResponse(HttpStatusCode.Unauthorized, ex.ToString(), ex.Message);
            }

        }

    }




    /// <summary>
    /// EcuafactApp Authorization Attribute:
    /// Used by the authorization methods for the API.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class EcuafactAppAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Override OnAuthorization method
        /// </summary>
        /// <param name="ActionContext">Controller context</param>
        public override void OnAuthorization(HttpActionContext ActionContext)
        {
            try
            {
                // Buscamos el token entre los encabezados
                var token = ActionContext.Request.Headers
                    .TryGetValues("TokenId", out IEnumerable<string> headerValues)
                        ? headerValues.FirstOrDefault()
                        : ActionContext.Request.Headers?.Authorization?.Parameter;

                if (!string.IsNullOrEmpty(token))
                {
                    if (!Constants.ServiceToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var response = ActionContext.Request.ValidateClientToken(token);

                        if (response.IsSuccess)
                        {
                            HttpContext.Current.Session["login"] = response;
                        }
                        else
                        {
                            throw new Exception(response.UserMessage);
                        }
                    }
                }
                else
                {
                    throw new Exception("No se especificó el token de seguridad");
                }
            }
            catch (Exception ex)
            {
                ActionContext.Response = ActionContext.Request.BuildHttpErrorResponse(HttpStatusCode.Unauthorized, ex.ToString(), ex.Message);
            }

        }
     }


    public static class ActionContextExtensions
    {

        public static RequestSession GetRequestSession(this HttpActionContext ActionContext, string token)
        {
            return ActionContext.Request
                .GetService<IRequestSessionsService>()
                .GetSessionByToken(token);
        }

        public static IssuerDto GetAuthenticatedIssuer(this HttpActionContext ActionContext, long issuerId)
        {
            return ActionContext.Request
                .GetIssuersService()
                .GetIssuer(issuerId)?
                .ToIssuerDto();
        }
    }
}