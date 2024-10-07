
using Ecuafact.Web.MiddleCore.ApplicationServices;
using System.Collections.Generic;
using System.Web.Mvc;
using Ecuafact.Web.Domain.Entities;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using Ecuafact.Web.Reporting;
using Ecuafact.Web.Filters;
using System;
using System.Globalization;
using Ecuafact.Web.Models;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Configuration;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize(Roles ="Issuer|Admin")]
    public class FacturaController : DocumentControllerBase
    {
        protected override string SessionName => SessionInfo.FACTURAS_SESSION;

        #region Acciones del controlador
         
        protected override string DocumentType => "01";        

        protected override object DocumentById(string id)
        {
            InvoiceModel model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = ServicioFactura.BuscarFacturaPorId(IssuerToken, id);                
                model.Status = (model.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(model.AuthorizationDate)) ? DocumentStatusEnum.Authorized : model.Status;
            }

            return model;
        }

        public ActionResult Editar(string id = null)
        {
            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            {
                var model = ServicioFactura.BuscarFacturaPorId(IssuerToken, id ?? "0")?.ToRequest();
                if (model == null)
                {
                    SessionInfo.Alert.SetAlert("El documento especificado no existe!", SessionInfo.AlertType.Error);
                    return RedirectToAction("Emitidos", "Comprobantes");
                }

                if (model.Status != DocumentStatusEnum.Draft && model.Status != DocumentStatusEnum.Error)
                {
                    SessionInfo.Alert.SetAlert("Este documento no puede ser modificado porque su estado no lo permite", SessionInfo.AlertType.Error);
                    return RedirectToAction("Ver", new { id });
                }

                return View(model);
            }

            SessionInfo.Alert.SetAlert("Renueva tu suscripción por un plan Pymes o Pro y podrás solicitar tu firma electrónica", SessionInfo.AlertType.Info);
            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult Nuevo()
        {
            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            {
                var model = new InvoiceRequestModel();
                return View("Editar", model);
            }
            SessionInfo.Alert.SetAlert("Renueva tu suscripción por un plan Pymes o Pro y podrás solicitar tu firma electrónica", SessionInfo.AlertType.Info);
            return RedirectToAction("Index", "Dashboard");
            
        }
        
        [HttpGet]
        public async Task<JsonResult> GetDocumentAuthSync(string docType, string docNumber)
        {
            if (!string.IsNullOrEmpty(docType) && !string.IsNullOrEmpty(docNumber))
            {
                var ruc = this.Issuer.RUC;
                var result = await ServicioComprobantes.ObtenerAutorizacionAsync(SecurityToken, ruc, docType, docNumber);
                
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult { Data = new DocumentStatusInfo(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public async System.Threading.Tasks.Task<JsonResult> GetContributorByIdAsync(long? id)
        {
            if (id.HasValue)
            {
                var data = await ServicioContribuyentes.ObtenerContribuyenteAsync(IssuerToken, id.Value);

                if (data != null)
                {
                    return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }

            return new JsonResult { Data = default(ContributorDto), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HandleJsonException]
        public async Task<ActionResult> EliminarAsync(string id, string reason = null)
        {
            var result = await ServicioDocumento.EliminarDocumentoAsync(IssuerToken, id, reason);

            if (result.IsSuccess)
            {
                ClearSession();
                var _actionResult = SessionInfo.AnulacionForm ?? "Borradores";
                SessionInfo.Alert.SetAlert("El Documento fue ANULADO correctamente!", SessionInfo.AlertType.Success);
                return RedirectToAction(_actionResult, "Comprobantes");
            }

            SessionInfo.Alert.SetAlert(result.UserMessage, Request.UrlReferrer.ToString(), SessionInfo.AlertType.Error);
            return Redirect(Request.UrlReferrer.ToString()); 
        }

        [HttpPost]
        [HandleJsonException]
        public async Task<ActionResult> EmitirAsync(string id)
        {
            var response = await ServicioDocumento.EmitirDocumentoAsync(IssuerToken, id, string.Empty);

            if (response.IsSuccess)
            {
                ClearSession();
                SessionInfo.Alert.SetAlert("El Documento fue emitido correctamente!", Url.Action("Ver", new { id }), SessionInfo.AlertType.Success);
            }
            else
            {                
                SessionInfo.Alert.SetAlert(response.UserMessage, Url.Action("Ver", new { id }), SessionInfo.AlertType.Error);
            }

            return RedirectToAction("Ver", new { id });
        }

        [HttpPost]
        [HandleJsonException]
        public async System.Threading.Tasks.Task<JsonResult> SaveInvoiceAsync(InvoiceRequestModel model)
        {
            var errors = GetValidationError();

            if (ModelState.IsValid && model != null)
            {
                model.AdditionalFields = model.AdditionalFields.ToAdditionalField();
                var response = await ServicioFactura.GuardarFacturaAsync(IssuerToken, model);
                var text = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    ClearSession();

                    var doc = JsonConvert.DeserializeObject<InvoiceModel>(text);
                    return new JsonResult
                    {
                        Data = new
                        {
                            id = doc.Id,
                            result = doc,
                            error = default(object),
                            status = response.StatusCode,
                            statusText = "Documento Guardado",
                            url = Url.Action("Ver", new { id = doc.Id })
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        ContentType = "application/json"
                    };
                }
                else
                {
                    errors = JsonConvert.DeserializeObject<System.Web.Http.HttpError>(text);
                    if (errors.ContainsKey("devMessage")){
                        if (errors["devMessage"].ToString().Contains(Constants.limitePlanMsj) || errors["devMessage"].ToString().Contains("Suscripción inactiva")) {
                            return new JsonResult(){
                                Data = new{
                                    id = -999,
                                    result = default(object),
                                    error = errors,
                                    status = response.StatusCode,
                                    statusText = $"Limite emisión de Documento.",
                                    url = Url.Action("limiteDocuments", "Dashboard")
                                },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                ContentType = "application/json"
                            };
                        }
                        else if (errors["devMessage"].ToString().Contains("El valor de la Factura supera los 50 dólares"))
                        {
                            return new JsonResult() {
                                Data = new {
                                    id = -1,
                                    result = default(object),
                                    error = errors,
                                    status = response.StatusCode,
                                    statusText = errors?.ExceptionMessage,
                                    url = Url.Action("limiteDocuments", "Dashboard")
                                },
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                ContentType = "application/json"
                            };
                        }                        
                    }
                }
            }

            return new JsonResult()
            {
                Data = new
                {
                    id = 0,
                    result = default(object),
                    error = errors,
                    status = HttpStatusCode.InternalServerError,
                    statusText = $"Error en el Documento. {errors?.ExceptionMessage}"
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentType = "application/json"
            };

        } 
        #endregion

        [HttpGet]
        public async Task<ActionResult> DownloadFile(string accessKey, string type)
        {
            var response = await ServicioComprobantes.DescargarComprobanteAsync(IssuerToken, accessKey, type);

            if (response.IsSuccessStatusCode)
            {
                return await Task.FromResult(File(await response.Content.ReadAsStreamAsync(), response.Content.Headers?.ContentType?.MediaType));
            }

            return await Task.FromResult(PrintDocumentById(accessKey, type));
        }

        public async Task<ActionResult> PDF(string id)
        {
            return await DownloadFile(id, "PDF");
        }

        public ActionResult PrintDocumentById(object id, string reportType)
        {
            var document = ServicioFactura.BuscarFacturaPorId(IssuerToken, id);

            if (string.IsNullOrEmpty(document?.AccessKey) || document?.AccessKey == "-")
            {
                document.AccessKey = "DOCUMENTO SIN VALOR TRIBUTARIO";
            } 

            return PrintDocument(document, reportType);
        }

        public ActionResult PrintDocument(InvoiceModel document, string reportType)
        {
 
            if (document != null)
            {
                InvoiceReport report = new InvoiceReport(Issuer, Server.MapPath("~/"));

                var result = report.Render(document, reportType);
                if (result != null)
                {

                    string filename;

                    if (!string.IsNullOrEmpty(document.AccessKey) && document.AccessKey != "-")
                    {
                        filename = $"FA{document.AccessKey}.{result.Extension}";
                    }
                    else if (!string.IsNullOrEmpty(document.AuthorizationNumber) && document.AuthorizationNumber != "-")
                    {
                        filename = $"FA{document.AuthorizationNumber}.{result.Extension}";
                    }
                    else
                    {
                        filename = $"FA{document.InvoiceInfo.IssuedOn.Replace("/","")}{Issuer.RUC}{document.DocumentTypeCode}{document.DocumentNumber.Replace("-", "")}.{result.Extension}";
                    }


                    return File(result.Content, result.MimeType);
                } 
            }
            return HttpNotFound("El documento no ha sido procesado.");

        } 
    }
}
