using System;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.Filters;
using Ecuafact.Web.Models;
using Ecuafact.Web.Domain.Entities;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize]
    public class SRIController : AppControllerBase
    {

		public ServicioUsuario servicioUsuario = new ServicioUsuario();
        //
        // GET: /SRI/

        public ActionResult Conectar()
        {
            return PartialView();
        }

        public ActionResult Index()
        {
			if(SessionInfo.Issuer?.RUC != null)
				ViewBag.RUC = SessionInfo.Issuer.RUC;

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txtclavesri"></param>
        /// <returns></returns>
		public ActionResult CambiarClave(string txtclavesri)
		{
			try
			{
				if (string.IsNullOrEmpty(SecurityToken))
					return RedirectToAction("Index", "Auth");

				string ruc = SessionInfo.Issuer?.RUC ?? SessionInfo.UserInfo.Username;

				if (txtclavesri.Count() > 2)
				{
					servicioUsuario.ActualizaClaveSri(ruc, txtclavesri);
					return Json(new { id = 1, description = "La clave se guardó satisfactoriamente!" }, JsonRequestBehavior.AllowGet);
				}
				return Json(new { id = 0, description = "La clave es muy corta!" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { id = 0, description = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

        [HttpPost]
		public ActionResult CambiarClaveAPI(SRIRequestModel model)
		{
			try
			{
				if (string.IsNullOrEmpty(SecurityToken))
					return RedirectToAction("Index", "Auth");

				if (model?.SRIPassword?.Count() > 2)
				{
					var response = ServicioUsuario.LoginSri(SecurityToken, model.RUC, model.SRIPassword);					
					if (response.result != null)
					{
						if (response.result?.code == "1000")
						{
							SessionInfo.UserInfo.SRIConnected = true;

							return Json(new { id = 1, description = response.result.message, response.result }, JsonRequestBehavior.AllowGet);
						}
						else
						{
							return Json(new { id = 0, description = response.result.message, response.result }, JsonRequestBehavior.AllowGet);
						}
					}
					else
					{
						return Json(new { id = 1, description = "La clave se guardó satisfactoriamente!" }, JsonRequestBehavior.AllowGet);
					}
				}

				return Json(new { id = 0, description = "La clave es muy corta!" }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { id = 0, description = "Error: " + ex.Message, error = ex.ToString() }, JsonRequestBehavior.AllowGet);
			}
		}		
    }
}
