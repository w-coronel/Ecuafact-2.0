using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecuafact.Web.Controllers
{
    public class InicioController : Controller
    {
        // GET: Bienvenido
        public ActionResult Index()
        {
            return View();
        }
    }
}