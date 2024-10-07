using Ecuafact.Web.Controllers;
using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Services;
using Ecuafact.Web.Filters;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Ecuafact.Web.Areas.Carrier.Controllers
{
    public class EstablishmentController : AppControllerBase, IController
    {
        [ExpressAuthorize]
        public async Task<ActionResult> Index()
        {
            var model = await ServicioEmisor.SearchEstablishmentsAsync(IssuerToken);
            return View(model);
        }

        #region Establecimiento
        public ActionResult CrearEstablecimiento()
        {
            return  PartialView("_mantEstablishmentPartial", new Establishments());
        }

        public async Task<ActionResult> EditarEstablecimiento(long id)
        {
            var model = await ServicioEmisor.GetEstablishmentsByIdAsync(IssuerToken, id);
            return PartialView("_mantEstablishmentPartial", model);
        }

        [HttpPost]
        [HandleJsonException]
        public async Task<JsonResult> GuardarEstablecimientoAsync(Establishments model)
        {
            try
            {                
                HttpResponseMessage response = null;
                if (model.Id > 0)
                {
                    response = await ServicioEmisor.ActualizarEstablishmentsAsync(IssuerToken, model);
                }
                else
                {
                    response = await ServicioEmisor.CrearEstablishmentsAsync(IssuerToken, model);
                }

                var text = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var estab = JsonConvert.DeserializeObject<Establishments>(text);
                    return new JsonResult
                    {
                        Data = new
                        {
                            id = estab.Id,
                            result = estab,
                            error = default(object),
                            status = response.StatusCode,
                            message = "Establecimiento Guardado con exito!"
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        ContentType = "application/json"
                    };
                }

                var errors = JsonConvert.DeserializeObject<OperationResult>(text);

                return new JsonResult
                {
                    Data = new
                    {
                        id = 0,
                        result = default(object),
                        error = errors,
                        status = response.StatusCode,
                        message = errors.UserMessage ?? "Error en los datos del establecimiento!"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentType = "application/json"
                };
            }
            catch (Exception ex)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        id = 0,
                        result = default(object),
                        error = GetValidationError(),
                        status = HttpStatusCode.InternalServerError,
                        statusText = ex.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentType = "application/json"
                };
            }




        }

        #endregion


    }
}