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
    public class IssuePointController : AppControllerBase
    {
        [ExpressAuthorize]
        public async Task<ActionResult> Index(long establishmentId)
        {
            var model = await ServicioEmisor.SearchIssuePointAsync(IssuerToken, establishmentId);
            ViewBag.EstablishmentId = establishmentId;
            return View(model);
        }       

        public async Task<ActionResult> Crear(long establishmentId)
        {
            return PartialView("_mantIssuePointPartial", new IssuePoint { EstablishmentsId = establishmentId, IssuerId = SessionInfo.Issuer.Id });
        }

        public async Task<ActionResult> Editar(long id)
        {
            var model = await ServicioEmisor.GetIssuePointByIdAsync(IssuerToken, id);            
            return PartialView("_mantIssuePointPartial", model);
        }

        [HttpPost]
        [HandleJsonException]
        public async Task<JsonResult> GuardarAsync(IssuePoint model)
        {
            try
            {
                HttpResponseMessage response = null;
                if (model.Id > 0)
                {
                    response = await ServicioEmisor.ActualizarIssuePointAsync(IssuerToken, model);
                }
                else
                {
                    response = await ServicioEmisor.CrearIssuePointAsync(IssuerToken, model);
                }

                var text = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var issuePoint = JsonConvert.DeserializeObject<IssuePoint>(text);
                    return new JsonResult
                    {
                        Data = new
                        {
                            id = issuePoint.Id,
                            result = issuePoint,
                            error = default(object),
                            status = response.StatusCode,
                            message = "Punto Emisión guardado con exito!"
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
                        message = errors.UserMessage ?? "Error en los datos del punto emisón!"
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

        [HttpPost]
        public async Task<JsonResult> BuscarAsync(string ruc)
        {
            try
            {

                if (string.IsNullOrEmpty(ruc))
                {
                    return Json(new OperationResult(false, HttpStatusCode.BadRequest), JsonRequestBehavior.AllowGet);
                }
                var result = await Task.FromResult(ServicioEmisor.BuscarEmisorPorRUC(SecurityToken, ruc));

                if (result != null)
                {
                    return Json(new OperationResult<CarrierIssuer>(true, HttpStatusCode.OK, result), JsonRequestBehavior.AllowGet);
                }
               
            }
            catch (Exception ex)
            {
                return Json(new OperationResult(ex), JsonRequestBehavior.AllowGet);
            }

            return Json(new OperationResult(false, HttpStatusCode.BadRequest), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleJsonException]
        public async Task<HttpResponseMessage> EliminarAsync(long? id)
        {
            return await ServicioEmisor.EliminarIssuePointAsync(IssuerToken, id.Value);
        }
    }
}