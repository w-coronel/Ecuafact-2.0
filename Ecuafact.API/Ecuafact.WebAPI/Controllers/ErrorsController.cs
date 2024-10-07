using Ecuafact.WebAPI.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Ecuafact.WebAPI.Controllers
{
    
    [Obsolete]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class ErrorsController : ApiController
    {
        public OperationResult GetError(HttpStatusCode id)
        {
            var title = "";
            try
            {
                var result = new HttpClient().GetStringAsync($"https://developer.mozilla.org/es/docs/Web/HTTP/Status/{Convert.ToInt32(id)}").Result;
                title = result.Substring(result.IndexOf("<h1>") + 4, result.IndexOf("</h1>") - result.IndexOf("<h1>") - 4);
            }
            catch (Exception ex)
            {
                if( Enum.IsDefined(typeof(HttpStatusCode), id))
                {
                    title = Enum.GetName(typeof(HttpStatusCode), id);
                }
                else
                {
                    title = "Custom - (Personalizado)";
                }

                title = $"{Convert.ToInt32(id)} {title}";
                ex.ToString();
                
            }

            return new OperationResult(id < HttpStatusCode.BadRequest, id)
            {
                DevMessage = $"Documentacion en: https://developer.mozilla.org/es/docs/Web/HTTP/Status/{Convert.ToInt32(id)}",
                UserMessage = title
            };
        }
    }
}
