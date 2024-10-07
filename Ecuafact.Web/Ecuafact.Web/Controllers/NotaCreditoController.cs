
using Ecuafact.Web.MiddleCore.ApplicationServices;
using System.Collections.Generic;
using System.Web.Mvc;
using Ecuafact.Web.Domain.Entities;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using Ecuafact.Web.Reporting;
using System.Globalization;
using Ecuafact.Web.Filters;
using Ecuafact.Web.Models;
using System;
using System.Linq;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize(Roles ="Issuer")]
    public class NotaCreditoController : DocumentControllerBase
    {
        protected override string SessionName => SessionInfo.NOTASCREDITO_SESSION;

        protected override string DocumentType => "04";

        protected override object DocumentById(string id)
        {
            CreditNoteModel model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = ServicioNotaCredito.BuscarNotaCreditoPorId(IssuerToken, id);
                model.Status = (model.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(model.AuthorizationDate)) ? DocumentStatusEnum.Authorized : model.Status;
            }

            return model;
        }

        #region Acciones del controlador

        public ActionResult Nuevo()
        {
            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            {
                //return View("Nuevo", new CreditNoteRequestModel());           
                return View("Editar", new CreditNoteRequestModel());
            }           

            SessionInfo.Alert.SetAlert("Renueva tu suscripción por un plan Pymes o Pro y podrás solicitar tu firma electrónica", SessionInfo.AlertType.Info);
            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult Editar(string id = null)
        {
            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            { 
                var model = ServicioNotaCredito.BuscarNotaCreditoPorId(IssuerToken, id ?? "0")?.ToRequest();
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

        [HttpGet]
        public async System.Threading.Tasks.Task<JsonResult> GetDocumentAuthSync(string docType, string docNumber)
        {
            if (!string.IsNullOrEmpty(docType) && !string.IsNullOrEmpty(docNumber))
            {
                var ruc = this.Issuer.RUC;
                var result = await ServicioComprobantes.ObtenerAutorizacionAsync(SecurityToken, ruc, docType, docNumber);
                
                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult { Data = new DocumentStatusInfo(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<JsonResult> SearchContribAsync(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return new JsonResult { Data = new List<ContributorDto>(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            var result = await ServicioContribuyentes.SearchContribuyentesAsync(IssuerToken, search);
             
            // Para el Selector se debe usar una lista de esta forma: 
            // { id:1, text:"texto1" } entonces formatearemos la informacion relevante: 
            var data = result.ConvertAll(o =>
            {
                return new
                {
                    id = o.Id,
                    text = o.Identification + " - " + o.BussinesName,
                    data = o
                };
            });

            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetDocumentById(long? id)
        {
            if (id.HasValue)
            {
                var result = ServicioFactura.BuscarFacturaPorId(IssuerToken, id.Value);

                return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult { Data = new InvoiceModel(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<JsonResult> SearchContribDocumentsAsync(long? id, string term)
        {
            if (id.HasValue)
            {
                var result = await ServicioFactura.BuscarFacturaContribAsync(IssuerToken, id.Value, term, true, true);

                // Para el Selector se debe usar una lista de esta forma: 
                // { id:1, text:"texto1" } entonces formatearemos la informacion relevante: 
                // Tambien debe filtrar solo los documentos que tengan estado>0 [Draft] o Eliminada.
                var data = result.ConvertAll(o => new
                {
                    id = (long?)o.Id,
                    text = $"{o.ContributorIdentification} {o.ContributorName} | FACTURA # {o.DocumentNumber} {o.IssuedOn.ToString("dd/MM/yyyy")} | {o.Total.ToString("c", new CultureInfo("en"))}",
                    selected = false,
                    data = o
                });

                return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult { Data = new List<InvoiceModel>(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public async Task<JsonResult> SearchProductAsync(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var result = await ServicioProductos
                    .ObtenerProductosAsync(IssuerToken, search);

                // Para el Selector se debe usar una lista de esta forma: 
                // { id:1, text:"texto1" } entonces formatearemos la informacion relevante: 
                var data = result.ConvertAll(o =>
                {
                    return new
                    {
                        id = o.Id,
                        text =
                        string.IsNullOrEmpty(o.AuxCode)
                        ? $"{o.MainCode.Trim()} {o.Name}" 
                        : $"{o.MainCode.Trim()}.{o.AuxCode.Trim()} {o.Name}",
                        data = o
                    };
                });

                return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult { Data = new List<ProductDto>(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

            SessionInfo.Alert.SetAlert(result.UserMessage, Url.Action("Ver", new { id }), SessionInfo.AlertType.Error);
            return RedirectToAction("Ver", new { id });
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
        public async System.Threading.Tasks.Task<JsonResult> SaveCreditNoteAsync(CreditNoteRequestModel model)
        {
            var errors = GetValidationError();

            if (model != null)
            {
                model.AdditionalFields = model.AdditionalFields.ToAdditionalField();
                var response = await ServicioNotaCredito.GuardarNotaCreditoAsync(IssuerToken, model);
                var text = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    ClearSession();
                    var doc = JsonConvert.DeserializeObject<CreditNoteModel>(text);
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
                    if (errors.ContainsKey("devMessage")) {
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
            var document = ServicioNotaCredito.BuscarNotaCreditoPorId(IssuerToken, id);

            if (string.IsNullOrEmpty(document.AccessKey) || document.AccessKey == "-")
            {
                document.AccessKey = "DOCUMENTO SIN VALOR TRIBUTARIO";
            }

            return PrintDocument(document, reportType);
        }

        public ActionResult PrintDocument(CreditNoteModel document, string reportType)
        {
            if (document != null)
            {
                CreditNoteReport report = new CreditNoteReport(Issuer, Server.MapPath("~/"));

                var result = report.Render(document, reportType);
                if (result != null)
                {

                    string filename;

                    if (!string.IsNullOrEmpty(document.AccessKey) && document.AccessKey != "-")
                    {
                        filename = $"NC{document.AccessKey}.{result.Extension}";
                    }
                    else if (!string.IsNullOrEmpty(document.AuthorizationNumber) && document.AuthorizationNumber != "-")
                    {
                        filename = $"NC{document.AuthorizationNumber}.{result.Extension}";
                    }
                    else
                    {
                        filename = $"NC{document.CreditNoteInfo.IssuedOn.Replace("/","")}{Issuer.RUC}{document.DocumentTypeCode}{document.DocumentNumber.Replace("-", "")}.{result.Extension}";
                    }


                    return File(result.Content, result.MimeType);
                } 
            }
            return HttpNotFound("El documento no ha sido procesado.");

        }

         

    }
}
