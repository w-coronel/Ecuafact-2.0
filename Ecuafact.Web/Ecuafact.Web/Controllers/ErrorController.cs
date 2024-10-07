using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecuafact.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            var json = GetRequestJSON();

            return View(new HandleErrorInfo(new Exception("Ha ocurrido un error inesperado.") {  Source = json }, "Error", "Index"));
        }

        private string GetRequestJSON()
        {
            // Genero el JSON para los datos recibidos:
            var keys = Request.Form.AllKeys;
            var json = "{";
            foreach (var key in keys)
            {
                if (key != keys.FirstOrDefault())
                {
                    json += ", ";
                }

                json += "\"" + key + "\" : " + "\"" + Request.Form[key] + "\" ";
            }

            json += " }";

            return json;
        }
         
    }
}