using Ecuafact.Web.Domain.Entities.API;
using Ecuafact.Web.Models;
using Ecuafact.Web.Models.Security;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;

namespace Ecuafact.Web
{

    namespace Models.Security
    {
        public interface IExpressUserLogin : IPrincipal
        {
            List<LoginIssuerModel> Issuers { get; set; }

            ClientModel UserInfo { get; set; }

            LoginIssuerModel CurrentIssuer { get; set; }

            SecuritySessionRole UserRole { get; }
        }


        internal class ExpressUserLogin : LoginResponseModel, IExpressUserLogin
        {
            internal ExpressUserLogin()
            {
                this.Error = new ErrorModel { Code = "404", Message = "Usuario no existe!" };
            }

            internal ExpressUserLogin(LoginResponseModel model)
            {
                this.Error = model.Error;
                this.CurrentIssuer = model.CurrentIssuer;
                this.Result = model.Result;
                this.UserInfo = model.UserInfo;
                this.Issuers = model.Issuers;
            }


            public IIdentity Identity { get; set; }

            public SecuritySessionRole UserRole
            {
                get
                {
                    if (UserInfo != null)
                    {
                        if (CurrentIssuer != null)
                        {
                            if (CurrentIssuer.RUC.Contains(UserInfo.Username))
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

            public static IExpressUserLogin User
            {
                get
                {
                    return (Thread.CurrentPrincipal as IExpressUserLogin) ?? new ExpressUserLogin();
                }
            }
        }
    }


    public static class SecurityExtensions
    {
        public static IExpressUserLogin ToLogin(this LoginResponseModel model)
        {
            return new ExpressUserLogin(model);
        }

        public static IPrincipal ToPrincipal(this IExpressUserLogin login)
        {
            return login;
        }
         
        public static IExpressUserLogin ToLogin(this IPrincipal principal)
        {
            return (principal as IExpressUserLogin);
        }

        public static IExpressUserLogin CurrentUser(this Thread thread)
        {
            return ExpressUserLogin.User;
        }
    }
}