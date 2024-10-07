using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Filters;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.Reporting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Ecuafact.Web.Controllers
{    

    [ExpressAuthorize(Roles = "Issuer")]
    public class NotaDebitoController : DocumentControllerBase
    {
        protected override string SessionName => SessionInfo.NOTASDEBITO_SESSION;

        protected override string DocumentType => "05";

        protected override object DocumentById(string id)
        {
            DebitNoteModel model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = ServicioNotaDebito.BuscarNotaDebitoPorId(IssuerToken, id);
                model.Status = (model.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(model.AuthorizationDate)) ? DocumentStatusEnum.Authorized : model.Status;
            }

            return model;
        }
        #region Acciones del controlador

        //[HttpGet]
        //public ActionResult Index()
        //{
        //    DocumentosQueryModel model = null;

        //    try
        //    {
        //        if (Session[SessionName] != null)
        //        {
        //            model = (DocumentosQueryModel)Session[SessionName];
        //        }
        //    }
        //    catch { }

        //    // POR SEGURIDAD VERIFICA SI NO SE OBTUVO LA INFORMACION
        //    if (model == null)
        //    {
        //        model = new DocumentosQueryModel(DebitNoteType);
        //    }

        //    return View(model);
        //}
        //private const string DebitNoteType = "05";

        //[HttpPost]
        //public async Task<ActionResult> GetDocumentsAsync(DocumentosQueryModel model)
        //{
        //    model.DocumentType = DebitNoteType;
        //    model.Data = await ServicioDocumento.BuscarDocumentosAsync(IssuerToken, model);

        //    if (model.Data == null)
        //    {
        //        model.Data = new List<DocumentModel>();
        //    }

        //    Session[SessionName] = model;

        //    return PartialView("_DocumentosPartial", model);
        //}

        //public ActionResult Ver(string id)
        //{

        //    DebitNoteModel model = null;

        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        model = ServicioNotaDebito.BuscarNotaDebitoPorId(IssuerToken, id);
        //    }

        //    if (model == null)
        //    {
        //        var msg = "No existe la nota débito especificada!";
        //        SessionInfo.Alert.SetAlert(msg, SessionInfo.AlertType.Error);
        //        // Si no se encuentra la factura debe volver a la pagina de inicio
        //        return Index();
        //    }

        //    return View("Ver", model);
        //}

        public ActionResult Editar(string id = null)
        {
            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            {
                SessionInfo.Alert.SetAlert("Renueva tu suscripción por un plan Pymes o Pro y podrás solicitar tu firma electrónica", SessionInfo.AlertType.Info);
                return RedirectToAction("Index", "Dashboard");

                var model = ServicioNotaDebito.BuscarNotaDebitoPorId(IssuerToken, id ?? "0")?.ToRequest();

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
                var model = new DebitNoteRequestModel();
                return View("Editar", model);
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
                SessionInfo.Alert.SetAlert("El Documento fue anulado correctamente!", SessionInfo.AlertType.Success);
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
        public async System.Threading.Tasks.Task<JsonResult> SaveDebitNoteAsync(DebitNoteRequestModel model)
        {

            var errors = GetValidationError();

            if (ModelState.IsValid && model != null)
            {
                model.AdditionalFields = model.AdditionalFields.ToAdditionalField();
                var response = await ServicioNotaDebito.GuardarNotaDebitoAsync(IssuerToken, model);
                var text = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    ClearSession();

                    var doc = JsonConvert.DeserializeObject<DebitNoteModel>(text);
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
            var document = ServicioNotaDebito.BuscarNotaDebitoPorId(IssuerToken, id);

            if (string.IsNullOrEmpty(document.AccessKey) || document.AccessKey == "-")
            {
                document.AccessKey = "DOCUMENTO SIN VALOR TRIBUTARIO";
            }

            return PrintDocument(document, reportType);
        }

        public ActionResult PrintDocument(DebitNoteModel document, string reportType)
        {
            if (document != null)
            {
                DebitNoteReport report = new DebitNoteReport(Issuer, Server.MapPath("~/"));

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
                        filename = $"NC{document.DebitNoteInfo.IssuedOn.Replace("/", "")}{Issuer.RUC}{document.DocumentTypeCode}{document.DocumentNumber.Replace("-", "")}.{result.Extension}";
                    }


                    return File(result.Content, result.MimeType);
                }
            }
            return HttpNotFound("El documento no ha sido procesado.");

        }



    }
}
