
using Ecuafact.Web.MiddleCore.ApplicationServices;
using System.Collections.Generic;
using System.Web.Mvc;
using Ecuafact.Web.Domain.Entities;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using Ecuafact.Web.Reporting;
using System.Linq;
using System;
using Ecuafact.Web.Filters;
using Ecuafact.Web.Models;
using Ecuafact.Web.MiddleCore.NexusApiServices;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Xml.Serialization;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize]
    public class RetencionController : DocumentControllerBase
    {
        protected override string SessionName => SessionInfo.RETENCION_SESSION;

        #region Acciones del controlador

        protected override string DocumentType => "07";

        protected override object DocumentById(string id)
        {
            RetentionModel model = null;

            if (!string.IsNullOrEmpty(id))
            {
                model = ServicioRetencion.BuscarRetencionPorId(IssuerToken, id);
                model.Status = (model.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(model.AuthorizationDate)) ? DocumentStatusEnum.Authorized : model.Status;
            }

            return model;
        }    
        
        public ActionResult Nuevo()
        {
            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            {
                return View("Editar", new RetentionRequestModel());
            }            
            SessionInfo.Alert.SetAlert("Renueva tu suscripción por un plan Pymes o Pro y podrás solicitar tu firma electrónica", SessionInfo.AlertType.Info);
            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult Editar(string id = null)
        {

            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            {
                var model = ServicioRetencion.BuscarRetencionPorId(IssuerToken, id ?? "0")?.ToRequest();
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


        public JsonResult GetDocumentById(string id, string search = "", int page = 1)
        {
            var paramsDto = new ConsultaDocumentosParamsDto
            {
                FechaInicio = DateTime.Now.AddMonths(-3),
                FechaHasta = DateTime.Now.AddDays(1),
                Contenido = id,
                TipoDocumento = "01",
                TipoFecha = "1",
                NumeroPagina = page,
                CantidadPorPagina = 10
            };

            var _item = new {
                id = "-1",
                text = $"INGRESO MANUAL DEL DOCUMENTO SUSTENTO...",
                data = new Document { pk = "0", authorizationNumber = "-9999", codTypeDoc="00" }
            };

            var result = ServicioComprobantes.ObtenerComprobantesRecibidos(IssuerToken, paramsDto);
            if (result.documents != null && result.documents.Count > 0)
            {
                var data = result.documents.Where(model =>
                    string.IsNullOrEmpty(search) ||
                    model.authorizationNumber.Contains(search) ||
                    model.sequence.Contains(search) ||
                    model.date.Contains(search))
                    .Select(model => new
                    {
                        id = model.pk,
                        text = $"FACTURA # {model.sequence} | Fecha: {model.date} | Total: {model.total} | Autorizacion: {model.authorizationNumber}",
                        data = model
                    });

                var _data = data.ToList();
                _data.Insert(0, _item);
                return Json(_data, JsonRequestBehavior.AllowGet);
            }

            return Json(new[] { _item }, JsonRequestBehavior.AllowGet);
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
                SessionInfo.Alert.SetAlert(result.UserMessage, SessionInfo.AlertType.Success);
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

        [HttpGet]
        public async Task<JsonResult> SearchContribAsync(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
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

            return new JsonResult { Data = new List<ContributorDto>(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public async Task<JsonResult> SearchTaxAsync(long type, string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var result = await ServicioImpuestos.ObtenerImpuestosAsync(IssuerToken, type, search);

                // Para el Selector se debe usar una lista de esta forma: 
                // { id:1, text:"texto1" } entonces formatearemos la informacion relevante: 
                var data = result.ConvertAll(o =>
                {
                    return new
                    {
                        id = o.Id,
                        text = $"{o.SriCode.Trim()} {o.Name}",
                        data = o
                    };
                });

                return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            return new JsonResult { Data = new List<RetentionTax>(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public async Task<JsonResult> GetContributorByIdAsync(long? id)
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
        public async Task<JsonResult> SaveRetentionAsync(RetentionRequestModel model)
        {
            var errors = GetValidationError();

            if (ModelState.IsValid && model != null)
            {
                model.AdditionalFields = model.AdditionalFields.ToAdditionalField();
                model.PaymentResident = "01";
                model.Details?.ForEach(d => { d.ReferenceDocumentCode = model.ReferenceDocumentCode;
                    d.ReferenceDocumentDate = model.ReferenceDocumentDate;
                    d.ReferenceDocumentNumber = model.ReferenceDocumentNumber;                    
                    d.ReferenceDocumentAuth = model.ReferenceDocumentAuth;
                    d.ReferenceDocumentAmount = model.ReferenceDocumentAmount;
                    d.ReferenceDocumentTotal = model.ReferenceDocumentTotal;
                    d.SupportCode = model.SupportCode;
                    d.AccountingRegistrationDate = model.AccountingRegistrationDate;
                    d.PaymentResident = model.PaymentResident;
                });

                var response = await ServicioRetencion.GuardarRetencionAsync(IssuerToken, model);
                var text = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    ClearSession();                    
                    var doc = JsonConvert.DeserializeObject<RetentionModel>(text);
                    //SessionInfo.Notifications.Add($"Retención # {doc.DocumentNumber} ha sido guardada.", Url.Action("Ver", new { id = doc.Id }), SessionInfo.AlertType.Success, Request.GetDocumentIcon(RetentionType));                    
                    return new JsonResult
                    {
                        Data = new
                        {
                            id = doc.Id,
                            result = doc,
                            error = default(object),
                            status = response.StatusCode,
                            statusText = "Retencion Guardada",
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
                        if (errors["devMessage"].ToString().Contains(Constants.limitePlanMsj) || errors["devMessage"].ToString().Contains("Suscripción inactiva")) {
                            return new JsonResult(){
                                Data = new {
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

            SessionInfo.Notifications.Add($"Error al generar el Documento de Retencion ({model?.ContributorName})", type: SessionInfo.AlertType.Error, icon: Request.GetDocumentIcon(DocumentType));
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

        [HttpPost]
        public JsonResult ValidateDocumentSupportXml(DocumentSupportXml documentSupport)
        {
            try
            {
                if (documentSupport.XmlRaw == null || documentSupport.Name == null)
                {
                    throw new Exception("Debe seleccionar un documento sustento en formato xml valido");
                }
                var request = new UploadFileRequest{
                    Token = IssuerToken,
                    FileName = $"{Guid.NewGuid()}-{DateTime.Now.Millisecond}.xml",
                    File = Convert.ToBase64String(documentSupport.XmlRaw.GetBytes()),
                    client_token = IssuerToken
                };        

                var result = ServicioComprobantes.ObtenerComprobanteByXml(request);
                if(result != null)
                {
                    ReadXml(result, documentSupport.XmlRaw.GetBytes());
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }            
            catch (Exception ex)
            {
                return new JsonResult { Data = new DocumentRecividModel(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpGet]
        public JsonResult GetDocumentReceivedById(string id)
        {
            var document = ServicioComprobantes.ObtenerComprobanteById(IssuerToken, id);
            if (document != null )
            {               
                return Json(document, JsonRequestBehavior.AllowGet);
            }

            return Json(new[] { new { id = 0, text = "" } }, JsonRequestBehavior.AllowGet);
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
            var document = ServicioRetencion.BuscarRetencionPorId(IssuerToken, id);

            if (string.IsNullOrEmpty(document.AccessKey) || document.AccessKey == "-")
            {
                document.AccessKey = "DOCUMENTO SIN VALOR TRIBUTARIO";
            }

            return PrintDocument(document, reportType);
        }

        public ActionResult PrintDocument(RetentionModel document, string reportType)
        {
            if (document != null)
            {
                RetentionReport report = new RetentionReport(Issuer, Server.MapPath("~/"));

                var result = report.Render(document, reportType);
                if (result != null)
                {

                    string filename;

                    if (!string.IsNullOrEmpty(document.AccessKey) && document.AccessKey != "-")
                    {
                        filename = $"RE{document.AccessKey}.{result.Extension}";
                    }
                    else if (!string.IsNullOrEmpty(document.AuthorizationNumber) && document.AuthorizationNumber != "-")
                    {
                        filename = $"RE{document.AuthorizationNumber}.{result.Extension}";
                    }
                    else
                    {
                        filename = $"RE{document.RetentionInfo.IssuedOn.Replace("/","")}{Issuer.RUC}{document.DocumentTypeCode}{document.DocumentNumber.Replace("-", "")}.{result.Extension}";
                    }


                    return File(result.Content, result.MimeType);
                } 
            }
            return HttpNotFound("El documento no ha sido procesado.");

        }

        public DocumentRecividModel ReadXml(DocumentRecividModel recivid, byte[] file)
        {
            try
            {               
                string xml = System.Text.Encoding.UTF8.GetString(file);
                var serializer = new XmlSerializer(typeof(totalConImpuestos));
                totalConImpuestos _totalConImpuestos;
                xml = xml.Replace("<![CDATA[", "").Replace("]]>", "");
                xml = xml.Replace("&lt;![CDATA[", "").Replace("]]&gt;", "");
                xml = xml.Replace("&lt;", "<").Replace("&gt;", ">");
                int indiceTagComprobante = xml.IndexOf("<totalConImpuestos>");
                int indiceTagComprobanteCierre = -1;
                if (indiceTagComprobante >= 0)
                {
                    indiceTagComprobanteCierre = xml.IndexOf("</totalConImpuestos>");
                    if (indiceTagComprobanteCierre > indiceTagComprobante && indiceTagComprobanteCierre > 0 && indiceTagComprobanteCierre > 0)
                    {
                        string comprobanteXML = xml.Substring(indiceTagComprobante + "<totalConImpuestos>".Length, indiceTagComprobanteCierre - indiceTagComprobante - "</totalConImpuestos>".Length + 1);
                        xml = $"<totalConImpuestos>{comprobanteXML}</totalConImpuestos>";
                    }
                }

                using (var reader = new StringReader(xml))
                {                    
                    _totalConImpuestos = (totalConImpuestos)serializer.Deserialize(reader);
                }
                if(_totalConImpuestos != null)
                {
                   var ivaRates =  SessionInfo.Catalog.IVARates.OrderBy(ord => ord.RateValue);
                    recivid.TotalTax = new List<TotalTaxModel>();
                    _totalConImpuestos.totalImpuesto.ForEach(rate=> {

                        var _rate = ivaRates.Where(_code => _code.SriCode == rate.codigoPorcentaje).FirstOrDefault().RateValue;
                        recivid.TotalTax.Add(new TotalTaxModel { 
                            TaxCode = rate.codigo,
                            PercentageTaxCode = rate.codigoPorcentaje,
                            TaxableBase = rate.baseImponible,
                            TaxRate = _rate,
                            TaxValue = rate.valor

                        });

                    });
                }

                if(recivid.PaymentTypes.Count > 0)
                {
                    recivid.PaymentTypes.ForEach(term =>term.UnitTime.Replace('?', 'i'));
                }

                return recivid;

            }
            catch (Exception)
            {
                return recivid;
            }            
        }
    }
}
