
using Ecuafact.Web.MiddleCore.ApplicationServices;
using System.Collections.Generic;
using System.Web.Mvc;
using Ecuafact.Web.Domain.Entities;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using Ecuafact.Web.Reporting;
using Ecuafact.Web.Filters;
using Ecuafact.Web.Models;
using System;
using System.Linq;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize]
    public class GuiaRemisionController : DocumentControllerBase
    {
        protected override string SessionName => SessionInfo.GUIAREMISION_SESSION;

        #region Acciones del controlador

        protected override string DocumentType => "06";

        protected override object DocumentById(string id)
        {
            ReferralGuideModel model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = ServicioGuiaRemision.BuscarGuiaRemisionPorId(IssuerToken, id);
                model.Status = (model.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(model.AuthorizationDate)) ? DocumentStatusEnum.Authorized : model.Status;
            }

            return model;
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
         
        public ActionResult Nuevo()
        {
            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            {
                return View("Editar", new ReferralGuideRequestModel());
            }          

            SessionInfo.Alert.SetAlert("Renueva tu suscripción por un plan Pymes o Pro y podrás solicitar tu firma electrónica", SessionInfo.AlertType.Info);
            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult Editar(string id = null)
        {
            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            {
                var model = ServicioGuiaRemision.BuscarGuiaRemisionPorId(IssuerToken, id ?? "0")?.ToRequest();
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

                return View("Editar", model);
            }
            SessionInfo.Alert.SetAlert("Renueva tu suscripción por un plan Pymes o Pro y podrás solicitar tu firma electrónica", SessionInfo.AlertType.Info);
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public async  Task<JsonResult> GetDocumentAuthSync(string docType, string docNumber)
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
        public async Task<JsonResult> SearchDocumentsAsync(string search, string _type, int page = 1)
        {
            try
            {
                var result = await ServicioFactura.SearchInvoicesAsync(IssuerToken, search, _type, page);               
                if (page == 1)
                {
                    var invoiceModel = new InvoiceModel { Id = -1, DocumentTypeCode = "-1" };
                    var item = new Select2ListItem<InvoiceModel>(invoiceModel) { id = invoiceModel.Id, text = "Sin Documento Sustento" };
                    result.results.Insert(0, item);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { ex.ToString(); }

            return Json(new Select2List<InvoiceModel>(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async  Task<JsonResult> SearchContribAsync(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var result = await ServicioContribuyentes
                    .SearchContribuyentesAsync(IssuerToken, search);

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

            return new JsonResult { Data = new List<ContributorDto>(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

        public async  Task<JsonResult> GetContributorByIdAsync(long? id)
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
        public async Task<JsonResult> SaveReferralGuideAsync(ReferralGuideRequestModel model)
        {            
            var errors = GetValidationError();
            if (model != null)
            {
                model.AdditionalFields = model.AdditionalFields.ToAdditionalField();
                var response = await ServicioGuiaRemision.GuardarGuiaRemisionAsync(IssuerToken, model);
                var text = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    ClearSession();                    
                    var doc = JsonConvert.DeserializeObject<ReferralGuideModel>(text);
                    //SessionInfo.Notifications.Add($"Se ha guardado la Guia de Remisión # {doc.DocumentNumber}!", Url.Action("Ver", new { id = doc.Id }), 
                    //    SessionInfo.AlertType.Success, Request.GetDocumentIcon(DocumentType));
                    return new JsonResult
                    {
                        Data = new
                        {
                            id = doc.Id,
                            result = doc,
                            error = default(object),
                            status = response.StatusCode,
                            statusText = "Documento Guardado",
                            Url = Url.Action("Ver", new { id = doc.Id })
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        ContentType = "application/json"
                    };
                }
                else
                {
                    errors = JsonConvert.DeserializeObject<System.Web.Http.HttpError>(text);
                    if (errors.ContainsKey("devMessage")){
                        if (errors["devMessage"].ToString().Contains(Constants.limitePlanMsj) || errors["devMessage"].ToString().Contains("Suscripción inactiva")){
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
            SessionInfo.Notifications.Add($"No se pudo guardar el documento para {model.ContributorName}!", "", SessionInfo.AlertType.Success, Request.GetDocumentIcon(DocumentType));
            return new JsonResult()
            {
                Data = new
                {
                    id = 0,
                    result = default(object),
                    error = errors,
                    status = HttpStatusCode.InternalServerError,
                    statusText = "Error en el Documento"
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
            var document = ServicioGuiaRemision.BuscarGuiaRemisionPorId(IssuerToken, id);

            if (string.IsNullOrEmpty(document.AccessKey) || document.AccessKey == "-")
            {
                document.AccessKey = "DOCUMENTO SIN VALOR TRIBUTARIO";
            }

            return PrintDocument(document, reportType);
        }

        public ActionResult PrintDocument(ReferralGuideModel document, string reportType)
        {
            if (document != null)
            {
                ReferralGuideReport report = new ReferralGuideReport(Issuer, Server.MapPath("~/"));

                var result = report.Render(document, reportType);
                if (result != null)
                {

                    string filename;

                    if (!string.IsNullOrEmpty(document.AccessKey) && document.AccessKey != "-")
                    {
                        filename = $"GR{document.AccessKey}.{result.Extension}";
                    }
                    else if (!string.IsNullOrEmpty(document.AuthorizationNumber) && document.AuthorizationNumber != "-")
                    {
                        filename = $"GR{document.AuthorizationNumber}.{result.Extension}";
                    }
                    else
                    {
                        filename = $"GR{document.ReferralGuideInfo.IssuedOn.Replace("/", "")}{Issuer.RUC}{document.DocumentTypeCode}{document.DocumentNumber.Replace("-", "")}.{result.Extension}";
                    }


                    return File(result.Content, result.MimeType);
                }
            }
            return HttpNotFound("El documento no ha sido procesado.");

        }

    }
}
