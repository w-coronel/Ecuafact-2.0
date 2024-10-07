using System;
using System.Web.Mvc;
using System.Net.Http;
using Ecuafact.Web.Models;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Filters;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Ecuafact.Web.MiddleCore.NexusApiServices;
using Ecuafact.Web.Reporting;
using Ecuafact.Web.Domain.Services;
using Newtonsoft.Json;
using System.Net;
using System.Xml.Serialization;
using System.IO;
using System.Data;

namespace Ecuafact.Web.Controllers
{
    /// <summary>
    /// Este es un servicio externo desde EcuaFactApp
    /// </summary>
    [ExpressAuthorize()]
    public class ComprobantesController : AppControllerBase
    {
        public async Task<ActionResult> Index(string id = null, string type = null)
        { 
            return await Recibidos(id, type);
        }

        public async Task<ActionResult> Borradores(string id = null, string type = null)
        {
            if (id != null)
            {
                if (id.ToLower() == "descargar")
                {
                    return await DescargarBorradores();
                }
                else
                {
                    if (type == null)
                    {
                        return await VerEmitidos(id);
                    }
                    else
                    {
                        return await DescargarEmitidos(id, type);
                    }
                }
            }

            return await ListaBorradores();
        }

        [HttpPost]
        [HandleJsonException]
        public async Task<ActionResult> EliminarAsync(string id, string reason = null)
        {
            var result = await ServicioDocumento.EliminarDocumentoAsync(IssuerToken, id, reason);

            if (result.IsSuccess)
            {
                ClearSession();
                SessionInfo.Alert.SetAlert("El Documento fue ANULADO correctamente!", SessionInfo.AlertType.Success);
                return RedirectToAction("Borradores", "Comprobantes");
            }
            
            var url = Request.UrlReferrer;

            SessionInfo.Alert.SetAlert(result.UserMessage, Url.Action("Ver", new { id }), SessionInfo.AlertType.Error);
            return RedirectToAction("Ver", new { id });
        }

        private async Task<ActionResult> ListaBorradores()
        {
            DocumentosQueryModel model = null;
            //try
            //{
            //    if (Session[SessionInfo.BORRADORES_SESSION] != null)
            //    {
            //        model = (DocumentosQueryModel)Session[SessionInfo.BORRADORES_SESSION];
            //    }
            //}
            //catch { }

            // POR SEGURIDAD VERIFICA SI NO SE OBTUVO LA INFORMACION
            if (model == null)
            {
                model = new DocumentosQueryModel();
            }

            return await Task.FromResult(View("Borradores", model));
        }
        
        [HttpPost]
        public async Task<ActionResult> GetBorradores(DocumentosQueryModel model)
        {
            model.Status = DocumentStatusEnum.Draft;
            model.Data = await ServicioDocumento.BuscarDocumentosAsync(IssuerToken, model) ?? new List<DocumentModel>();             
            Session[SessionInfo.BORRADORES_SESSION] = model;           
            SessionInfo.AnulacionForm = "Borradores";
            return PartialView("Shared/_BorradoresPartial", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="draw">Pagina</param>
        /// <param name="start">Inicio</param>
        /// <param name="length">Cantidad de registros x pagina</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetBorradoresAsync(DocumentosQueryModel model)
        {
            int draw = Convert.ToInt32(Request["draw"] ?? "1");
            int start = Convert.ToInt32(Request["start"] ?? "1");
            int length = Convert.ToInt32(Request["length"] ?? "10");
            string search = Request["search[value]"];
            long columna = Convert.ToInt32(Request["order[0][column]"]);
            string order = Request["order[0][dir]"];
            model.SearchTerm = model.SearchTerm ?? search;
            model.Status = DocumentStatusEnum.Draft;

            var page = 1;
            if (start > 0 && length > 0)
            {
                page = Convert.ToInt32(start / length) + 1;
            }
            var issuePCode = string.Empty;
            var estabCode = string.Empty;
            //validar si el emisor tiene rol de coperativa de transporte para obtener los documentos del establecimiento y punto de emision del usuario que tiene relacionado la cooperativa
            if (SessionInfo.UserRole == SecuritySessionRole.Cooperative)
            {
                SessionInfo.Issuer.Establishments.ForEach(s => {
                    var temp = s.IssuePoint.Where(d => d.CarrierRUC == SessionInfo.LoginInfo.UserInfo.Username)?.FirstOrDefault() ?? null;
                    if (temp != null)
                    {
                        issuePCode = temp.Code;
                        estabCode = SessionInfo.Issuer.Establishments.Where(e => e.Id == temp.EstablishmentsId)?.FirstOrDefault()?.Code;
                    }
                });
            }
            var document = await ServicioDocumento.ObtenerDocumentosPagedAsync(IssuerToken, model.SearchTerm, model.From.ToDateTime(), model.To.ToDateTime(), page, length, documentType: model.DocumentType, status: model.Status, establishmentCode: estabCode, issuePointCode: issuePCode);
            var documentType = SessionInfo.Catalog.DocumentTypes.ToList();

            Session[SessionInfo.BORRADORES_SESSION] = new DocumentosQueryModel() { Data = document.data };

            if (document.length > 0)
            {
                document.data.ToList().ForEach(d => {
                    d.DocumentName = documentType.Find(dt => dt.SriCode == d.DocumentTypeCode).Name;
                    d.Controller = Url.Action("Ver", Request.GetController(d.DocumentTypeCode), new { id = d.Id });
                    d.IconClass = Request.GetStatusIcon(d.Status.Value);
                    d.IconType = Request.GetDocumentIcon(d.DocumentTypeCode);
                    d.StyleClass = d.Status.Value.GetValorCore() == "100" ? (d.AuthorizationDate == null ? DocumentStatusEnum.Authorized.GetStyle() : d.Status.Value.GetStyle()) : (d.Status.Value.GetStyle() ?? "brand");
                    d.StatusMessage = d.Status.Value.GetValorCore() == "100" ? (d.AuthorizationDate == null ? DocumentStatusEnum.Authorized.GetDisplayValue() : d.Status.Value.GetDisplayValue()) : (d.Status.Value.GetDisplayValue() ?? "Procesando");
                    d.StatusType = Request.GetStatusType(d.Status.Value);
                    d.StatusMsg = d.StatusMsg ?? $"El documento actual se encuentra en el estado: {d.StatusMessage}";
                    d.StyleColor = Request.GetColor(d.DocumentTypeCode);
                    d.PdfUrl = Url.Action("PDF", Request.GetController(d.DocumentTypeCode), new { id = d.Id, type = "PDF"});
                    if (d.RetentionInfo != null) { d.Total = d.RetentionInfo.FiscalAmount; }
                });
            }

            // Solo es una validacion del request [es un ID request] 
            // debe ser el mismo que el requerido.
            document.draw = draw;
            return Json(document, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Emitidos(string id = null, string type = null)
        {
            if (id != null)
            {
                if (id.ToLower() == "descargar")
                {
                    return await DescargarEmitidos();
                }
                else
                {
                    if (type == null)
                    {
                        return await VerEmitidos(id);
                    }
                    else
                    {
                        return await DescargarEmitidos(id, type);
                    }
                }
            }
            SessionInfo.AnulacionForm = "Emitidos";
            return await ListaEmitidos();
        }
         
        [HttpPost]
        public async Task<ActionResult> GetEmitidos(DocumentosQueryModel model)
        {
            model.Data = await ServicioDocumento.BuscarDocumentosAsync(IssuerToken, model.SearchTerm, model.From.ToDateTime(), model.To.ToDateTime(), documentType: model.DocumentType, status: model.Status) ?? new List<DocumentModel>();
             
            Session[SessionInfo.EMITIDOS_SESSION] = model;

            return PartialView("Shared/_EmitidosPartial", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="draw">Pagina</param>
        /// <param name="start">Inicio</param>
        /// <param name="length">Cantidad de registros x pagina</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetEmitidosAsync(DocumentosQueryModel model)
        {
            int draw = Convert.ToInt32(Request["draw"] ?? "1");
            int start = Convert.ToInt32(Request["start"] ?? "1");
            int length = Convert.ToInt32(Request["length"] ?? "10");
            string search = Request["search[value]"];
            long columna = Convert.ToInt32(Request["order[0][column]"]);
            string order = Request["order[0][dir]"];
            model.SearchTerm = model.SearchTerm ?? search;

            var page = 1;
            if (start > 0 && length > 0){
                page = Convert.ToInt32(start / length) + 1;
            }

            var issuePCode = model.IssuePointCode;
            var estabCode = model.EstablishmentCode;
            //validar si el emisor tiene rol de coperativa de transporte para obtener los documentos del establecimiento y punto de emision del usuario que tiene relacionado la cooperativa
            if (SessionInfo.UserRole == SecuritySessionRole.Cooperative) {
                SessionInfo.Issuer.Establishments.ForEach(s => {
                    var temp = s.IssuePoint.Where(d => d.CarrierRUC == SessionInfo.LoginInfo.UserInfo.Username)?.FirstOrDefault() ?? null;
                    if (temp != null){
                        issuePCode = temp.Code;
                        estabCode = SessionInfo.Issuer.Establishments.Where(e => e.Id == temp.EstablishmentsId)?.FirstOrDefault()?.Code;
                    }
                });
            }
            var document = await ServicioDocumento.ObtenerDocumentosPagedAsync(IssuerToken, model.SearchTerm, model.From.ToDateTime(), model.To.ToDateTime(), page, length, documentType: model.DocumentType, status: model.Status, establishmentCode: estabCode, issuePointCode: issuePCode, order == "desc", columna);            
            var documentType = SessionInfo.Catalog.DocumentTypes.ToList();
            Session[SessionInfo.EMITIDOS_SESSION] = new DocumentosQueryModel() { Data = document.data };            

            if (document.length > 0)
            {
                document.data.ToList().ForEach(d =>{
                    d.DocumentName = documentType.Find(dt => dt.SriCode == d.DocumentTypeCode).Name;
                    d.Controller = Url.Action("Ver", Request.GetController(d.DocumentTypeCode), new { id = d.Id });
                    d.Status = (d.Status == DocumentStatusEnum.Validated && string.IsNullOrWhiteSpace(d.AuthorizationDate)) ? DocumentStatusEnum.Authorized : d.Status;
                    d.IconClass = Request.GetStatusIcon(d.Status.Value);
                    d.IconType = Request.GetDocumentIcon(d.DocumentTypeCode);
                    d.StyleClass = d.Status.Value.GetValorCore() == "100" ? (d.AuthorizationDate == null ? DocumentStatusEnum.Authorized.GetStyle() : d.Status.Value.GetStyle()) : (d.Status.Value.GetStyle() ?? "brand");
                    d.StatusMessage = d.Status.Value.GetValorCore() == "100" ? (d.AuthorizationDate == null ? DocumentStatusEnum.Authorized.GetDisplayValue() : d.Status.Value.GetDisplayValue()) : (d.Status.Value.GetDisplayValue() ?? "Procesando");
                    d.StatusType = Request.GetStatusType(d.Status.Value);
                    d.StatusMsg = (d.Status == DocumentStatusEnum.Authorized) ? DocumentStatusEnum.Authorized.GetDisplayValue() : d.StatusMsg ?? $"El documento actual se encuentra en el estado: {d.StatusMessage}";
                    if (d.DocumentTypeCode == "06")
                    {
                        if (string.IsNullOrWhiteSpace(d.ContributorName) && string.IsNullOrWhiteSpace(d.ReferralGuideInfo.ReferenceDocumentAuth))
                        {
                            d.ContributorIdentification = d.ReferralGuideInfo.RecipientIdentification;
                            d.ContributorName = d.ReferralGuideInfo.RecipientName;
                        }
                    }
                    else if (d.DocumentTypeCode == "07")
                    {
                        if (d.RetentionInfo != null)
                        {
                            d.Total = d.RetentionInfo.FiscalAmount;
                        }
                    }
                });
            }

            // Solo es una validacion del request [es un ID request] 
            // debe ser el mismo que el requerido.
            document.draw = draw;            
            return Json(document, JsonRequestBehavior.AllowGet);
        }

        private async Task<ActionResult> ListaEmitidos()
        {
            DocumentosQueryModel model = null;

            //try
            //{
            //    if (Session[SessionInfo.EMITIDOS_SESSION] != null)
            //    {
            //        model = (DocumentosQueryModel)Session[SessionInfo.EMITIDOS_SESSION];
            //    }
            //}
            //catch { }

            // POR SEGURIDAD VERIFICA SI NO SE OBTUVO LA INFORMACION
            if (model == null)
            {
                model = new DocumentosQueryModel();
            }

            return await Task.FromResult(View("Emitidos", model));
        }

        private async Task<ActionResult> VerEmitidos(string id)
        {
            return await Task.FromResult(View());
        }

        private async Task<ActionResult> DescargarEmitidos()
        {
            var emitidos = Session[SessionInfo.EMITIDOS_SESSION] as DocumentosQueryModel;
             
            var report = emitidos.ToExcel();

            return await Task.FromResult(File(report, "application/excel", $"documentos_emitidos_{DateTime.Now.ToFileTime()}.xlsx"));
        }

        public async Task<ActionResult> Descargar(string filtro, DateTime? startDate = null, DateTime? endDate = null,  string docType = "", DocumentStatusEnum? status = null)
        {
            // Verificamos si es recibido o emitido:
            var result = await ServicioDocumento.DescargarDocumentosAsync(IssuerToken, filtro, startDate, endDate, docType, status);

            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStreamAsync();

                return File(content, result.Content.Headers.ContentType.ToString(), result.Content.Headers.ContentDisposition.FileName);
            }

            return HttpNotFound(result.ReasonPhrase);
        }

        private async Task<ActionResult> DescargarEmitidos(string id, string type)
        {
            type = type ?? "PDF";

            if (!string.IsNullOrEmpty(id) || id == "-")
            {
                // Verificamos si es recibido o emitido:
                var result = await ServicioComprobantes.DescargarComprobanteAsync(IssuerToken, id, type);

                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStreamAsync();

                    return File(content, result.Content.Headers.ContentType.ToString());
                }

                return HttpNotFound(result.ReasonPhrase);
            }

            return HttpNotFound("El documento no existe!");
        }

        public async Task<ActionResult> Recibidos(string id = null, string type = null)
        {
            if (id != null)
            {
                if (id.ToLower() == "descargar")
                {
                    return await DescargarRecibidos();
                }
                else
                {
                    if (type == null)
                    {
                        return await VerRecibidos(id);
                    }
                    else
                    {
                        return await DescargarRecibidos(id, type);
                    }
                }
            }

            return await ListaRecibidos();
        }

        private async Task<ActionResult> VerRecibidos(string id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetRecibidos(RecibidosQueryModel model)
        {
            var paramsDto = new ConsultaDocumentosParamsDto
            {
                FechaInicio = model.From.ToDateTime(),
                FechaHasta = model.To.ToDateTime(),
                Contenido = model.SearchTerm,
                TipoDocumento = model.DocumentType,
                TipoFecha = model.QueryType,
                Deducible = model.DeductibleType
            };
            model.Data = ServicioComprobantes.ObtenerComprobantesRecibidos(IssuerToken, paramsDto);
            Session[SessionInfo.RECIBIDOS_SESSION] = model;
            return PartialView("Shared/_RecibidosPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> GetRecibidosAsync(RecibidosQueryModel model)
        {
            int draw = Convert.ToInt32(Request["draw"] ?? "1");
            int start = Convert.ToInt32(Request["start"] ?? "1");
            int length = Convert.ToInt32(Request["length"] ?? "10");
            string search = Request["search[value]"];
            long columna = Convert.ToInt32(Request["order[0][column]"]);
            string order = Request["order[0][dir]"];
            model.SearchTerm = model.SearchTerm ?? search;

            var page = 1;
            if (start > 0 && length > 0)
            {
                page = Convert.ToInt32(start / length) + 1;
            }

            var paramsDto = new ConsultaDocumentosParamsDto
            {
                FechaInicio = model.From.ToDateTime(),
                FechaHasta = model.To.ToDateTime(),
                Contenido = model.SearchTerm,
                TipoDocumento = model.DocumentType,
                TipoFecha = model.QueryType,
                Deducible = model.DeductibleType,
                NumeroPagina = page,
                CantidadPorPagina = length
            };
            var documentReceived = await ServicioComprobantes.ObtenerComprobantesRecibidosPagedAsync(IssuerToken, paramsDto);
            documentReceived.draw = draw;
            return Json(documentReceived, JsonRequestBehavior.AllowGet);
        }

        private async Task<ActionResult> ListaRecibidos()
        {
            RecibidosQueryModel model = null;
            try
            {
                if (HttpContext.Cache[SessionInfo.RECIBIDOS_SESSION] != null)
                {
                    model = (RecibidosQueryModel)Session[SessionInfo.RECIBIDOS_SESSION];
                }
            }
            catch { }

            // POR SEGURIDAD VERIFICA SI NO SE OBTUVO LA INFORMACION
            if (model == null)
            {
                model = new RecibidosQueryModel();
            }

            return View("Recibidos", model);
        }

        public async Task<ActionResult> DescargarRecibidosII(string filtro, DateTime? startDate = null, DateTime? endDate = null, string docType = "", string deductible = "")
        {
            // Verificamos si es recibido o emitido:
            var result = await ServicioComprobantes.DescargarRecibidosAsync(IssuerToken, filtro, startDate, endDate, docType, deductible);

            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStreamAsync();
                return File(content, result.Content.Headers.ContentType.ToString(), result.Content.Headers.ContentDisposition.FileName);
            }
           
            return await Task.FromResult(File(new DocumentResponse().ToExcel(), "application/excel", $"documentos_recibidos_{DateTime.Now.ToFileTime()}.xlsx"));
        }

        private async Task<ActionResult> DescargarRecibidos()
        {
            var recibidos = Session[SessionInfo.RECIBIDOS_SESSION] as RecibidosQueryModel;

            var report = (recibidos?.Data ?? new DocumentResponse()).ToExcel();

            return await Task.FromResult(File(report, "application/excel", $"documentos_recibidos_{DateTime.Now.ToFileTime()}.xlsx"));
        }

        private async Task<ActionResult> DescargarRecibidos(string id, string type)
        {
            type = type ?? "PDF";         

            if (!string.IsNullOrEmpty(id) || id == "-")
            {
                var document = await ServicioComprobantes.ObtenerComprobanteRecibidoById(IssuerToken, id);
                if (document == null)
                {
                    var documentUrl = ServicioComprobantes.ObtenerUrlRecibidos(id, type);

                    if (!string.IsNullOrEmpty(documentUrl))
                    {
                        using (var client = new HttpClient())
                        {
                            var response = await client.GetAsync(documentUrl);

                            if (response.IsSuccessStatusCode)
                            {
                                var content = response.Content;
                                var fileData = content.ReadAsByteArrayAsync().Result;
                                return await Task.FromResult(File(fileData, content.Headers.ContentType.ToString(), GetFileName(id, type)));
                            }

                        }
                    }
                }
                else {
                    if (document.codTypeDoc == "02")
                    {
                        var result = await ServicioComprobantes.DescargarNotaVentaAsync(IssuerToken, id);
                        if (result.IsSuccessStatusCode)
                        {
                            var content = await result.Content.ReadAsStreamAsync();
                            return File(content, result.Content.Headers.ContentType.ToString());
                        }
                    }
                }
            }

            return HttpNotFound("El documento no existe!");
        }

        private static string GetFileName(string name, string type)
        {
            return $"{name}.{type?.ToLower() ?? "pdf"}";
        }

        private async Task<ActionResult> DescargarBorradores()
        {
            var borradores = Session[SessionInfo.BORRADORES_SESSION] as DocumentosQueryModel;

            var report = borradores.ToExcel();

            return await Task.FromResult(File(report, "application/excel", $"documentos_borradores_{DateTime.Now.ToFileTime()}.xlsx"));
        }

        [HttpGet]
        public ActionResult  ImportarData()
        {
            return PartialView("Shared/_ImportarDataPartial");
        }

        [HttpGet]
        public ActionResult ClasificarDocumento(string id)
        {
            var document = SessionInfo.DocumentsReceived?.FirstOrDefault(m => m.pk == id);
            

            return PartialView("_Clasificar", document);
        }   
        [HttpPost]
        public ActionResult Clasificar(long id, string deducible)
        {
            SetDocumentsDeductiblesRequest request =
                new SetDocumentsDeductiblesRequest
                {
                    client_token = SecurityToken,
                    deductible = deducible,
                    items_exceptions = new List<DetalleDeducible>()
                };

            var result = ServicioComprobantes.ClasificarGastosDocumento(IssuerToken, id, request);

            if (result != null)
            {
                ClearSession();
            }

            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> TipoSustentoDocumento(string id)
        {
            var document = await ServicioComprobantes.ObtenerComprobanteRecibidoById(IssuerToken, id);
            return PartialView("_TipoSustento", document);
        }

        [HttpPost]
        public async Task<ActionResult> Sustento(long id, string sustento)
        {
            var response = await ServicioComprobantes.TipoSustentoDocumento(IssuerToken, id, sustento).ConfigureAwait(false);
            return Json(new { response }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Preliminar(string id)
        {
            //var document = SessionInfo.DocumentsReceived?.FirstOrDefault(m => m.pk == id);
            var document = ServicioComprobantes.ObtenerComprobanteById(IssuerToken, id); 
            return PartialView("Shared/_DocumentoRecibido", document);
        }

        [HttpPost]
        public async Task<ActionResult> SendMail(string id, SendMailRequest request)
        {
            if (id.ToLower() == "emitidos")
            {
                OperationResult result = await ServicioDocumento.EnviarCorreoAsync(IssuerToken, request.email, request.uid);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else if(id.ToLower() == "recibidos")
            {
                OperationResult result = await ServicioComprobantes.EnviarCorreoAsync(IssuerToken, request.email, request.uid);

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(new OperationResult(false, System.Net.HttpStatusCode.BadRequest, $"No se envió el correo electrónico {id}"));
        }

        [HttpPost]
        [HandleJsonException]
        public async Task<JsonResult> ImportarDataAsync(DocumentSupportXml model)
        {
            var errors = "Los datos enviados son inválidos!";
            try
            {
                HttpResponseMessage response = null;
                if (model.XmlRaw == null || model.Name == null)
                {
                    throw new Exception("Debe seleccionar un documento sustento en formato xml valido");
                }

                if(!ReadXml(model.XmlRaw.GetBytes(), out string errormsg))
                {
                    return new JsonResult
                    {
                        Data = new
                        {
                            result = false,
                            error = errormsg,
                            status = HttpStatusCode.InternalServerError,
                            message = errormsg
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        ContentType = "application/json"
                    };
                }

                var request = new UploadFileRequest
                {
                    Token = IssuerToken,
                    FileName = $"{Guid.NewGuid()}-{DateTime.Now.Millisecond}.xml",
                    File = Convert.ToBase64String(model.XmlRaw.GetBytes()),
                    client_token = IssuerToken
                };
                var result = ServicioComprobantes.ObtenerComprobanteByXml(request);
                return new JsonResult
                {
                    Data = new
                    {
                        id = result.Pk,
                        result = true,
                        error = "",
                        status = HttpStatusCode.OK,
                        message = "El documento recibido se registro con éxito"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentType = "application/json"
                };
               
            }
            catch (Exception ex)
            {
                errors = ex.Message;
            }
            return new JsonResult
            {
                Data = new
                {
                    result = false,
                    error = errors,
                    status = HttpStatusCode.InternalServerError,
                    message = errors
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentType = "application/json"
            };

        }

        [HttpGet]
        [HandleJsonException]
        public  JsonResult SincronizarAsync()
        {
            var errors = "Error al Sincronizar los documentos recibidos";
            try
            {
                var _id = ServicioComprobantes.SincronizarRecibidos(IssuerToken);
                if(_id > 0)
                {
                    return new JsonResult
                    {
                        Data = new
                        {
                            id = _id,
                            result = true,
                            error = "",
                            status = HttpStatusCode.OK,
                            message = "La sincronización de sus documentos recibidos ha iniciado, en las proximas 24 a 48 horas se sincronizarán los documentos."
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        ContentType = "application/json"
                    };
                }
            }
            catch (Exception ex)
            {
                errors = ex.Message;
            }
            return new JsonResult
            {
                Data = new
                {
                    id= 0,
                    result = false,
                    error = errors,
                    status = HttpStatusCode.InternalServerError,
                    message = "Error al sincronizar los documentos recibidos"
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentType = "application/json"
            };

        }

        [HttpPost]
        public async Task<ActionResult> CancelDocument(int id)
        {
            if (id > 0)
            {
                var result = await ServicioComprobantes.AnularDocumentoRecibidoAsync(IssuerToken, id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(new OperationResult(false, System.Net.HttpStatusCode.BadRequest, $"No se anulo el documento {id}"));
        }

        public bool ReadXml(byte[] file, out string errormsg)
        {
            errormsg = "";
            try
            {
                string xml = System.Text.Encoding.UTF8.GetString(file);
                string xml2 = xml;
                var serializer = new XmlSerializer(typeof(totalConImpuestos));
                infoTributaria _infoTributaria;  
                xml = xml.Replace("<![CDATA[", "").Replace("]]>", "");
                xml = xml.Replace("&lt;![CDATA[", "").Replace("]]&gt;", "");
                int indiceTagComprobante = xml.IndexOf("<infoTributaria>");
                int indiceTagComprobanteCierre = -1;
                if (indiceTagComprobante >= 0)
                {
                    indiceTagComprobanteCierre = xml.IndexOf("</infoTributaria>");
                    if (indiceTagComprobanteCierre > indiceTagComprobante && indiceTagComprobanteCierre > 0 && indiceTagComprobanteCierre > 0)
                    {
                        string comprobanteXML = xml.Substring(indiceTagComprobante + "<infoTributaria>".Length, indiceTagComprobanteCierre - indiceTagComprobante - "</infoTributaria>".Length + 1);
                        xml = $"<infoTributaria>{comprobanteXML}</infoTributaria>";
                    }
                }

                XmlSerializer _serializer = new XmlSerializer(typeof(infoTributaria));
                using (var reader = new StringReader(xml))
                {
                    //_infoTributaria = (autorizacion)serializer.Deserialize(reader);
                    _infoTributaria = (infoTributaria)_serializer.Deserialize(reader);
                }
                if (_infoTributaria != null)
                {
                    var documnetName = "infoFactura";
                    _serializer = new XmlSerializer(typeof(infoFactura));
                    switch (_infoTributaria.codDoc)
                    {
                        case "03":
                            documnetName = "infoLiquidacionCompra";
                            _serializer = new XmlSerializer(typeof(infoLiquidacionCompra));
                            break;
                        case "04":
                            documnetName = "infoNotaCredito";
                            _serializer = new XmlSerializer(typeof(infoNotaCredito));
                            break;
                        case "05":
                            documnetName =  "infoNotaDebito";
                            _serializer = new XmlSerializer(typeof(infoNotaDebito));
                            break;
                        case "06":
                            documnetName = "infoGuiaRemision";
                            _serializer = new XmlSerializer(typeof(infoGuiaRemision));
                            break;
                        case "07":
                            documnetName = "infoCompRetencion";
                            _serializer = new XmlSerializer(typeof(infoCompRetencion));
                            break;
                        default:
                            break;
                    }

                    indiceTagComprobante = xml2.IndexOf($"<{documnetName}>");
                     indiceTagComprobanteCierre = -1;
                    if (indiceTagComprobante >= 0)
                    {
                        indiceTagComprobanteCierre = xml2.IndexOf($"</{documnetName}>");
                        if (indiceTagComprobanteCierre > indiceTagComprobante && indiceTagComprobanteCierre > 0 && indiceTagComprobanteCierre > 0)
                        {
                            string comprobanteXML = xml2.Substring(indiceTagComprobante + $"<{documnetName}>".Length, indiceTagComprobanteCierre - indiceTagComprobante - $"</{documnetName}>".Length + 1);
                            xml2 = $"<{documnetName}>{comprobanteXML}</{documnetName}>";
                        }
                    }                    
                    using (var reader = new StringReader(xml2))
                    {
                        var _ruc = "";                       
                        switch (_infoTributaria.codDoc)
                        {
                            case "03":
                                var _infoLiquidacion = (infoLiquidacionCompra)_serializer.Deserialize(reader);
                                _ruc = _infoLiquidacion.identificacionComprador;
                                break;
                            case "04":
                                var _infoNotaCredito = (infoNotaCredito)_serializer.Deserialize(reader);
                                _ruc = _infoNotaCredito.identificacionComprador;                                
                                break;
                            case "05":
                                var _infoNotaDebito = (infoNotaDebito)_serializer.Deserialize(reader);
                                _ruc = _infoNotaDebito.identificacionComprador;  
                                break;
                            case "06":
                                var _infoGuiaRemision = (infoGuiaRemision)_serializer.Deserialize(reader);
                                _ruc = _infoGuiaRemision.identificacionComprador;
                                                        
                                break;
                            case "07":
                                var _infoCompRetencion = (infoCompRetencion)_serializer.Deserialize(reader);
                                _ruc = _infoCompRetencion.identificacionSujetoRetenido; 
                                break;
                            default:
                                var _infoFactura = (infoFactura)_serializer.Deserialize(reader);
                                _ruc = _infoFactura.identificacionComprador;
                                break;
                        }

                        if (SessionInfo.Issuer != null)
                        {
                            if (!SessionInfo.Issuer.RUC.Equals(_ruc))
                            {
                                errormsg = "Verifica el archivo seleccionado: el número de identificación del cliente en el archivo XML, es diferente a su RUC, por favor verifique.";
                            }
                        }
                        else {
                            errormsg = "Cofigurar emisor: para utilizar es opción debes configurar la cuenta, por favor verifique.";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            return string.IsNullOrWhiteSpace(errormsg);
        }


    }

    public class SendMailRequest
    {
        public string email { get; set; }
        public string uid { get; set; }
    }
}
