using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Http;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models;
using System.Linq.Expressions;
using System.Collections;
using X.PagedList;
using System.IO;
using Newtonsoft.Json;
using System.Web.Http.Description;
using System.Threading.Tasks;
using Ecuafact.WebAPI.Domain.Reporting;
using System.ComponentModel;
using Ecuafact.WebAPI.Domain.Dal.Core;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// DOCUMENTOS: Proceso para generar documentos electronicos genericos
    /// </summary>
    [EcuafactExpressAuthorize]
    [DisplayName("Documentos")]
    public class DocumentsController : DocumentAPIControllerBase<DocumentRequestModel>
    {
        /// <summary>
        /// Documento
        /// </summary>
        /// <param name="contributorsService"></param>
        /// <param name="catalogsService"></param>
        /// <param name="productsService"></param>
        /// <param name="documentsService"></param>
        /// <param name="taxesService"></param>
        /// // <param name="subscriptionService"></param>         
        /// <param name="appService"></param> 
        public DocumentsController(IContributorsService contributorsService, ICatalogsService catalogsService,
            IProductsService productsService, IDocumentsService documentsService, ITaxesService taxesService, ISubscriptionService subscriptionService, IAppService appService)
            : base(contributorsService, catalogsService, productsService, documentsService, taxesService, subscriptionService, appService)
        {
        }


        /// <summary>
        ///     Buscar documentos
        /// </summary>
        /// <remarks>
        ///     Realiza la busqueda de documentos electrónicos usando una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de documentos electrónicos</returns>
        [HttpGet, Route("documents")]
        public async  Task<IPagedList<Document>> SearchDocuments(string search = "", DateTime? startDate = null, DateTime? endDate = null, string documentType = "", DocumentStatusEnum? status = null, int? page = 1, int? pageSize = null, bool includeDeleted = false,
            string establishmentCode = null, string issuePointCode = null)
        {
            return await GetDocuments(search, startDate, endDate, documentType, status, page, pageSize, includeDeleted, document => document, establishmentCode, issuePointCode);
        }

        /// <summary>
        /// Buscar documentos recibidos
        /// </summary>
        /// <remarks>
        ///     Realiza la busqueda de documentos electrónicos recibidos usando una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de documentos electrónicos recibidos</returns>
        [HttpGet, Route("documents/received/page")]
        public async Task<DocumentResponse> SearchDocumentsReceivedPage(string search = "", DateTime? startDate = null, DateTime? endDate = null, string documentType = "", string deductible = "", int? page = 1, int? pageSize = null)
        {
            var securityToken = Request.Headers?.Authorization?.Parameter;
            return await GetDocumentsReceivedPage(search, startDate, endDate, documentType, deductible, page, pageSize);
            
        }

        /// <summary>
        /// Buscar documentos recibidos
        /// </summary>
        /// <remarks>
        ///     Realiza la busqueda de documentos electrónicos recibidos usando una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de documentos electrónicos recibidos</returns>
        [HttpGet, Route("documents/received")]
        public async Task<List<DocumentReceived>> SearchDocumentsReceived(string search = "", DateTime? startDate = null, DateTime? endDate = null, string documentType = "", string deductible = "", int? page = 1, int? pageSize = null)
        {
            var securityToken = Request.Headers?.Authorization?.Parameter;
            return await GetDocumentsReceived(securityToken, search, startDate, endDate, documentType, deductible, page, pageSize, document => document);
        }

        /// <summary>
        ///    Obtener Documento recibido
        /// </summary>
        /// <remarks>
        ///     Devuelve un documento electronico recibido por ID 
        /// </remarks>
        /// <param name="id">Id del documento recibido</param>
        /// <returns></returns>
        [HttpGet, Route("documents/{id}/received")]
        public async Task<HttpResponseMessage> GetDocumentReceivedById(string id)
        {
            try
            {
                var securityToken = Request.Headers?.Authorization?.Parameter;
                if (!string.IsNullOrEmpty(id))
                {
                    return await GetDocumentReceivedById(Convert.ToInt64(id));
                }

                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
            }
            catch (HttpResponseException ex)
            {
                return ex.Response;
            }
            catch (Exception ex)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), $"Hubo un error al buscar el documento con el valor {id}!");
            }
        }

        /// <summary>
        ///    Obtener Documento recibido
        /// </summary>
        /// <remarks>
        ///     Devuelve un documento electronico recibido por ID 
        /// </remarks>
        /// <param name="id">Id del documento recibido</param>
        /// <returns></returns>
        [HttpGet, Route("documents/{id}/received/detail")]
        public async Task<HttpResponseMessage> GetDocumentReceivedDetailById(string id)
        {
            try
            {
                var securityToken = Request.Headers?.Authorization?.Parameter;
                if (!string.IsNullOrEmpty(id))
                {
                    return await GetDocumentReceivedDetailById(Convert.ToInt64(id));
                }

                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
            }
            catch (HttpResponseException ex)
            {
                return ex.Response;
            }
            catch (Exception ex)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), $"Hubo un error al buscar el documento con el valor {id}!");
            }
        }

        /// <summary>
        ///    Obtener Documento recibido
        /// </summary>
        /// <remarks>
        ///     Devuelve un documento electronico recibido por ID 
        /// </remarks>
        /// <param name="id">Id del documento recibido</param>
        /// <param name="supportTypeCode">código del tipo sustento</param>
        /// <param name="emissionType">tipo de emisión</param>
        /// <returns></returns>
        [HttpPost, Route("documents/received/{id}/sustenancetype/{supportTypeCode}/{emissionType}")]
        public async Task<HttpResponseMessage> SustenanceTypeDocumentReceived(long id = 0, string supportTypeCode = "", int emissionType = 1)
        {
            try
            {
                var securityToken = Request.Headers?.Authorization?.Parameter;
                if (id > 0 && !string.IsNullOrWhiteSpace(supportTypeCode))
                {
                    return await SustenanceTypeDocument(id, supportTypeCode, emissionType);
                }
                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
            }
            catch (HttpResponseException ex)
            {
                return ex.Response;
            }
            catch (Exception ex)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), $"Hubo un error al buscar el documento con el valor {id}!");
            }
        }

        /// <summary>
        ///     Crear Nota de venta recibida
        /// </summary>
        /// <remarks>
        ///     Permite crear una nota de venta.
        /// </remarks>
        /// <param name="requestModel">datos de la nota de venta</param>
        /// <returns>Devuelve la nota de venta creada.</returns>
        [HttpPost, Route("documents/received/salesnote")]
        [ResponseType(typeof(DocumentReceived))]       
        public async Task<HttpResponseMessage> PostSaleNote(SalesNoteRequestModel requestModel)
        {
            try
            {
                this.AllowFinalConsumer = false;
                var result = await Task.FromResult(SaveReceived(requestModel));  
                return Request.CreateResponse(HttpStatusCode.Created, result, JsonCustomFormatter.Default);
            }
            catch (HttpResponseException ex)
            {
                Log("Request Message: ", Request?.Content?.ReadAsStringAsync()?.Result);
                Log(ex.Response, requestModel);
                return ex.Response;
            }
            catch (Exception ex)
            {
                Log("Request Message: ", Request?.Content?.ReadAsStringAsync()?.Result);
                Log(ex, requestModel);
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la nota de venta!");
            }
        }



        /// <summary>
        /// Descargar Documento
        /// </summary>
        /// <remarks>
        ///     Permite la descarga del archivo de la nota de venta.
        /// </remarks>
        /// <param name="id"></param>       
        /// <returns></returns>
        [HttpGet, Route("documents/{id}/salesnote/download")]
        public async Task<HttpResponseMessage> DownloadDocument(string id)
        {
            return await DownloadSaleNoteFile(id);
        }

        /// <summary>
        ///     Reporte documentos
        /// </summary>
        /// <remarks>
        ///     Realiza la busqueda de documentos electrónicos usando una serie de filtros
        /// </remarks>
        /// <returns>Devuelve reporte en excel de los documentos electrónicos</returns>
        [HttpGet, Route("documents/export/received")]
        public async Task<HttpResponseMessage> GetExportReceived(string search = "", DateTime? startDate = null, DateTime? endDate = null, string documentType = "", string deductible ="")
        {
            return await ExportReceiveds(search, startDate, endDate, documentType, deductible);
        }

        /// <summary>
        ///     Obtener Documento
        /// </summary>
        /// <remarks>
        ///     Devuelve un documento electronico por ID o Numero de documento
        /// </remarks>
        /// <param name="id">Id del documento o numero del documento en formato XXX-XXX-XXXXXXXXX</param>
        /// <returns></returns>
        [HttpGet, Route("documents/{id}")]
        public HttpResponseMessage GetDocumentById(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    Document document = null;
                    IQueryable<Document> documents = base.GetDocuments();
                    
                    // Buscamos el documento por numero de autorizacion y de acceso.
                    if (id.Length >= 20)
                    {
                        document = documents.FirstOrDefault(model => model.AuthorizationNumber == id || model.AccessKey == id);
                    }
                    else
                    {
                        if (id.Contains("-"))
                        {
                            document = documents.FirstOrDefault(model => model.IssuerId == Issuer.Id  && model.EstablishmentCode + "-" + model.IssuePointCode + "-" + model.Sequential == id);
                        }
                        else
                        {
                            var issuerId = Issuer.Id;
                            var idLong = Convert.ToInt64(id);

                            document = _documentsService.GetIssuerDocumentById(issuerId, idLong);

                            if (document == null)
                            {
                                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
                            }
                        }

                    }

                    if (document != null)
                    {
                        if(document.Status == DocumentStatusEnum.Authorized)
                        {
                            document.Status = DocumentStatusEnum.Validated;
                        }
                        return Request.CreateResponse<Document>(HttpStatusCode.OK, document);
                    }
                }

                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
            }
            catch (HttpResponseException ex)
            { 
                return ex.Response;
            }
            catch (Exception ex)
            { 
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), $"Hubo un error al buscar el documento con el valor {id}!");
            }
        }


        /// <summary>
        ///    Enviar Documento al SRI
        /// </summary>
        /// <remarks>
        ///     Permite emitir un documento electrónico existente para su emisión en el SRI. 
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <param name="issueDate"></param>
        /// <returns></returns>
        [HttpPost, Route("documents/{id}/send")]
        public Document IssueDocument(long id, string reason = null, string issueDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(Issuer.Certificate?.Trim()) || string.IsNullOrEmpty(Issuer.CertificatePass?.Trim()))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "Usted no ha configurado su firma Electronica o ya ha caducado!");
                }   

                var subsc = _subscriptionService.GetSubscription(Issuer.RUC);
                if (subsc != null) {
                    if(subsc.Status == SubscriptionStatusEnum.Inactiva) {
                        throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "Suscripción inactiva", $"{subsc.StatusMsg}, por el momento no puedes emitir el documento(s).");
                    }
                    else if (subsc.LicenceType.Code != Constants.PlanPro){
                        if (subsc.BalanceDocument <= 0) {
                            throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "Sin emisión, a llegado al limite del plan.",
                                $"La cantidad de documentos de emisión de tu plan {subsc.LicenceType.Name} a llegado al limite, y por el momento no puedes emitir documento(s).");
                        }
                    }
                }

                var _issueDate = Convert.ToDateTime(issueDate, new CultureInfo("es"));
                var result = _documentsService.IssueDocument(Issuer.Id, id, reason, _issueDate);

                if (result.IsSuccess)
                {
                    if (result?.Entity != null)
                    {
                        if(result?.Entity.Status == DocumentStatusEnum.Authorized)
                        {
                            result.Entity.Status = DocumentStatusEnum.Validated;
                        }
                        return result?.Entity;
                    }

                    throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "Hubo un problema al emitir el documento!");
                }

                throw Request.BuildHttpErrorException(result);
            }
            catch (HttpResponseException ex)
            {
                Log(ex, ex.Response?.Content?.ReadAsStringAsync()?.Result);
                
                throw ex;
            }
            catch (Exception ex)
            {
                Log(ex);

                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, ex.ToString(), "Error al guardar el Documento!");
            }

        }
         
        /// <summary>
        /// Eliminar Documento
        /// </summary>
        /// <remarks>
        ///     Elimina el documento electrónico especificado. Debe especificar la razón de la eliminación.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpDelete, Route("documents/{id}")]
        public IHttpActionResult DeleteDocument(long id, string reason = null)
        {
            // En realidad los documentos no se eliminan cambian de estado.
            try
            {
                var result = _documentsService.DeleteDocument(Issuer.Id, id, reason);

                if (!result.IsSuccess)
                {
                    throw Request.BuildHttpErrorException(result.StatusCode, result.DevMessage, result.UserMessage);
                }
                
                return Ok(result);
            }
            catch (HttpResponseException ex)
            {
                Log(ex, ex.Response?.Content?.ReadAsStringAsync()?.Result);

                return ResponseMessage(ex.Response);
            }
            catch (Exception ex)
            {
                Log(ex);

                var exMsg = $"{ex.Message}-{ex.InnerException?.Message}-{ex.InnerException?.InnerException?.Message}";

                return ResponseMessage(Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, exMsg, "Error al guardar el Documento!"));
            }

        }


        /// <summary>
        /// Crear Documento 
        /// </summary>
        /// <remarks>
        ///     Permite crear un documento electrónico
        /// </remarks>
        /// <param name="requestModel">Informacion del Documento</param>
        /// <returns>Devuelve un mensaje de respuesta incluyendo un codigo HTTP e informacion adicional</returns>
        [HttpPost, Route("documents")]
        public HttpResponseMessage PostDocument(DocumentRequestModel requestModel)
        {
            Log("Guardando el documento externo recibido:", requestModel);

            try
            {
                if (requestModel == null)
                {
                    return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, $"El documento enviado es invalido y no se pudo procesar! Por favor revise la estructura. {requestModel}", "El documento enviado es invalido!");
                }

                // Estos documentos son de inmediata emision:
                requestModel.Status = NewDocumentStatusEnum.Issued;

                // Actualizamos el tipo de documento: De forma predeterminada debe ser factura
                requestModel.DocumentTypeCode = _documentType = requestModel.DocumentTypeCode ?? "01";                

                // Guardamos el documento
                var result = Save(requestModel);

                if (result.DocumentTypeCode == DocumentTypeEnum.Invoice.GetCoreValue())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, result.ToInvoice(), JsonCustomFormatter.Default);
                }
                else if (result.DocumentTypeCode == DocumentTypeEnum.RetentionReceipt.GetCoreValue())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, result.ToRetention(), JsonCustomFormatter.Default);
                }
                else if (result.DocumentTypeCode == DocumentTypeEnum.ReferralGuide.GetCoreValue())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, result.ToReferralGuide(), JsonCustomFormatter.Default);
                }
                else if (result.DocumentTypeCode == DocumentTypeEnum.CreditNote.GetCoreValue())
                {
                    return Request.CreateResponse(HttpStatusCode.Created, result.ToCreditNote(), JsonCustomFormatter.Default);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Created, result, JsonCustomFormatter.Default);
                }
            }
            catch (HttpResponseException ex)
            {
                Log("Request Message: ", Request?.Content?.ReadAsStringAsync()?.Result);

                Log(ex.Response, requestModel);

                return ex.Response;
            }
            catch (Exception ex)
            {
                Log("Request Message: ", Request?.Content?.ReadAsStringAsync()?.Result);

                Log(ex, requestModel);
                
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Error al guardar el documento!");
            }
        }

        /// <summary>
        /// Descargar Documento
        /// </summary>
        /// <remarks>
        ///     Permite la descarga del documento especificado en diferentes formatos: PDF, XML, Word, Excel, etc.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="formatType"></param>
        /// <returns></returns>
        [HttpGet, Route("documents/{id}/{formatType}")]
        public HttpResponseMessage DownloadDocument(string id, string formatType)
        {
            return Download(id, formatType);
        }

        /// <summary>
        ///     Reporte documentos
        /// </summary>
        /// <remarks>
        ///     Realiza la busqueda de documentos electrónicos usando una serie de filtros
        /// </remarks>
        /// <returns>Devuelve reporte en excel de los documentos electrónicos</returns>
        [HttpGet, Route("documents/export")]
        public async Task<HttpResponseMessage> Export(string search = "", DateTime? startDate = null, DateTime? endDate = null, string documentType = "", DocumentStatusEnum? status = null, int? page = 1, int? pageSize = null, bool includeDeleted = false,
            string establishmentCode = null, string issuePointCode = null)
        {
            return await ExportDocuments(search, startDate, endDate, documentType, status, page, pageSize, includeDeleted, establishmentCode, issuePointCode);
        }

        /// <summary>
        /// Permite reenviar un documento electronico emitido
        /// </summary>
        /// <param name="id"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpPost, Route("documents/{id}/email")]
        public OperationResult EmailDocument(long id, string to = null)
        {
            try
            {
                var doc = _documentsService.GetIssuerDocumentById(Issuer.Id, id);

                if (doc != null)
                {
                    return SendMail(doc, to);
                }

                throw new Exception("No se pudo enviar el correo electrónico");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, HttpStatusCode.InternalServerError, $"{ex.Message} {ex?.InnerException?.Message}");
            }
        }

        /// <summary>
        ///Sincronizar Documentos
        /// </summary>
        /// <remarks>
        ///     Permite la sincronización de los documentos recibidos.
        /// </remarks>
        [HttpGet, Route("documents/received/syncup")]
        public HttpResponseMessage SyncUpReceived()
        {
            return SyncUp();
        }

        private OperationResult SendMail(Document doc, string email)
        {
            var docType = doc.GetDocumentType();

            var emailer = Emailer.Create("document-email", $"{doc.BussinesName}: {docType.GetDisplayValue()} # {doc.DocumentNumber}", email);
            var filename = string.IsNullOrWhiteSpace(doc.AccessKey) ? $"{doc.RUC}_{doc.DocumentNumber}" : doc.AccessKey;
             
            emailer.Parameters.Add("Cliente", doc.ContributorName);
            emailer.Parameters.Add("TipoDocumento", docType.GetDisplayValue());
            emailer.Parameters.Add("NumDocumento", doc.DocumentNumber);
            emailer.Parameters.Add("RucEmisor", doc.RUC);
            emailer.Parameters.Add("Emisor", doc.BussinesName);
            emailer.Parameters.Add("NumAutoriza", doc.AuthorizationNumber ?? "NO AUTORIZADO");

            var ride = GetDocumentFile(doc, "pdf");

            if (ride == null)
            {
                var report = doc.GetReportBuilder(Issuer.ToIssuer(), "pdf");

                if (report != null)
                {
                    ride = report.Content;
                }
                else
                {
                    throw new Exception("Hubo un error al procesar el documento electrónico!");
                }
            }

            emailer.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(ride), $"{filename}.pdf"));
            
            var xml = GetDocumentFile(doc, "xml");
            
            if (xml != null)
            {
                emailer.Attachments.Add(new System.Net.Mail.Attachment(new MemoryStream(xml), $"{filename}.xml"));
            }
             
            if (emailer.Send())
            {
                return new OperationResult(true, HttpStatusCode.OK, "El mensaje se ha enviado con éxito!");
            }

            return new OperationResult(false, HttpStatusCode.InternalServerError, "No se pudo enviar el correo!");
        }

        [HttpGet, Route("documents/{id}/process/Xml")]
        public HttpResponseMessage ProcessDocumentXml(long id)
        {
            var securityToken = Request.Headers?.Authorization?.Parameter;
            return ProcesDocumentXmlById(securityToken,id).Result;
        }

        /// <summary>
        /// anular Documento recibido
        /// </summary>
        /// <remarks>
        /// Anula el documento electrónico especificado.
        /// </remarks>
        /// <param name="id"></param>       
        /// <returns></returns>
        [HttpDelete, Route("documents/received/{id}/delete")]
        public OperationResult DeleteDocumentReceived(long id)
        {
            if (id > 0)
            {
                return DeleteDocumentReceivedById(id);
            }

            return new OperationResult(false, HttpStatusCode.InternalServerError, "No se pudo anular el documento!");

        }

        #region Document API Implementation

        private string _documentType = "01";
        /// <summary>
        /// Tipo de Documento
        /// </summary>
        protected override string DocumentType
        {
            get { return _documentType; }
        }

        /// <summary>
        /// Valida el RequestModel
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        protected override bool ValidateRequest(DocumentRequestModel requestModel, out string errormsg)
        {
            errormsg = "OK";

            if (requestModel.Details == null || !requestModel.Details.Any())
            {
                errormsg = "El documento debe incluir por lo menos un detalle valido!";
                return false;
            }

            if (!ValidateDetails(requestModel.Details, out errormsg))
            {
                return false;
            }

            // Corregimos los detalles que utilizan un modelo antiguo
            requestModel.Details.ForEach(detailModel =>
            {
                detailModel.Description = string.IsNullOrEmpty(detailModel.Description) ? detailModel.Name : detailModel.Description;

                if (requestModel.Details.Count == 0)
                {
                    // Si esta factura contiene valores con subtotal zero y este producto puede contener impuesto zero
                    if (requestModel.SubtotalVatZero > 0 &&
                        detailModel.SubTotal <= requestModel.SubtotalVatZero)
                    {
                        // Le agregamos los impuestos
                        detailModel.Taxes.Add(new TaxModel { Code = "2", PercentageCode = "0", Rate = 0, TaxableBase = detailModel.SubTotal, TaxValue = 0 });
                    }
                    else // Si el valor de la factura contiene iva debe contener impuestos en los detalles:
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "Debe especificar los impuestos en los detalles de los productos", "Error en los datos del documento");

                    }
                }
            });

            if (requestModel.ContributorId == 0 && string.IsNullOrEmpty(requestModel.Identification))
            {
                errormsg = "El documento debe incluir la identificacion del contribuyente!";
                return false;
            }

            if (requestModel.Payments == null || requestModel.Payments.Count == 0)
            {
                // En lugar de enviar un mensaje de error llenamos este detalle:
                requestModel.Payments = new List<PaymentModel>()
                {
                    new PaymentModel
                    {
                        PaymentMethodCode = DEFAULT_PAYMENT_METHOD,
                        Name =  DEFAULT_PAYMENT_NAME,
                        Term = 0,
                        TimeUnit = "dias",
                        Total = requestModel.Total
                    }
                };
            }
            else
            {
                var payMethod = _catalogsService.GetPaymentMethods();
                foreach (var item in requestModel.Payments)
                {
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        var payName = payMethod.FirstOrDefault(model => model.SriCode == item.PaymentMethodCode)?.Name;

                        item.Name = payName ?? $"OTROS METODOS DE PAGO";
                    }
                }
            }
             

            return base.ValidateRequest(requestModel, out errormsg);

        }


        /// <summary>
        /// Generador de Documentos
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document GenerateDocument(DocumentRequestModel requestModel)
        {
            var document = requestModel.ToDocument(requestModel.DocumentTypeCode);

            return SetDocumentInfo(document, requestModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document ChangeDocument(Document document, DocumentRequestModel requestModel)
        {
            document.MapTo(requestModel, requestModel.DocumentTypeCode);

            return SetDocumentInfo(document, requestModel);
        }

        private Document SetDocumentInfo(Document document, DocumentRequestModel requestModel)
        {
            // FACTURA
            if (requestModel.DocumentTypeCode == DocumentTypeEnum.Invoice.GetCoreValue())
            {
                document.InvoiceInfo = this.GetInvoiceInfo(requestModel);
            }

            // RETENCION
            if (requestModel.DocumentTypeCode == DocumentTypeEnum.RetentionReceipt.GetCoreValue())
            {
                document.RetentionInfo = this.GetRetentionInfo(requestModel);
            }

            // NOTA DE CREDITO
            if (requestModel.DocumentTypeCode == DocumentTypeEnum.CreditNote.GetCoreValue())
            {
                document.CreditNoteInfo = this.GetCreditNoteInfo(requestModel);
            }

            // GUIA DE REMISION
            if (requestModel.DocumentTypeCode == DocumentTypeEnum.ReferralGuide.GetCoreValue())
            {
                document.ReferralGuideInfo = this.GetReferralGuideInfo(requestModel);
            }

            return document;
        }


        private InvoiceInfo GetInvoiceInfo( DocumentRequestModel requestModel)
        {
            var invoiceInfo = new InvoiceInfo
            {
                EstablishmentAddress = Issuer.MainAddress,
                IdentificationType = requestModel.IdentificationType,
                Identification = requestModel.Identification,
                BussinesName = requestModel.ContributorName,
                Currency = requestModel.Currency,
                ReferralGuide = requestModel.ReferralGuide,
                SubtotalVat = requestModel.SubtotalVat,
                SubtotalVatZero = requestModel.SubtotalVatZero,
                SubtotalExempt = requestModel.SubtotalExempt,
                SubtotalNotSubject = requestModel.SubtotalNotSubject,
                Subtotal = requestModel.Subtotal,
                Total = requestModel.Total,
                TotalDiscount = requestModel.TotalDiscount,
                SpecialConsumTax = requestModel.SpecialConsumTax,
                ValueAddedTax = requestModel.ValueAddedTax,
                Tip = requestModel.Tip,
                Address = requestModel.Address,
                IssuedOn = requestModel.IssuedOn,

                // INCOTERM
                //InvoiceIncoTerm = requestModel.InvoiceIncoTerm,
                //PlaceIncoTerm = requestModel.PlaceIncoTerm,
                //OriginCountry = requestModel.OriginCountry,
                //PortBoarding = requestModel.PortBoarding,
                //DestinationPort = requestModel.DestinationPort,
                //DestinationCountry = requestModel.DestinationCountry,
                //CountryAcquisition = requestModel.CountryAcquisition,
                //TotalWithoutTaxesIncoTerm = requestModel.TotalWithoutTaxesIncoTerm,

                //// ACTUALIZACION!!: Referencia del documento modificado, por lo general es usado por las notas de debito o credito:
                //ModifiedDocumentCode = customRequestModel.ModifiedDocumentCode,
                //ModifiedDocumentNumber = customRequestModel.ModifiedDocumentNumber,
                //SupportDocumentIssueDate = customRequestModel.SupportDocumentIssueDate
            };


            if (requestModel.Payments != null && requestModel.Payments.Any())
            {
                invoiceInfo.Payments = requestModel.Payments.Select(py => py.ToPayment()).ToList();

                // Proceso para guardar los nombres del tipo de pago para reporteria
                var paymentNotName = invoiceInfo.Payments.FindAll(p => string.IsNullOrEmpty(p.Name));
                if (paymentNotName.Any())
                {
                    var method = _catalogsService.GetPaymentMethods().ToList();
                    foreach (var pay in paymentNotName)
                    {
                        if (string.IsNullOrEmpty(pay.PaymentMethodCode))
                        {
                            pay.PaymentMethodCode = "01";
                        }

                        var result = method.Find(m => pay.PaymentMethodCode == m.SriCode)?.Name ?? "SIN UTILIZACION DEL SISTEMA FINANCIERO";

                        pay.Name = result;
                    }
                }
            }
            else
            {
                // Si no hay formas de pago especificadas, 
                // entonces definimos un tipo de pago predeterminado
                invoiceInfo.Payments = new List<Domain.Entities.Payment>() {
                        new Domain.Entities.Payment
                        {
                            PaymentMethodCode ="01",
                            Name = "SIN UTILIZACION DEL SISTEMA FINANCIERO",
                            Term = 0, TimeUnit="dias",
                            Total = invoiceInfo.Total
                        }
                    };
            }

            invoiceInfo.Details = new List<DocumentDetail>();
            // El detalle de los totales igualmente se autogenera ya no es necesario enviarlo en el requestmodel
            invoiceInfo.TotalTaxes = new List<TotalTax>();

            foreach (var detailModel in requestModel.Details)
            {
                //var product = _productsService.GetIssuerProduct(detailModel.ProductId, Issuer.Id);
                var detail = new DocumentDetail
                {
                    MainCode = detailModel.MainCode,
                    AuxCode = detailModel.AuxCode,
                    UnitPrice = detailModel.UnitPrice,
                    Description = detailModel.Name ?? detailModel.Description,
                    Amount = detailModel.Amount,
                    Discount = detailModel.Discount,
                    SubTotal = detailModel.SubTotal
                };

                detail.Taxes = detailModel.Taxes.Select(tx => tx.ToTax()).ToList();
                foreach (var tax in detail.Taxes)
                {
                    var totalTax = invoiceInfo.TotalTaxes
                            .FirstOrDefault(tt => tt.TaxCode == tax.Code && tt.PercentageTaxCode == tax.PercentageCode);

                    // Calculamos los impuestos totales
                    if (totalTax == null)
                    {
                        invoiceInfo.TotalTaxes.Add(
                            new TotalTax
                            {
                                // Informacion del impuesto
                                TaxCode = tax.Code,
                                PercentageTaxCode = tax.PercentageCode,
                                TaxRate = tax.Rate,
                                // Datos Variables
                                TaxableBase = tax.TaxableBase,
                                TaxValue = tax.TaxValue
                            });
                    }
                    else
                    {
                        totalTax.TaxableBase += tax.TaxableBase;
                        totalTax.TaxValue += tax.TaxValue;
                    }
                }
                

                invoiceInfo.Details.Add(detail);
            }


            return invoiceInfo;
        }


        private CreditNoteInfo GetCreditNoteInfo(DocumentRequestModel requestModel)
        {
            var creditNoteInfo = new CreditNoteInfo
            {
                IdentificationType = requestModel.IdentificationType,
                Identification = requestModel.Identification,
                BussinesName = requestModel.ContributorName,
                Currency = requestModel.Currency,
                SubtotalVat = requestModel.SubtotalVat,
                SubtotalVatZero = requestModel.SubtotalVatZero,
                SubtotalExempt = requestModel.SubtotalExempt,
                SubtotalNotSubject = requestModel.SubtotalNotSubject,
                Subtotal = requestModel.Subtotal,
                Total = requestModel.Total,
                TotalDiscount = requestModel.TotalDiscount,
                SpecialConsumTax = requestModel.SpecialConsumTax,
                ValueAddedTax = requestModel.ValueAddedTax,
                Tip = requestModel.Tip,
                Address = requestModel.Address,
                IssuedOn = requestModel.IssuedOn,

                ModifiedValue = requestModel.Total,
                Reason = requestModel.Reason,


                ReferenceDocumentId = requestModel.ReferenceDocumentId,
                ReferenceDocumentAuth = requestModel.ReferenceDocumentAuth,
                ReferenceDocumentCode = requestModel.ReferenceDocumentCode,
                ReferenceDocumentDate = requestModel.ReferenceDocumentDate,
                ReferenceDocumentNumber = requestModel.ReferenceDocumentNumber

            };



            creditNoteInfo.Details = new List<DocumentDetail>();
            // El detalle de los totales igualmente se autogenera ya no es necesario enviarlo en el requestmodel
            creditNoteInfo.TotalTaxes = new List<TotalTax>();

            foreach (var detailModel in requestModel.Details)
            {
                //var product = _productsService.GetIssuerProduct(detailModel.ProductId, Issuer.Id);
                var detail = new DocumentDetail
                {
                    MainCode = detailModel.MainCode,
                    AuxCode = detailModel.AuxCode,
                    UnitPrice = detailModel.UnitPrice,
                    Description = detailModel.Name ?? detailModel.Description,
                    Amount = detailModel.Amount,
                    Discount = detailModel.Discount,
                    SubTotal = detailModel.SubTotal
                };
                 

                if (detailModel.Taxes == null || !detailModel.Taxes.Any())
                {
                    // Si este documento contiene valores con subtotal zero y este producto puede contener impuesto zero
                    if (requestModel.SubtotalVatZero != 0 && detailModel.SubTotal <= requestModel.SubtotalVatZero)
                    {
                        // Le agregamos los impuestos
                        detailModel.Taxes = new List<TaxModel>
                        {
                            new TaxModel
                            {
                                Code = "2",
                                PercentageCode = "0",
                                Rate = 0,
                                TaxableBase = detailModel.SubTotal,
                                TaxValue = 0
                            }
                        };
                    }
                    else // Si el valor del documento contiene iva debe contener impuestos en los detalles:
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "Debe especificar los impuestos en los detalles de los productos", "Error en los datos del documento");
                    }
                }

                detail.Taxes = detailModel.Taxes.Select(tx => tx.ToTax()).ToList();

                foreach (var tax in detail.Taxes)
                {
                    var totalTax = creditNoteInfo.TotalTaxes
                            .FirstOrDefault(tt => tt.TaxCode == tax.Code && tt.PercentageTaxCode == tax.PercentageCode);

                    // Calculamos los impuestos totales
                    if (totalTax == null)
                    {
                        creditNoteInfo.TotalTaxes.Add(
                            new TotalTax
                            {
                                // Informacion del impuesto
                                TaxCode = tax.Code,
                                PercentageTaxCode = tax.PercentageCode,
                                TaxRate = tax.Rate,
                                // Datos Variables
                                TaxableBase = tax.TaxableBase,
                                TaxValue = tax.TaxValue
                            });
                    }
                    else
                    {
                        totalTax.TaxableBase += tax.TaxableBase;
                        totalTax.TaxValue += tax.TaxValue;
                    }
                }
                 

                creditNoteInfo.Details.Add(detail);
            }


            return creditNoteInfo;
        }

        private ReferralGuideInfo GetReferralGuideInfo(DocumentRequestModel requestModel)
        {
            return new ReferralGuideInfo
            {
                IdentificationType = requestModel.IdentificationType,
                Identification = requestModel.Identification,
                BusinessName = requestModel.ContributorName,

                IssuedOn = requestModel.IssuedOn,

                ContributorId = requestModel.ContributorId,
                SenderAddress = requestModel.Address,

                OriginAddress = Issuer.MainAddress,
                DestinationAddress = requestModel.Address,

                DriverId = requestModel.ContributorId,
                DriverIdentificationType = requestModel.ContributorName,
                DriverIdentification = requestModel.Identification,
                DriverName = requestModel.ContributorName,

                CarPlate = "XXX-XXXX",
                ShippingStartDate = requestModel.IssuedOn,
                ShippingEndDate = requestModel.IssuedOn,

                RecipientId = requestModel.ContributorId,
                RecipientIdentificationType = requestModel.IdentificationType,
                RecipientIdentification = requestModel.Identification,
                RecipientName = requestModel.ContributorName,
                RecipientAddress = requestModel.Address,

                Reason = requestModel.Reason,
                DAU = "",
                RecipientEstablishment = "001",
                ShipmentRoute = requestModel.Reason,

                ReferenceDocumentId = requestModel.ReferenceDocumentId,
                ReferenceDocumentCode = requestModel.ReferenceDocumentCode,
                ReferenceDocumentNumber = requestModel.ReferenceDocumentNumber,
                ReferenceDocumentDate = requestModel.ReferenceDocumentDate,
                ReferenceDocumentAuth = requestModel.ReferenceDocumentAuth,

                Details = requestModel.Details?.ConvertAll(detail => 
                new ReferralGuideDetail
                {
                    MainCode = detail.MainCode,
                    AuxCode = detail.AuxCode,
                    Description = detail.Description,
                    ProductId = detail.ProductId,
                    Quantity = Convert.ToInt32(detail.Amount)
                })
            };
        }

        private RetentionInfo GetRetentionInfo(DocumentRequestModel requestModel)
        {
            return new RetentionInfo()
            {
                IdentificationType = requestModel.IdentificationType,
                Identification = requestModel.Identification,
                BusinessName = requestModel.ContributorName,
                Currency = Issuer.Currency,

                IssuedOn = requestModel.IssuedOn,

                ContributorId = requestModel.ContributorId,
                Reason = requestModel.Reason,
                FiscalPeriod = requestModel.IssuedOn.Substring(3, 7),
                FiscalAmount = requestModel.Total,

                ReferenceDocumentCode = requestModel.ReferenceDocumentCode,
                ReferenceDocumentNumber = requestModel.ReferenceDocumentNumber,
                ReferenceDocumentDate = requestModel.ReferenceDocumentDate,
                ReferenceDocumentAuth = requestModel.ReferenceDocumentAuth,
                ReferenceDocumentAmount = requestModel.ReferenceDocumentAmount,
                ReferenceDocumentVat = requestModel.ReferenceDocumentVat,

                Details = requestModel.Details.ConvertAll(item =>
                {
                    var tax = _taxesService.SearchTaxes(item.MainCode).FirstOrDefault();

                    // Si el impuesto no existe - No se puede continuar
                    if (tax == null)
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, 
                            $"El detalle de impuestos esta incorrecto {JsonConvert.SerializeObject(item)}", 
                            $"El codigo de impuesto: [{item.MainCode}] esta incorrecto.");
                    }

                    var taxType = _catalogsService.GetTaxTypes().FirstOrDefault(o => o.Id == tax.TaxTypeId)?.SriCode;

                    return new RetentionDetail
                    {
                        ReferenceDocumentCode = requestModel.ReferenceDocumentCode,
                        ReferenceDocumentNumber = requestModel.ReferenceDocumentNumber,
                        ReferenceDocumentDate = requestModel.ReferenceDocumentDate,
                        RetentionTaxCode = tax.SriCode,
                        RetentionTaxId = tax.Id,
                        TaxBase = item.UnitPrice,
                        TaxRate = item.Amount,
                        TaxValue = item.SubTotal,
                        TaxTypeCode = tax.TaxType.SriCode
                    };
                    // Se supone que el resto esta bien
                })
            };


        }

        private DebitNoteInfo GetDebitNoteInfo(DocumentRequestModel requestModel)
        {
            var debitNoteInfo = new DebitNoteInfo
            {
                IdentificationType = requestModel.IdentificationType,
                Identification = requestModel.Identification,
                BussinesName = requestModel.ContributorName,
                Currency = requestModel.Currency,
                SubtotalVat = requestModel.SubtotalVat,
                SubtotalVatZero = requestModel.SubtotalVatZero,
                SubtotalExempt = requestModel.SubtotalExempt,
                SubtotalNotSubject = requestModel.SubtotalNotSubject,
                Subtotal = requestModel.Subtotal,
                Total = requestModel.Total,
                TotalDiscount = requestModel.TotalDiscount,
                SpecialConsumTax = requestModel.SpecialConsumTax,
                ValueAddedTax = requestModel.ValueAddedTax,               
                Address = requestModel.Address,
                IssuedOn = requestModel.IssuedOn,

                ModifiedValue = requestModel.Total,
                Reason = requestModel.Reason,


                ReferenceDocumentId = requestModel.ReferenceDocumentId,
                ReferenceDocumentAuth = requestModel.ReferenceDocumentAuth,
                ReferenceDocumentCode = requestModel.ReferenceDocumentCode,
                ReferenceDocumentDate = requestModel.ReferenceDocumentDate,
                ReferenceDocumentNumber = requestModel.ReferenceDocumentNumber

            };



            debitNoteInfo.DebitNoteDetail = new List<DebitNoteDetail>();
            // El detalle de los totales igualmente se autogenera ya no es necesario enviarlo en el requestmodel
            debitNoteInfo.TotalTaxes = new List<TotalTax>();

            foreach (var reasonModel in requestModel.Details)
            {
                
            }


            return debitNoteInfo;
        }              

        #endregion

        #region Métodos obsoletos

        /// <summary>
        /// OBSOLETO: Enviar Documento al SRI (Emision de Documentos) - Este metodo dejara de estar disponible en la proxima version del API.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <param name="issueDate-dd/MM/yyyy"></param>
        /// <returns></returns>
        [Obsolete, ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost, Route("issuedocument")]
        public Document IssueObsolete(long id, string reason = null, string issueDate = null)
        {
            return IssueDocument(id, reason, issueDate);
        }

        #endregion
    }

    #region Metodo Obsoleto v1.0

    /// <summary>
    /// Envio de Documentos personalizados [OBSOLETO] 
    /// </summary>
    [Obsolete]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class InvoiceCustomController : DocumentAPIControllerBase<CustomDocumentRequestModel>
    {
        DocumentsController __documentService;
        /// <summary>
        /// Envio de Documentos personalizados [OBSOLETO]
        /// </summary>
        /// <param name="contributorsService"></param>
        /// <param name="catalogsService"></param>
        /// <param name="productsService"></param>
        /// <param name="documentsService"></param>
        /// <param name="taxesService"></param>
        /// /// <param name="subscriptionService"></param>       
        /// <param name="appService"></param> 
        public InvoiceCustomController(IContributorsService contributorsService, ICatalogsService catalogsService,
            IProductsService productsService, IDocumentsService documentsService, ITaxesService taxesService, ISubscriptionService subscriptionService, IAppService appService)
            : base(contributorsService, catalogsService, productsService, documentsService, taxesService, subscriptionService, appService)
        {
            __documentService = new DocumentsController(contributorsService, catalogsService, productsService, documentsService, taxesService, subscriptionService, appService);
        }

        /// <summary>
        /// Envio de Documentos personalizados [OBSOLETO]
        /// </summary>
        /// <param name="requestModel">documento</param>
        /// <returns></returns>
        //[Obsolete]
        public HttpResponseMessage PostInvoiceCustom(CustomDocumentRequestModel requestModel)
        {
            try
            {
                requestModel.ReferenceDocumentNumber = requestModel.ReferenceDocumentNumber ?? requestModel.ModifiedDocumentNumber;
                requestModel.ReferenceDocumentCode = requestModel.ReferenceDocumentCode ?? requestModel.ModifiedDocumentCode;
                requestModel.ReferenceDocumentDate = requestModel.ReferenceDocumentDate ?? requestModel.SupportDocumentIssueDate;
                requestModel.ReferenceDocumentAuth = requestModel.ReferenceDocumentAuth ?? string.Empty;
                requestModel.IdentificationType = requestModel.IdentificationType ?? requestModel.ContributorType;

                return __documentService.PostDocument(requestModel);
            }
            catch (HttpResponseException ex)
            {
                Log(ex.Response, requestModel);

                return ex.Response;
            }
            catch (Exception ex)
            {
                Log(ex, requestModel);

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Error al guardar el documento!");
            }
        }

        /// <summary>
        /// ESTE METODO NO SE UTILIZA ES OPCIONAL PARA ESTE CASO:
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document GenerateDocument(CustomDocumentRequestModel requestModel)
        {
            throw new NotImplementedException();
        }

        protected override Document ChangeDocument(Document document, CustomDocumentRequestModel requestModel)
        {
            throw new NotImplementedException();
        }

    }

    #endregion

}