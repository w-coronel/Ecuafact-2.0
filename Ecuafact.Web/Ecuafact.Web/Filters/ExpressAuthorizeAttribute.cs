using System;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Ecuafact.Web.Models;
using Ecuafact.Web.Controllers;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using System.Threading;

namespace Ecuafact.Web.Filters
{

    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ExpressAuthorizeAttribute : AuthorizeAttribute
    {

        public SecuritySessionRole UserRole
        {
            get
            {
                if (SessionInfo.LoginInfo?.UserInfo != null)
                {
                    if (SessionInfo.UserSession != null)
                    {
                        if (SessionInfo.UserSession.IssuerRUC.StartsWith(SessionInfo.UserSession.Username))
                        {
                            return SecuritySessionRole.Admin;
                        }
                        else
                        {
                            return SecuritySessionRole.Issuer;
                        }
                    }
                    else
                    {
                        return SecuritySessionRole.User;
                    }
                }
                else
                {
                    return SecuritySessionRole.None;
                }
            }
        }

        public string ViewName { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!SessionInfo.IsLoggedIn)
            {

                if (SessionInfo.LoginInfo == null)
                {
                    bool wasLoggedIn = (filterContext.HttpContext.User != null && filterContext.HttpContext.User.Identity.IsAuthenticated);
                    var token = filterContext.HttpContext.User?.Identity?.Name;

                    if (string.IsNullOrEmpty(token) && filterContext.HttpContext.Request.Cookies[AuthController.USER_COOKIE] != null)
                    {
                        token = filterContext.HttpContext.Request.Cookies[AuthController.USER_COOKIE].Value.Decode();
                    }

                    // Si el usuario ha iniciado sesion pero no
                    // se encuentra cargada su informacion se debe cargar sus datos localmente
                    if (wasLoggedIn)
                    {
                        //proceso de validacion:
                        var response = ServicioUsuario.ValidateToken(token);

                        if (response.UserInfo != null)
                        {
                            AuthController.PerformIssuerAuthentication(response, true);
                            
                            filterContext.HttpContext.User = Thread.CurrentPrincipal;
                            return;
                        }
                    }

                    // Verificamos si el cliente decidio recordar la sesion en esta maquina:
                    

                    //Si no existe la informacion del usuario entonces se tiene que cerrar la sesion
                    // para iniciar nuevamente.
                    FormsAuthentication.SignOut();
                    filterContext.RequestContext.HttpContext.Session.Clear();

                    var returnUrl = filterContext.HttpContext?.Request?.Url?.AbsolutePath?.ToLower();
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        if (!returnUrl.Contains("auth"))
                        { 
                            if (returnUrl.Contains("dashboard"))
                            {
                                returnUrl = $"auth";
                            }
                            else
                            {
                                returnUrl = $"auth?returnUrl={returnUrl}";
                            }
                        }
                    }

                    filterContext.Result = new RedirectResult($"~/{returnUrl}");
                }

                //filterContext.Result = new RedirectResult($"~/auth?returnUrl={filterContext.HttpContext?.Request?.Url?.AbsolutePath}");
            }
        }


        public bool IsInRole(string role)
        {
            if (string.IsNullOrEmpty(role?.Trim()) || role.ToLower() == "none")
            {
                return true;
            }

            if (role?.ToLower() == "none" && UserRole >= SecuritySessionRole.None)
            {
                return true;
            }

            if (role?.ToLower() == "user" && UserRole >= SecuritySessionRole.User)
            {
                return true;
            }

            if (role?.ToLower() == "issuer" && UserRole >= SecuritySessionRole.Issuer)
            {
                return true;
            }

            if (role?.ToLower() == "admin" && UserRole >= SecuritySessionRole.Admin)
            {
                return true;
            }

            return false;
        }


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.User;

            if (user != null)
            {
                if (user.Identity.IsAuthenticated)
                {
                    if (string.IsNullOrEmpty(Roles))
                    {
                        return true;
                    }

                    foreach (var role in Roles.Split(',', ';', '|', '/'))
                    {
                        if (this.IsInRole(role))
                        {
                            return true;
                        }
                    }

                }
            }


            return base.AuthorizeCore(httpContext);
        }
    }
}