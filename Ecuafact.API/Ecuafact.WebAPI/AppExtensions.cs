using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.Models.Authentication;
using System.IO;
using System.Security;
using System.Web.Hosting;

namespace System
{
    namespace Web
    {
        internal static class ContextExtensions
        {
            public static string GetLogoFileName(this HttpContext ctx, string logo)
            {
                var filename = logo;
                if (System.IO.File.Exists(filename))
                {
                    return filename;
                }
                else
                {
                    filename = $"~/{logo}";
                    if (System.IO.File.Exists(HostingEnvironment.MapPath(filename)))
                    {
                        return filename;
                    }
                    else
                    {
                        filename = $"~/Logos/{logo}";
                        if (System.IO.File.Exists(HostingEnvironment.MapPath(filename)))
                        {
                            return filename;
                        }
                        else
                        {
                            filename = $"~/Logos/{logo}.jpg";
                            if (System.IO.File.Exists(HostingEnvironment.MapPath(filename)))
                            {
                                return filename;
                            }
                            else
                            {
                                filename = $"~/Logos/{logo}_logo.jpg";
                                if (System.IO.File.Exists(HostingEnvironment.MapPath(filename)))
                                {
                                    return filename;
                                }
                            }
                        }
                    }
                }

                return HostingEnvironment.MapPath($"~/Logos/no_logo.jpg");
            }

            public static byte[] GetFileContent(this HttpContext ctx, string filename)
            {
                byte[] file = new byte[0];

                if (File.Exists(filename))
                {
                    try
                    {
                        file = File.ReadAllBytes(filename);
                    }
                    catch { }
                }

                return file;
            }
        }

        namespace Http
        {
            internal static class ControllerExtensions
            {

                public static LoginResponseModel GetAuthenticatedUser(this ApiController controller)
                {
                    try
                    {
                        var user = HttpContext.Current.Session.GetAuthenticatedUser();
                        return user?.Entity;
                    }
                    catch (Exception ex)
                    {
                        throw new SecurityException("No se pudo tener acceso a la informacion del emisor especificado.", ex);
                    }
                }

                public static IssuerDto GetAuthenticatedIssuer(this ApiController controller)
                {
                    try
                    {
                        var issuerAuth = HttpContext.Current.Session.GetAuthenticatedIssuerSession();

                        return issuerAuth;
                    }
                    catch (Exception ex)
                    {
                        throw new SecurityException("No se pudo tener acceso a la informacion del emisor especificado.", ex);
                    }
                }

                public static bool IsAdmin(this ApiController controller)
                {
                    try
                    {
                        return Convert.ToBoolean(HttpContext.Current.Session["IsAdmin"] ?? false);
                    }
                    catch { return false; }
                }
            }
        }
    }

    namespace Collections.Generic
    {
        internal static class ListExtensions
        {
            public static List<T> Empty<T>(this List<T> list)
            {
                if (list == null)
                {
                    list = new List<T>();
                }
                else
                {
                    list.Clear();
                }

                return list;
            }
        }
    }
} 