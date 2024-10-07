using Microsoft.Reporting.Map.WebForms.BingMaps;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;

namespace Ecuafact.WebAPI.Controllers
{
    [Obsolete]
    [ApiExplorerSettings(IgnoreApi=true)]
    public class DefaultController : ApiController
    {
        [HttpGet]
        [Route("home")]
        public HttpResponseMessage home()
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(
                    Template.Replace("{{api-file}}", Url.Content("~/swagger/docs/api?specs.json")), 
                    Encoding.UTF8, "text/html")
            };
        }

        [HttpGet]
        [Route("admin")]
        public HttpResponseMessage admin()
        {
            var html = Template.Replace("{{api-file}}", Url.Content("~/swagger/docs/admin?specs.json"));
            return new HttpResponseMessage
            {
                Content = new StringContent(html, Encoding.UTF8, "text/html")
            };
        }


        static string template_html;
        private static string Template
        {
            get
            {
                if (string.IsNullOrEmpty(template_html))
                {
                    var file = HostingEnvironment.MapPath("~/_home.cshtml");
                    if (File.Exists(file))
                    {
                        template_html = File.ReadAllText(file);
                    }
                }

                return template_html;
            }
        }
    }
}
