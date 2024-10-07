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

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize]
    public class CooperativaController : AppControllerBase
    {

        public async Task<ActionResult> Index()
        {
            if (SessionInfo.UserSession?.Subscription != null)
            {
                if (SessionInfo.UserSession?.Subscription.Status == SubscriptionStatusEnum.Activa && SessionInfo.UserSession.Issuer.IsCooperativeCarrier
                    && SessionInfo.UserSession?.Subscription.LicenceType.Code == Constants.PlanPro)
                {
                    var model = await ServicioEmisor.SearchEstablishmentsAsync(IssuerToken);
                    return View(model);
                }
            }

            return RedirectToAction("index", "Dashboard");
        }

        #region Establecimiento
        public ActionResult CrearEstablecimiento()
        {
            return PartialView("_mantEstablishmentPartial", new Establishments());
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

        #region PuntoEmision
        public async Task<ActionResult> PuntoEmision(long establishmentId)
        {
            if (SessionInfo.UserSession?.Subscription != null)
            {
                if (SessionInfo.UserSession?.Subscription.Status == SubscriptionStatusEnum.Activa && SessionInfo.UserSession.Issuer.IsCooperativeCarrier
                    && SessionInfo.UserSession?.Subscription.LicenceType.Code == Constants.PlanPro)
                {
                    var model = await ServicioEmisor.SearchIssuePointAsync(IssuerToken, establishmentId);
                    ViewBag.EstablishmentId = establishmentId;
                    return View(model);
                }
            }
            return RedirectToAction("index", "Dashboard");
        }

        public async Task<ActionResult> CrearPuntoEmision(long establishmentId)
        {
            return PartialView("_mantIssuePointPartial", new IssuePoint { EstablishmentsId = establishmentId, IssuerId = SessionInfo.Issuer.Id });
        }

        public async Task<ActionResult> EditarPuntoEmision(long id)
        {
            var model = await ServicioEmisor.GetIssuePointByIdAsync(IssuerToken, id);
            return PartialView("_mantIssuePointPartial", model);
        }

        [HttpPost]
        [HandleJsonException]
        public async Task<JsonResult> GuardarPuntoEmiAsync(IssuePoint model)
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
                    var _issuePoint = SessionInfo.UserSession?.Subscription.AmountIssuePoint ?? 0;
                    if(_issuePoint > 0)
                    {
                        var issuePoints = await ServicioEmisor.GetIssuePointsByIssuer(IssuerToken);
                        var amountIssuePoint = issuePoints?.Count ?? 0;
                        if (amountIssuePoint > 0)
                        {
                            if(amountIssuePoint == _issuePoint)
                            {
                                return new JsonResult
                                {
                                    Data = new
                                    {
                                        id = 0,
                                        result = default(object),
                                        error = $"La cantidad de puntos de emisión del plan adquirido, ha llegado al límite de {_issuePoint} puntos de emisión!",
                                        status = HttpStatusCode.NotFound,
                                        message = $"La cantidad de puntos de emisión del plan adquirido, ha llegado al límite de {_issuePoint} puntos de emisión!"
                                    },
                                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                    ContentType = "application/json"
                                };
                            }
                        }
                    }
                   
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
        public async Task<JsonResult> BuscarPuntoEmiAsync(string ruc)
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
        public async Task<HttpResponseMessage> EliminarPuntoEmiAsync(long? id)
        {
            return await ServicioEmisor.EliminarIssuePointAsync(IssuerToken, id.Value);
        }

        #endregion
    }
}