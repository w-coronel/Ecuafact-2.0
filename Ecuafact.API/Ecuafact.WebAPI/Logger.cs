using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Http;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;

namespace Ecuafact.WebAPI
{

    /// <summary>
    /// 
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="origen"></param>
        /// <param name="messages"></param>
        public static void Log(string origen, params object[] messages)
        {
            try
            {
                if (Constants.EnableLog)
                {
                    var curretDirectory = HttpContext.Current?.Server?.MapPath("/");
                    var logLocation = Constants.LogLocation;

                    if (string.IsNullOrEmpty(logLocation))
                    {
                        logLocation = Path.Combine(curretDirectory, "Logs");
                    }

                    if (!Path.IsPathRooted(logLocation))
                    {
                        logLocation = Path.Combine(curretDirectory, logLocation);
                    }

                    if (!Directory.Exists(logLocation))
                    {
                        Directory.CreateDirectory(logLocation);
                    }

                    var filename = Path.Combine(logLocation, $"API.{origen}.{DateTime.Now.ToFileTime():X}.log");

                    // Generamos el mensaje para el log
                    StringBuilder message = new StringBuilder();
                    message.AppendLine($"{DateTime.Now} | {Thread.CurrentPrincipal.Identity?.Name}");

                    var issuer = HttpContext.Current.Session.GetAuthenticatedIssuerSession();
                    if (issuer != null)
                    {
                        message.AppendLine($"{issuer?.Id} | {issuer?.RUC} | {issuer?.BussinesName}");
                    }

                    message.AppendLine();

                    foreach (var item in messages)
                    {
                        if (item is HttpResponseException)
                        {
                            var ex = item as HttpResponseException;
                            var text = ex.Response?.Content?.ReadAsStringAsync()?.Result; ;
                            message.AppendLine(text);
                        }
                        else if (item is HttpResponseMessage)
                        {
                            var response = item as HttpResponseMessage;
                            message.AppendLine(response?.Content?.ReadAsStringAsync()?.Result);
                        }
                        else if (item is Exception)
                        {
                            var ex = item as Exception;
                            message.AppendLine(ex.ToString());
                        }
                        else if (item is IConvertible) // Es texto, numero o valor convertible
                        {
                            var ex = item;
                            message.AppendLine(ex.ToString());
                        }
                        else // Es un objeto cualquiera - Lo convierte en JSON
                        {
                            try
                            {
                                var obj = JsonConvert.SerializeObject(item);
                                message.AppendLine(Convert.ToString(obj));
                            }
                            catch
                            {
                                message.AppendLine(Convert.ToString(item));
                            } 
                        }

                        message.AppendLine();
                    }
                     
                    if (File.Exists(filename))
                    {
                        File.AppendAllLines(filename, new string[] { "*".PadRight(100, '*'), // agregamos una division de mensajes
                            message.ToString()
                        });
                    }
                    else
                    {
                        File.WriteAllText(filename, message.ToString());
                    }
                }
            }
            catch (Exception)
            {
                // Si hay errores al escribir la informacion
                // No se realiza el log
            }
        }

    }
}