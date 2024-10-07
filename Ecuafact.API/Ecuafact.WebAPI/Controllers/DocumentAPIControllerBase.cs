using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using AutoMapper;
using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Dal.Services;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Pagination;
using Ecuafact.WebAPI.Domain.Reporting;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.Models.Authentication;
using Newtonsoft.Json;
using X.PagedList;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// Clase base para los controladores de documentos base
    /// </summary>
    [EcuafactExpressAuthorize]
    public abstract class DocumentAPIControllerBase<TRequestModel> : ApiController
        where TRequestModel: DocumentRequestBase
    {
        internal readonly IContributorsService _contributorsService;
        internal readonly ICatalogsService _catalogsService;
        internal readonly IProductsService _productsService;
        internal readonly IDocumentsService _documentsService;
        internal readonly ITaxesService _taxesService;
        internal readonly IEngineService _engineService;
        internal readonly ISubscriptionService _subscriptionService;
        internal readonly IAppService _appService;

        /// <summary>
        /// Metodo predeterminado de pago
        /// </summary>
        protected const string DEFAULT_PAYMENT_METHOD = "01";

        /// <summary>
        /// Nombre predeterminado del Metodo de Pago
        /// </summary>
        protected const string DEFAULT_PAYMENT_NAME = "SIN UTILIZACION DEL SISTEMA FINANCIERO";

        /// <summary>
        /// Permitir el uso de consumidor final
        /// </summary>
        protected bool AllowFinalConsumer { get; set; } = false;

        
        /// <summary>
        /// Moneda Predeterminada
        /// </summary>
        protected const string DEFAULT_CURRENCY = "DOLAR";

        /// <summary>
        /// 
        /// </summary>
        protected virtual string DocumentType
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        protected DocumentAPIControllerBase(IContributorsService contributorsService, ICatalogsService catalogsService,
            IProductsService productsService, IDocumentsService documentsService, ITaxesService taxesService, ISubscriptionService subscriptionService, IAppService appService)
        {
            _contributorsService = contributorsService;
            _catalogsService = catalogsService;
            _productsService = productsService;
            _documentsService = documentsService;
            _taxesService = taxesService;
            _engineService = new EngineService();
            _subscriptionService = subscriptionService;
            _appService = appService;
        }


        /// <summary>
        /// Devuelve un documento segun su numeracion
        /// </summary>
        /// <param name="documentNumber"></param>
        /// <returns></returns>
        protected Document GetDocument(string documentNumber)
        {
            var issuer = this.Issuer;
            var establishmentCode = issuer.EstablishmentCode;
            var issuePointCode = issuer.IssuePointCode;

            // Esta definicion solo aplica para los numeros de documento completos
            if (documentNumber.Contains("-"))
            {
                var docNumber = documentNumber.Split('-');

                // Si el numero de documento es completo debemos especificar el resto de los datos
                if (docNumber.Length == 3)
                {
                    establishmentCode = docNumber[0];
                    issuePointCode = docNumber[1];
                    documentNumber = docNumber[2];
                }
                else
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest,
                        "El formato correcto del numero de documento es XXX-XXX-XXXXXXXXX o simplemente los nueve numeros de la secuencia.",
                        "El numero de documento es incorrecto!");
                }
            }

            var document = _documentsService.GetIssuerDocument(issuer.Id, DocumentType, documentNumber, establishmentCode, issuePointCode);

            if (document == null)
                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound,
                    $"No existe el documento # {documentNumber} | Tipo: {DocumentType}.", "No existe el documento!");

            return document;
        }



        /// <summary>
        /// Proceso encargado de Guardar una nota de venta
        /// </summary>
        /// <param name="requestModel">Modelo de Datos para la nota de venta</param>
        /// <returns></returns>
        protected DocumentReceived SaveReceived(SalesNoteRequestModel requestModel)
        {
            try
            {
                Logger.Log($"LOG.{Issuer.RUC}.DOCUMENT.CREATE", "Request Message: ", requestModel);

                if (requestModel == null)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, $"La nota de venta enviada es invalida y no se pudo procesar! Por favor revise la estructura. {requestModel}", "El documento enviado es invalido!");
                }

                // Realiza la validacion si esta activada la suscripción
                var subsc = _subscriptionService.GetSubscription(Issuer.RUC);
                if (subsc != null)
                {
                    if (subsc?.Status != SubscriptionStatusEnum.Activa)
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "Suscripción inactiva", $"{subsc.StatusMsg}, por el momento no puedes emitir el documento(s).");
                    }
                    else if (subsc.LicenceType.Code != Constants.PlanPro)
                    {
                        if (subsc.BalanceDocument <= 0)
                        {
                            throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "Sin emisión, a llegado al limite del plan.",
                                $"La cantidad de documentos de emisión de tu plan {subsc.LicenceType.Name} a llegado al limite, y por el momento no puedes emitir documento(s).");
                        }
                    }
                }
                else {
                    throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "No tiene suscripción activa", $"El RUC {Issuer.RUC} no tiene suscripción, por el momento no puedes emitir el documento(s).");
                }                      

                if (!ModelState.IsValid)
                {
                    var msg = this.GetValidationErrors();
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, msg, "Los datos ingresados son incorrectos");
                }              
                var documentReceived = requestModel.ToSupplierDocument();
                // Ejecucion del proceso de grabacion del documento
                var result = _appService.AddSaleNote(documentReceived);
                                
                if (!result.IsSuccess)
                {
                    throw Request.BuildHttpErrorException(result.StatusCode, result.DevMessage, result.UserMessage);
                }   
                
                if(requestModel.DocumentPdfRaw.Length > 0)
                {
                    SaveFile(documentReceived.PDF, requestModel.DocumentPdfRaw, Constants.PDFSaleNoteFilesLocation);
                }
                
                return result.Entity.ToDocumentReceived();
            }
            catch (HttpResponseException ex)
            {
                Logger.Log($"LOG.{Issuer.RUC}.DOCUMENT.ERROR", "Exception: ", ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Logger.Log($"LOG.{Issuer.RUC}.DOCUMENT.ERROR", "Exception: ", ex);
                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, ex.ToString(), "Error al guardar el documento!");
            }
        }


        /// <summary>
        /// Proceso encargado de Guardar un nuevo Documento
        /// </summary>
        /// <param name="requestModel">Modelo de Datos para el Documento</param>
        /// <returns></returns>
        protected Document Save(TRequestModel requestModel)
        {
            try
            {
                Logger.Log($"LOG.{Issuer.RUC}.DOCUMENT.CREATE", "Request Message: ", requestModel);

                if (requestModel == null)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, $"El documento enviado es invalido y no se pudo procesar! Por favor revise la estructura. {requestModel}", "El documento enviado es invalido!");
                }

                // Realiza la validacion si esta activada la suscripción
                var subsc = _subscriptionService.GetSubscription(Issuer.RUC);
                if (subsc != null)
                {
                    if (subsc?.Status != SubscriptionStatusEnum.Activa)
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "Suscripción inactiva", $"{subsc.StatusMsg}, por el momento no puedes emitir el documento(s).");
                    }
                    else if (subsc.LicenceType.Code != Constants.PlanPro)
                    {
                        if (subsc.BalanceDocument <= 0)
                        {
                            throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "Sin emisión, a llegado al limite del plan.",
                                $"La cantidad de documentos de emisión de tu plan {subsc.LicenceType.Name} a llegado al limite, y por el momento no puedes emitir documento(s).");
                        }
                    }
                }
                else
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "No tiene suscripción activa", $"El RUC {Issuer.RUC} no tiene suscripción, por el momento no puedes emitir el documento(s).");
                }

                // Realiza la validacion de la informacion del contribuyente               
                if (!ValidateInfo(requestModel))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "Error al procesar la informacion del contribuyente.", "Documento Invalido!");
                }

                //Realiza validacion del monto del documento para el consumidor final
                if (!ValidateAmount(requestModel))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "Error al procesar el documento .", "Documento Invalido!");
                }

                // Antes de proceder debemos ejecutar el procedimiento de validacion del documento
                if (!ValidateRequest(requestModel, out string errormsg))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, errormsg, "Documento Invalido!");
                }

                if (!ModelState.IsValid)
                {
                    var msg = this.GetValidationErrors();
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, msg, "Los datos ingresados son incorrectos");
                }

                // Para poder emitir un documento es necesario que el usuario
                // tenga un certificado con su clave configurado en el sitio web, 
                // caso contrario no le permite emitir.
                if ((requestModel.Status == NewDocumentStatusEnum.Issued) && (string.IsNullOrEmpty(Issuer.Certificate) || string.IsNullOrEmpty(Issuer.CertificatePass)))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "No se puede enviar el documento al SRI, debido a que usted no ha configurado su firma electrónica.");
                }
                if ((requestModel.Status == NewDocumentStatusEnum.Issued) && (Issuer.CertificateExpirationDate < DateTime.Now))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "Error al enviar el Documento", "Atención: No puedes emitir documentos, tu firma electrónica está caducada, debes solicitarla pronto, si la renovaste no te olvides de configurarla");
                }
                var document = GenerateDocument(requestModel);
                if (requestModel.Status == NewDocumentStatusEnum.Issued && Issuer.IsEnabled)
                {
                    // Emite el documento
                    // El proceso es crear el documento y luego emitirlo si asi se lo desea
                    // De forma predeterminada al guardar se ignora el resto de opciones.
                    // Para poder emitir un documento es necesario que el emisor
                    // tenga un certificado con su clave configurado en el sitio web, 
                    // caso contrario no le permite emitir.
                    document.Status = DocumentStatusEnum.Issued;
                }
                else
                {
                    document.Status = DocumentStatusEnum.Draft;
                }

                // Ejecucion del proceso de grabacion del documento
                var result = _documentsService.AddDocument(Issuer.Id, document);

                if (!result.IsSuccess)
                {
                    throw Request.BuildHttpErrorException(result.StatusCode, result.DevMessage, result.UserMessage);
                }

                //eliminar cuando se actualice la app
                if (result.Entity.Status == DocumentStatusEnum.Authorized)
                {
                    result.Entity.Status = DocumentStatusEnum.Validated;
                }
                // Devuelvo el objeto como Json Serializado
                return result.Entity;
            }
            catch (HttpResponseException ex)
            {
                Logger.Log($"LOG.{Issuer.RUC}.DOCUMENT.ERROR", "Exception: ", ex);

                throw ex;
            }
            catch (Exception ex)
            {
                Logger.Log($"LOG.{Issuer.RUC}.DOCUMENT.ERROR", "Exception: ", ex);

                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, ex.ToString(), "Error al guardar el documento!");
            }
        }

        /// <summary>
        /// Proceso encargado de Guardar un nuevo Documento
        /// </summary>
        /// <param name="id">Id del Documento</param>
        /// <param name="requestModel">Modelo de Datos para el Documento</param>
        /// <returns></returns>
        protected Document Update(long id, TRequestModel requestModel)
        {
            try
            {
                var triggerXml = false;
                Logger.Log($"LOG.{Issuer.RUC}.DOCUMENT.UPDATE", "Request Message: ", requestModel);

                if (requestModel == null)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"El documento enviado es invalido y no se pudo procesar! Por favor revise la estructura. {requestModel}", "El documento enviado es invalido!");
                }

                // Obtener el documento que vamos a modificar.
                var document = GetDocumentById(id);
                if (document.Status == DocumentStatusEnum.Draft || document.Status == DocumentStatusEnum.Error)
                {
                    triggerXml = true;
                }

                // para recargar el documento en el Engine
                if (document.Status == DocumentStatusEnum.Error && !string.IsNullOrWhiteSpace(document.AccessKey))
                {
                    _documentsService.GetAllIssuerDocuments(document.AccessKey);
                }

                // Solo se deben modificar los documentos con estado 0 o Guardado.
                if (document.Status > 0 && document.Status != DocumentStatusEnum.Error)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"El documento ID# {id} tiene estado [{document.Status.GetDisplayValue()}] y no puede ser modificado.", "Documento Invalido!");
                }

                // Realiza la validacion si esta activada la suscripción para los documentos borrador
                if (document.Status == DocumentStatusEnum.Draft && requestModel.Status == NewDocumentStatusEnum.Issued)
                { 
                    var subsc = _subscriptionService.GetSubscription(Issuer.RUC);
                    if (subsc != null)
                    {
                        if (subsc?.Status != SubscriptionStatusEnum.Activa)
                        {
                            throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "Suscripción inactiva", $"{subsc.StatusMsg} por el momento no puedes emitir el documento(s).");
                        }
                        else if (subsc.LicenceType.Code != Constants.PlanPro)
                        {
                            if (subsc.BalanceDocument <= 0)
                            {
                                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "Sin emisión, a llegado al limite del plan.",
                                    $"La cantidad de documentos de emisión de tu plan {subsc.LicenceType.Name} a llegado al limite, y por el momento no puedes emitir documento(s).");
                            }
                        }
                    }
                }

                // Realiza la validacion de la informacion del contribuyente
                if (!ValidateInfo(requestModel))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "Error al procesar la información del contribuyente.", "Documento Invalido!");
                }

                // Antes de proceder debemos ejecutar el procedimiento de validacion del documento
                if (!ValidateRequest(requestModel, out string errormsg))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, errormsg, "Documento Invalido!");
                }

                if (!ModelState.IsValid)
                {
                    var msg = this.GetValidationErrors();
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, msg, "Los datos ingresados son incorrectos");
                }
               
                // Para poder emitir un documento es necesario que el usuario
                // tenga un certificado con su clave configurado en el sitio web, 
                // caso contrario no le permite emitir.
                if ((requestModel.Status == NewDocumentStatusEnum.Issued) && (string.IsNullOrEmpty(Issuer.Certificate) || string.IsNullOrEmpty(Issuer.CertificatePass)))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "No se puede enviar el documento al SRI, debido a que usted no ha configurado su firma electrónica.");
                }
                if ((requestModel.Status == NewDocumentStatusEnum.Issued) && (Issuer.CertificateExpirationDate < DateTime.Now))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "Error al enviar el Documento", "Atención: No puedes emitir documentos, tu firma electrónica está caducada, debes solicitarla pronto, si la renovaste no te olvides de configurarla");
                }
                // Ejecucion del proceso de verificar el detalle del documento
                if (document.DocumentTypeCode == DocumentTypeEnum.Invoice.GetCoreValue() ||
                    document.DocumentTypeCode == DocumentTypeEnum.PurchaseSettlement.GetCoreValue() ||
                    document.DocumentTypeCode == DocumentTypeEnum.CreditNote.GetCoreValue() ||
                    document.DocumentTypeCode == DocumentTypeEnum.DebitNote.GetCoreValue())
                {
                    var doc = _documentsService.DetailsDependants(document);
                    document = doc.Entity ?? document;
                }

                // cunado el documento tiene error y se envia nuevamente se asigna la clave de acceso al número de autorización
                if (document.Status == DocumentStatusEnum.Error && requestModel.Status == NewDocumentStatusEnum.Issued)
                {
                    if (!string.IsNullOrWhiteSpace(document.AccessKey))
                    {
                        document.AuthorizationNumber = document.AccessKey;
                    }
                }

                ChangeDocument(document, requestModel);

                //document.Status = (DocumentStatusEnum)Convert.ToInt16(requestModel.Status);               

                // Ejecucion del proceso de grabacion del documento
                var result = _documentsService.UpdateDocument(document);

                if (!result.IsSuccess)
                {
                    throw Request.BuildHttpErrorException(result.StatusCode, result.DevMessage, result.UserMessage);
                }

                //eliminar cuando se actualice la app
                if (result.Entity.Status == DocumentStatusEnum.Authorized)
                {
                    result.Entity.Status = DocumentStatusEnum.Validated;
                }

                // Devuelvo el objeto como Json Serializado
                return result.Entity;
            }
            catch (HttpResponseException ex)
            {
                Logger.Log($"LOG.{Issuer.RUC}.DOCUMENT.ERROR", "Exception: ", ex);

                throw ex;
            }
            catch (Exception ex)
            {
                Logger.Log($"LOG.{Issuer.RUC}.DOCUMENT.ERROR", "Exception: ", ex);
                
                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, ex.ToString(), "Error al guardar el documento!");
            }
        }

        /// <summary>
        /// Proceso encargado para la sincronización de los documentos recibidos
        /// </summary>       
        /// <returns></returns>
        protected HttpResponseMessage SyncUp()
        {
            try
            {
                var ruc = Issuer.RUC.Substring(0, 10);
                var yearMonth = $"{DateTime.Now:yyyyMM}";
                var date = DateTime.Now;
                var _status = "INI";

                var id = _appService.SyncUpReceived(ruc, yearMonth, date, _status);
                if(id > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, $"Sincronización iniciada con identificador {id}");
                }

                return Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, $"Error al iniciar sincronización");
            }
            catch (HttpResponseException ex)
            {
                return ex.Response;
            }
            catch (Exception ex)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, $"{ex.Message} {ex.InnerException?.Message}", $"Hubo un error al procesar la solicitud: {ex.Message}");
            }

        }


        protected HttpResponseMessage Download(string id, string reportType)
        {
            try
            {
                var file = string.Empty;
                var filename = string.Empty;

                if (!string.IsNullOrEmpty(id))
                {
                    Document doc;
                    if (id.Length > 40) // Es Numero de Autorizacion o Clave de Acceso
                    {
                        doc = _documentsService.GetAllIssuerDocuments(Issuer.Id)?.FirstOrDefault(m => m.AccessKey == id || m.AuthorizationNumber == id);
                    }
                    else if (id.Length == 17 && id.Contains("-")) // Es numero de Documento
                    {
                        doc = GetDocument(id);
                    }
                    else // Es un numero de ID Normal
                    {
                        var idValue = Convert.ToInt64(id);
                        doc = GetDocumentById(idValue);
                    }

                    // Validamos si se encontró el documento
                    if (doc != null)
                    {
                        filename = $"{Convert.ToString(doc.GetDocumentType())}-{doc.RUC}-{doc.DocumentNumber}";
                        ReportResult report = null;
                        var metPay =_catalogsService.GetPaymentMethods().Where(o => o.IsEnabled).ToList().Select(pym => pym.ToPaymentMethodDto());
                        if(doc.SettlementInfo?.Payments.Count > 0){
                            doc.SettlementInfo.Payments.ForEach(m => { m.Name = metPay.Where(o => o.SriCode == m.PaymentMethodCode).First().Name;});
                        }
                        else if (doc.InvoiceInfo?.Payments.Count > 0){
                            doc.InvoiceInfo.Payments.ForEach(n => { n.Name = metPay.Where(a => a.SriCode == n.PaymentMethodCode).First().Name; });
                        }
                        else if (doc.CreditNoteInfo?.Payments.Count > 0){
                            doc.CreditNoteInfo.Payments.ForEach(n => { n.Name = metPay.Where(a => a.SriCode == n.PaymentMethodCode).First().Name; });
                        }
                        else if (doc.DebitNoteInfo?.Payments.Count > 0)
                        {
                            doc.DebitNoteInfo.Payments.ForEach(n => { n.Name = metPay.Where(a => a.SriCode == n.PaymentMethodCode).First().Name; });
                        }

                        if (reportType.ToLower() == "xml")
                        {
                            if (!string.IsNullOrEmpty(doc.AccessKey))
                            {
                                var prefix = doc.GetDocumentType()
                                                .GetPrefixValue();
                                
                                file = $"{prefix}{doc.AccessKey}.xml";
                                var source = Path.Combine(Constants.XMLFilesLocation, "autorizados", file);
                                if (!File.Exists(source))
                                {             
                                    source = Path.Combine(Constants.XMLFilesLocation, "firmados", file);
                                    if (!File.Exists(source))
                                    {
                                        source = Path.Combine(Constants.XMLFilesLocation, "original", file);
                                        if (!File.Exists(source))
                                        {
                                            source = Path.Combine(Constants.XMLFilesLocation, "autorizados2", file);
                                            if (!File.Exists(source))
                                            {
                                                source = Path.Combine(Constants.XMLFilesLocation, "autorizados3", file);
                                                if (!File.Exists(source))
                                                {
                                                    source = Path.Combine(Constants.XMLFilesLocation, "autorizados4", file);
                                                    if (!File.Exists(source))
                                                    {
                                                        source = Path.Combine(Constants.XMLFilesLocation, "autorizados5", file);
                                                        if (!File.Exists(source))
                                                        {
                                                            source = Path.Combine(Constants.XMLFilesLocation, "autorizados7", file);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (File.Exists(source))
                                {
                                    report = new ReportResult(File.ReadAllBytes(source))
                                    {
                                        Encoding = $"{Encoding.UTF8}",
                                        MimeType = "text/xml",
                                        Extension = "xml"
                                    };
                                }
                            }
                        }
                        else
                        {
                            report = doc.GetReportBuilder(Issuer.ToIssuer(), reportType);
                        } 

                        if (report != null)
                        {
                            // Ahora generamos el archivo segun el tipo especificado:
                            HttpResponseMessage result = this.Request.CreateResponse(HttpStatusCode.OK);
                            var stream = new MemoryStream(report.Content);
                            result.Content = new StreamContent(stream);
                            result.Content.Headers.ContentType = new MediaTypeHeaderValue(report.MimeType);
                            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = $"{filename}.{report.Extension}"
                            };

                            return result;
                        }
                    }
                }

                return Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, $"No se encontró el archivo {file} [{filename}]");
            }
            catch (HttpResponseException ex)
            { 
                return ex.Response;
            }
            catch (Exception ex)
            {  
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, $"{ex.Message} {ex.InnerException?.Message}",$"Hubo un error al procesar el documento: {ex.Message}");
            }

        }


        /// <summary>
        /// Obtiene el objeto System.Web.HttpRequest de la solicitud HTTP actual
        /// </summary>
        /// <returns>Objeto System.Web.HttpRequest de la solicitud HTTP actual</returns>
        /// <exception cref="System.Web.HttpException">La aplicación Web se está ejecutando bajo IIS 7 en modo integrado.</exception>
        public new HttpRequestMessage Request
        {
            get { return base.Request ?? new HttpRequestMessage(); }
        }

        /// <summary>
        /// Realiza la validacion del contribuyente principal del documento
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected bool ValidateInfo(TRequestModel requestModel)
        {
            Contributor contributor = _contributorsService.GetFinalConsumer();           
            
            if (requestModel.ContributorId == 0)
            {
                // Valida si el contribuyente existe :
                // como dato adicional valida si es consumidor final cuando todos los numeros son 9. ej: 9999999999999
                if (!string.IsNullOrEmpty(requestModel.Identification) && !requestModel.Identification.All(c => c == '9'))
                {

                    // VALIDAMOS EL RUC: Es uno de los tipos mas exigentes para el SRI.
                    if (requestModel.Identification.Length == 13 && requestModel.Identification.EndsWith("001"))
                    {
                        // Es RUC: 04 -- 
                        // Debemos corregir esto porque la informacion muchas veces llega mal
                        requestModel.IdentificationType = "04";
                    }

                    contributor = _contributorsService.GetContributorByRUC(requestModel.Identification, Issuer.Id);

                    if (contributor == null)
                    {
                        // Si el cliente no existe lo genera automaticamente en el sistema

                        // Como requisito primordial es el nombre del cliente
                        if (string.IsNullOrEmpty(requestModel.ContributorName))
                        {
                            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "Debe especificar la informacion del Cliente: RAZON SOCIAL O NOMBRE", "Error al guardar la informacion del cliente en el Documento.");
                        }
                         
                        // Verificamos el tipo de identificacion
                        var contribIdType = _catalogsService.GetIdentificationTypes().FirstOrDefault(p => p.SriCode == requestModel.IdentificationType);

                        if (contribIdType == null)
                        {
                            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "Debe especificar la informacion del Cliente: TIPO DE IDENTIFICACION", "Error al guardar la informacion del cliente en el Documento.");
                        }

                        // Crear Contribuyente:
                        contributor = new Contributor
                        {
                            Id = 0,
                            Identification = requestModel.Identification,
                            BussinesName = requestModel.ContributorName,
                            TradeName = requestModel.ContributorName,
                            Phone = requestModel.Phone ?? "2999999",
                            Address = requestModel.Address ?? "Av. Principal",
                            IssuerId = Issuer.Id,
                            IdentificationTypeId = contribIdType.Id,
                            EmailAddresses = requestModel.EmailAddresses ?? Issuer.Email ?? "",
                            IsCustomer = (DocumentType != DocumentTypeEnum.RetentionReceipt.GetCoreValue()),
                            IsSupplier = (DocumentType == DocumentTypeEnum.RetentionReceipt.GetCoreValue())
                        };

                        //Realizamos el proceso de guardado del contribuyente:
                        var result = _contributorsService.Add(contributor);

                        if (result != null)
                        {
                            contributor = result.Entity;
                        }
                    }
                }
                else
                {
                    if (AllowFinalConsumer)
                    {

                        // Cuando no se especifica la informacion del cliente se factura como Consumidor Final.
                        contributor = _contributorsService.GetFinalConsumer();
                    }
                    else
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "Debe especificar la informacion del Cliente: NUMERO DE IDENTIFICACION!", "Error al guardar la informacion del cliente en el Documento.");
                    }
                }
            }
            else
            {
                contributor = _contributorsService.GetContributorById(requestModel.ContributorId, Issuer.Id);
                // Verificamos el tipo de identificacion

                if (contributor == null)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "El contribuyente especificado no existe!", "Error al guardar la informacion del cliente en el Documento.");
                }
            }  

            contributor.IdentificationType = _catalogsService.GetIdentificationTypes().FirstOrDefault(p => p.Id == contributor.IdentificationTypeId);

            // Si todo esta bien entonces empiezo a llenar la informacion que sea necesaria en el modelo
            requestModel.ContributorId = contributor.Id;
            requestModel.IdentificationType = contributor.IdentificationType.SriCode;
            requestModel.Identification = contributor.Identification;
            requestModel.ContributorName = contributor.BussinesName;
            requestModel.Address = string.IsNullOrEmpty(requestModel.Address) ? contributor.Address : requestModel.Address;
            requestModel.Phone = string.IsNullOrEmpty(requestModel.Phone) ? contributor.Phone : requestModel.Phone;
            requestModel.EmailAddresses = string.IsNullOrEmpty(requestModel.EmailAddresses) ? contributor.EmailAddresses : requestModel.EmailAddresses;

            if (string.IsNullOrEmpty(requestModel?.IssuedOn?.Trim()) || requestModel.IssuedOn.Contains("0001"))
            {
                requestModel.IssuedOn = DateTime.Now.ToString("dd/MM/yyyy");
            }

            // Al realizar la validacion se actualiza los valores predeterminados con los correctos:
            requestModel.Currency = string.IsNullOrEmpty(requestModel.Currency) ? DEFAULT_CURRENCY : requestModel.Currency;
            requestModel.Reason = string.IsNullOrEmpty(requestModel.Reason) ? "**DOCUMENTO GENERADO POR ECUAFACT EXPRESS**" : requestModel.Reason;

            
            
            // Se genero o no el procesamiento
            return contributor != null;
        }


        /// <summary>
        /// Realiza la validacion cuando es consumidor final valida el monto total para la emisión
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected bool ValidateAmount(TRequestModel requestModel)
        {
            var result = true;
            if (requestModel.ContributorId == 0)
            {
                // Valida si el contribuyente existe :
                // como dato adicional valida si es consumidor final cuando todos los numeros son 9. ej: 9999999999999
                if (!string.IsNullOrEmpty(requestModel.Identification) && requestModel.Identification.All(c => c == '9'))
                {
                    if (!Issuer.IsCooperativeCarrier && !Issuer.IsCarrier)
                    {
                        if (requestModel.Total > 50)
                        {
                            if (DocumentType == DocumentTypeEnum.Invoice.GetCoreValue() || DocumentType == DocumentTypeEnum.PurchaseSettlement.GetCoreValue())
                            {
                                result = false;
                                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"El valor de la {(DocumentType == DocumentTypeEnum.Invoice.GetCoreValue() ? "Factura" : "Liquidación de compra")} supera los 50 dólares, por lo tanto no puede ser emitida al consumidor final, debe especificar un cliente.", "Error al guardar  el Documento.");
                            }
                        }
                    }                            
                }              
            }   
            
            return result;
        }


        /// <summary>
        /// Proceso para la Generacion del Documento
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected abstract Document GenerateDocument(TRequestModel requestModel);

        /// <summary>
        /// Actualizar un documento existente
        /// </summary>
        /// <param name="document">Documento a modificar</param>
        /// <param name="requestModel">Datos que se deben aplicar</param>
        /// <returns></returns>
        protected abstract Document ChangeDocument(Document document, TRequestModel requestModel);
               

        /// <summary>
        /// Valida los datos del requerimiento
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        protected virtual bool ValidateRequest(TRequestModel requestModel, out string errormsg)
        {
            // Validamos las referencias
            if (requestModel != null  &&
                DocumentType != DocumentTypeEnum.Invoice.GetCoreValue() &&  // Si es una factura entonces no debe validarse:
                requestModel is ReferencedDocumentRequestBase)
            {
                if (!ValidateDocumentReference(requestModel as ReferencedDocumentRequestBase, out errormsg))
                {
                    return false;
                }
            }


            ModelState.Clear();
            Configuration = GlobalConfiguration.Configuration ?? new HttpConfiguration();

            // Revalidamos el modelo de datos
            Validate<TRequestModel>(requestModel);
            

            errormsg = $"{GetValidationErrors()}";

            // Si no hay errores entonces es valido el request 
            return string.IsNullOrEmpty(errormsg);
        }

        /// <summary>
        /// Valida el documento de soporte o referencia.
        /// Por el momento su uso es para asegurarnos de tener los datos del documento correcto.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        protected bool ValidateDocumentReference(ReferencedDocumentRequestBase requestModel, out string errormsg)
        {
            
            
            Document refDoc = null;
            // Si no existe la referencia directa se usa los datos enviados por el usuario
            if (requestModel.ReferenceDocumentId > 0)
            {
                // Si es nota de credito entonces se busca dentro de los documentos emitidos, 
                // de otra forma no realiza ningun proceso
                if (typeof(CreditNoteRequestModel).IsInstanceOfType(requestModel))
                {
                    refDoc = _documentsService.GetIssuerDocumentById(Issuer.Id, requestModel.ReferenceDocumentId.Value);
                }

            }
            else
            {
                if (string.IsNullOrEmpty(requestModel.ReferenceDocumentCode) ||
                    string.IsNullOrEmpty(requestModel.ReferenceDocumentNumber) ||
                    string.IsNullOrEmpty(requestModel.ReferenceDocumentDate))
                {
                    errormsg = "Debe especificar la informacion de la referencia para este documento!";
                    return false;
                }
                else
                {
                    var refDocCode = requestModel.ReferenceDocumentCode;
                    if (refDocCode.Length<2)
                    {
                        short refCodeInt = 1;
                        short.TryParse(refDocCode, out refCodeInt);
                        // DE FORMA PREDETERMINADA SON FACTURAS
                        var docTypeCode = _catalogsService.GetDocumentTypes().FirstOrDefault(m => m.Id == refCodeInt)?.SriCode ?? "01"; 
                        requestModel.ReferenceDocumentCode = docTypeCode;
                    } 

                    if (typeof(CreditNoteRequestModel).IsInstanceOfType(requestModel))
                    {
                        var docNum = requestModel.ReferenceDocumentNumber.Split('-');

                        if (docNum.Length != 3)
                        {
                            errormsg = $"El numero de documento {requestModel.ReferenceDocumentNumber} no existe o es incorrecto!";
                            return false;
                        }
                        else
                        {
                            refDoc = _documentsService.GetIssuerDocument(Issuer.Id, requestModel.ReferenceDocumentCode, docNum[2], docNum[0], docNum[1]);
                        }
                    }
                }
            }

            if (refDoc != null)
            {
                // VALIDAMOS SI EL DOCUMENTO PERTENECE A NUESTRO CLIENTE:
                if (refDoc.ContributorId != requestModel.ContributorId)
                {
                    errormsg = $"El documento # {requestModel.ReferenceDocumentNumber}, no pertenece al cliente {requestModel.Identification} {requestModel.ContributorName} !";
                    return false;
                }


                //ESTO ACTUALIZA EL DOCUMENTO CON LOS DATOS DE REFERENCIA CORRECTOS
                requestModel.ReferenceDocumentCode = refDoc.DocumentTypeCode ?? requestModel.ReferenceDocumentCode;
                requestModel.ReferenceDocumentNumber = refDoc.DocumentNumber ?? requestModel.ReferenceDocumentNumber;
                requestModel.ReferenceDocumentDate = refDoc.IssuedOn.ToString("dd/MM/yyyy");
                requestModel.ReferenceDocumentAuth = refDoc.AuthorizationNumber ?? requestModel.ReferenceDocumentAuth;
            }

            errormsg = "OK";
            return true;
        }

        /// <summary>
        /// Devuelve un Documento por su ID Unico
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected Document GetDocumentById(long id)
        {
            var issuer = this.Issuer;

            var document = _documentsService.GetIssuerDocumentById(Issuer.Id, id);

            if (document == null)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
            }

            return document;
        }

        IQueryable<IdentificationType> identificationTypes;

        /// <summary>
        /// Valida si el tipo de identificación es valido
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool IdenfiticationTypeIsValid(string type)
        {
            if (identificationTypes == null)
            {
                identificationTypes = _catalogsService.GetIdentificationTypes();
            }

            return identificationTypes.Any(x => x.SriCode == type);
        }

        /// <summary>
        /// Valida los Detalles de los documentos que usan productos
        /// </summary>
        /// <param name="documentDetails"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        protected bool ValidateDetails(IEnumerable<DocumentDetailModel> documentDetails, out string errormsg)
        {
            // VALIDACION DE LOS DETALLES DEL DOCUMENTO
            foreach (var detailModel in documentDetails)    
            {
                Product product = null;
                // Si el id del producto es nulo, entonces se tiene que validar la informacion enviada:
                if (detailModel.ProductId == 0)
                {
                    // Primero: Busca el producto por el codigo:
                    product = _productsService.GetProductByCode(detailModel.MainCode, Issuer.Id);

                    // Si no se encuentra por codigo
                    if (product == null || string.IsNullOrEmpty(detailModel.MainCode))
                    {
                        // Segundo: Busca el producto por descripcion:
                        product = _productsService.SearchProducts(detailModel.Description ?? "-", Issuer.Id).FirstOrDefault();
                    }

                    // Si se especifica el nombre del producto y el producto no existe, 
                    // entonces se lo tiene que generar automaticamente
                    if (product == null)
                    {
                        if (!string.IsNullOrEmpty(detailModel.Description))
                        {
                            // Si se especifico los datos especificos del producto entonces se lo crea automaticamente
                            product = _productsService.AddProduct(new Product
                            {
                                MainCode = detailModel.MainCode,
                                AuxCode = detailModel.AuxCode,
                                Name = detailModel.Description,
                                IssuerId = Issuer.Id,
                                UnitPrice = 0M,
                                IsEnabled = true,
                                ProductTypeId = 1,
                                IvaRateId = 1,
                                IceRateId = 1
                            }).Entity;
                        }
                        else
                        {
                            errormsg = $"No se ha especificado la descripcion del producto: {JsonConvert.SerializeObject(detailModel)}";
                            return false;
                        }
                    }

                    
                }
                else
                {
                    product = _productsService.GetIssuerProduct(detailModel.ProductId, Issuer.Id);
                }

                // Si todo va bien tiene que completar la informacion del producto en el documento
                if (product != null)
                {
                    detailModel.ProductId = product.Id;
                    detailModel.Description = string.IsNullOrEmpty(detailModel.Description) ? product.Name : detailModel.Description;
                    detailModel.MainCode = string.IsNullOrEmpty(detailModel.MainCode) ? product.MainCode : detailModel.MainCode;
                    detailModel.AuxCode = string.IsNullOrEmpty(detailModel.AuxCode) ? product.AuxCode : detailModel.AuxCode;
                }
                else
                {
                    errormsg = $"No se ha especificado los datos del producto: {JsonConvert.SerializeObject(detailModel)}";
                    return false;
                }


                // PROCESO DE VERIFICACION DE LOS DETALLES DE LOS IMPUESTOS
                // EN EL CASO DE QUE ESTOS NO EXISTAN PERO SE ENVIA LA INFORMACION
                if (detailModel.Taxes == null || detailModel.Taxes.Count == 0)
                {
                    detailModel.Taxes = new List<TaxModel>();

                    
                    if (string.IsNullOrEmpty(detailModel.ValueAddedTaxCode))
                    {
                        detailModel.ValueAddedTaxCode = "0";
                    }
                     
                    // Agregamos el Valor para el Impuesto IVA
                    var imp = _catalogsService.GetVatRates().FirstOrDefault(model => model.SriCode == detailModel.ValueAddedTaxCode);

                    if (imp == null)
                    {
                        imp = new VatRate { Id = 0, SriCode = detailModel.ValueAddedTaxCode, RateValue = 0M, Name = "0%" };
                    }
                    else
                    {
                        if (imp.RateValue > 0 && detailModel.ValueAddedTaxValue == 0)
                        {
                            detailModel.ValueAddedTaxValue = decimal.Round(detailModel.SubTotal * (imp.RateValue / 100), 2);
                        }
                    }

                    detailModel.Taxes.Add(new TaxModel
                    {
                        Code = "2",
                        PercentageCode = imp.SriCode,
                        Rate = imp.RateValue,
                        TaxableBase = detailModel.SubTotal,
                        TaxValue = detailModel.ValueAddedTaxValue
                    });

                    //Agregamos el ICE si esta especificado por lo menos el codigo:
                    if (!string.IsNullOrEmpty(detailModel.SpecialConsumTaxCode))
                    {
                        var ice = _catalogsService
                                    .GetIceRates()
                                    .FirstOrDefault(model => model.SriCode == detailModel.SpecialConsumTaxCode);

                        // Si no existe en nuestra tabla:
                        if (ice == null)
                        {
                            ice = new IceRate { SriCode = detailModel.SpecialConsumTaxCode, Rate = 0M, Name = "Desconocido" };
                        }

                        // Si existe el impuesto ICE Especificado
                        if ((ice.Rate ?? 0) > 0)
                        {
                            var impRate = ice.Rate.Value;
                            var impBase = (detailModel.SpecialConsumTaxValue * 100) / ice.Rate.Value;

                            detailModel.Taxes.Add(new TaxModel
                            {
                                Code = "3",
                                PercentageCode = ice.SriCode,
                                Rate = impRate,
                                TaxableBase = impBase,
                                TaxValue = detailModel.SpecialConsumTaxValue
                            });
                        }
                    }
                }

            }

            errormsg = "OK";
            return true;
        }
         
        /// <summary>
        /// Devuelve todos los documentos segun el estado
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        protected IQueryable<Document> GetDocuments(DocumentStatusEnum? status = null)
        {
            var issuer = this.Issuer;
            var documents = _documentsService.GetIssuerDocuments(issuer.Id, DocumentType, status, false);

            return documents;
        }

        /// <summary>
        /// Busca dentro de todos los documentos
        /// </summary>
        /// <param name="search">Termino de busqueda</param>
        /// <param name="startDate">Fecha de Inicio para la busqueda</param>
        /// <param name="endDate">Fecha Final de la busqueda</param>
        /// <param name="pageNumber">Numero de Pagina</param>
        /// <param name="pageSize">Filas por pagina</param>
        /// <param name="documentType">Tipo de Documento</param>
        /// <param name="status">Estado del Documento</param>
        /// <param name="includeDeleted">Mostrar Documentos Eliminados</param>
        /// <param name="selector">Convertidor de Respuestas</param>
        /// <param name="establishmentCode">Convertidor de Respuestas</param>
        /// <param name="issuePointCode">Convertidor de Respuestas</param>
        /// <returns></returns>
        protected async Task<IPagedList<T>> GetDocuments<T>(string search, DateTime? startDate, DateTime? endDate, string documentType, DocumentStatusEnum? status, int? pageNumber = 1, int? pageSize = null, bool includeDeleted = false, Func<Document, T> selector = null,
            string establishmentCode = null, string issuePointCode = null, bool invoiceOnly = false)
        {
            var issuer = this.Issuer;
            if (this?.UserRol == UserRolEnum.Cooperative)
            {
                establishmentCode = this.IssuePoint?.Establishments?.Code ?? establishmentCode;
                issuePointCode = this.IssuePoint?.Code ?? issuePointCode;
            }

            var documents = _documentsService.GetAllIssuerDocuments(issuer.Id);

            if (startDate != null)
            {
                startDate = startDate?.AddDays(-1);
            }

            if (endDate != null)
            {
                endDate = endDate?.AddDays(1);
            }

            if (status == DocumentStatusEnum.Validated)
            {
                documents = documents.Where(d => d.AuthorizationDate != null);
            }

            if (!string.IsNullOrWhiteSpace(establishmentCode) && !string.IsNullOrWhiteSpace(issuePointCode))
            {
                documents = documents.Where(d => d.EstablishmentCode == establishmentCode && d.IssuePointCode == issuePointCode);
            }

            Expression<Func<Document, bool>> where =
                model => (includeDeleted || status == DocumentStatusEnum.Revoked || status == DocumentStatusEnum.Deleted || (model.IsEnabled && model.Status >= 0))
                    && (string.IsNullOrEmpty(documentType) || model.DocumentTypeCode == documentType)
                    && ((status == null && model.Status > 0) || model.Status == status) // Si el estado es nulo, solo devuelve los emitidos. Los borradores son accesibles solo por el estado
                    && (startDate == null || model.IssuedOn > startDate)
                    && (endDate == null || model.IssuedOn < endDate)
                    && (string.IsNullOrEmpty(search) ||
                        (model.Sequential.Contains(search) ||
                        (!string.IsNullOrEmpty(model.AccessKey) && model.AccessKey.Contains(search)) ||
                        (!string.IsNullOrEmpty(model.ContributorName) && model.ContributorName.ToUpper().Contains(search)) ||
                        (!string.IsNullOrEmpty(model.ContributorIdentification) && model.ContributorIdentification.Contains(search))
                    ));

            if (invoiceOnly)
            {
                documents = documents.Where(where).OrderByDescending(p => p.IssuedOn).Include(doc => doc.InvoiceInfo);
            }
            else
            {
                documents = documents.Where(where).OrderByDescending(p => p.IssuedOn)
                    //.Include(doc => doc.InvoiceInfo)
                    .Include(doc => doc.RetentionInfo)
                    //.Include(doc => doc.CreditNoteInfo)
                    .Include(doc => doc.ReferralGuideInfo);
                    //.Include(doc => doc.SettlementInfo);
            }

            pageNumber = pageNumber ?? 1;
            pageSize = pageSize ?? 10;  //documents.Count();

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            if (selector == null)
            {
                return await documents.Select(x => ((T)Convert.ChangeType(x, typeof(T)))).ToPagedListAsync(pageNumber.Value, pageSize.Value);
            }

            var lis = await documents.Select(selector).ToPagedListAsync(pageNumber.Value, pageSize.Value);

            //return await documents.Select(selector).ToPagedListAsync(pageNumber.Value, pageSize.Value);
            //eliminar cunado el app este actualizada
            if (((X.PagedList.BasePagedList<Ecuafact.WebAPI.Domain.Entities.Document>)lis).Count > 0)
            {
                ((X.PagedList.BasePagedList<Ecuafact.WebAPI.Domain.Entities.Document>)lis).ToList().ForEach(d =>{
                    if (d.Status == DocumentStatusEnum.Authorized) {
                        d.Status = DocumentStatusEnum.Validated;
                    }
                });
            }
            return lis;
        }

        /// <summary>
        /// Busca dentro de todos los documentos
        /// </summary>
        /// <param name="search">Termino de busqueda</param>
        /// <param name="startDate">Fecha de Inicio para la busqueda</param>
        /// <param name="endDate">Fecha Final de la busqueda</param>
        /// <param name="pageNumber">Numero de Pagina</param>
        /// <param name="pageSize">Filas por pagina</param>
        /// <param name="documentType">Tipo de Documento</param>
        /// <param name="status">Estado del Documento</param>
        /// <param name="includeDeleted">Mostrar Documentos Eliminados</param>       
        /// <param name="establishmentCode">Convertidor de Respuestas</param>
        /// <param name="issuePointCode">Convertidor de Respuestas</param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> ExportDocuments(string search, DateTime? startDate, DateTime? endDate, string documentType, DocumentStatusEnum? status, int? pageNumber = 1, int? pageSize = null, bool includeDeleted = false, string establishmentCode = null, string issuePointCode = null)
        {
            try
            {
                var issuer = this.Issuer;
                var documents = _documentsService.GetAllIssuerDocuments(issuer.Id);

                if (startDate != null)
                {
                    startDate = startDate?.AddDays(-1);
                }

                if (endDate != null)
                {
                    endDate = endDate?.AddDays(1);
                }

                if (!string.IsNullOrWhiteSpace(establishmentCode) && !string.IsNullOrWhiteSpace(issuePointCode))
                {
                    documents = documents.Where(d => d.EstablishmentCode == establishmentCode && d.IssuePointCode == issuePointCode);
                }

                Expression<Func<Document, bool>> where =
                    model => (includeDeleted || status == DocumentStatusEnum.Revoked || status == DocumentStatusEnum.Deleted || (model.IsEnabled && model.Status >= 0))
                        && (string.IsNullOrEmpty(documentType) || model.DocumentTypeCode == documentType)
                        && ((status == null && model.Status > 0) || model.Status == status) // Si el estado es nulo, solo devuelve los emitidos. Los borradores son accesibles solo por el estado
                        && (startDate == null || model.IssuedOn > startDate)
                        && (endDate == null || model.IssuedOn < endDate)
                        && (string.IsNullOrEmpty(search) ||
                            (model.Sequential.Contains(search) ||
                            (!string.IsNullOrEmpty(model.AccessKey) && model.AccessKey.Contains(search)) ||
                            (!string.IsNullOrEmpty(model.ContributorName) && model.ContributorName.ToUpper().Contains(search)) ||
                            (!string.IsNullOrEmpty(model.ContributorIdentification) && model.ContributorIdentification.Contains(search))
                        ));


                var lis = await documents.Where(where).OrderByDescending(p => p.IssuedOn).Include(doc => doc.InvoiceInfo)
                    .Include(doc => doc.RetentionInfo)
                    .Include(doc => doc.CreditNoteInfo)
                    .Include(doc => doc.ReferralGuideInfo)
                    .Include(doc => doc.SettlementInfo).ToListAsync();                

                if (lis.Count > 0)
                {                   

                    // Ahora generamos el archivo segun el tipo especificado:
                    HttpResponseMessage result = this.Request.CreateResponse(HttpStatusCode.OK);
                    var stream = new MemoryStream(lis.ToExcelDocuments());
                    result.Content = new StreamContent(stream);
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/excel");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = $"documentos_emitidos_{issuer.RUC}.xlsx"
                    };

                    return result;
                }

                return Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, $"Error al descargar la exportación de documentos");
            }
            catch (HttpResponseException ex)
            {
                return ex.Response;
            }
            catch (Exception ex)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, $"{ex.Message} {ex.InnerException?.Message}", $"Hubo un error al procesar la descarga: {ex.Message}");
            }
        }

        /// <summary>
        /// Busca dentro de todos los documentos recibidos
        /// </summary>
        /// <param name="search">Termino de busqueda</param>
        /// <param name="startDate">Fecha de Inicio para la busqueda</param>
        /// <param name="endDate">Fecha Final de la busqueda</param>
        /// <param name="pageNumber">Numero de Pagina</param>
        /// <param name="pageSize">Filas por pagina</param>
        /// <param name="documentType">Tipo de Documento</param>       
        /// <param name="deductible">Deducibles</param>
        /// <returns></returns>
        protected async Task<DocumentResponse> GetDocumentsReceivedPage(string search, DateTime? startDate, DateTime? endDate, string documentType, string deductible, int? pageNumber = 1, int? pageSize = null)
        {
            var msj = "";
            try
            {
                var issuer = this.Issuer;
                deductible = deductible ?? "-1";
                startDate = startDate.HasValue ? startDate : DateTime.Now;
                endDate = endDate.HasValue ? endDate : DateTime.Now.AddMonths(-1);
                var deductibleId = Convert.ToInt32(deductible);
                documentType = documentType ?? "0";
                var filter = new FilterDocumentReceived{
                    Ruc = issuer.RUC.Substring(0, 10),
                    DateStart = startDate.Value,
                    DateEnd = endDate.Value,
                    CodTypeDoc = documentType != "0" ? documentType : null,
                    PageNumber = pageNumber.Value,
                    PageSize = pageSize.Value,
                    Search = search
                };
                if (deductibleId > 0)
                {
                    filter.DeductibleId = deductibleId;
                }

                var result = await Task.FromResult(_appService.GetDocumentosReceivedPagination(filter));
                if (result.IsSuccess)
                {
                    return new DocumentResponse
                    {
                        documents = result.Entity.Data.Select(doc => doc.ToDocumentReceivedDto()).ToList(),
                        meta = new Meta{
                            count = result.Entity.TotalCount,
                            countPage = result.Entity.TotalPages,
                            currentPage = result.Entity.PageNumber
                        },
                        result = new Result{
                            code = "100",
                            message = "OK"
                        }
                    };
                }
            }
            catch (Exception ex) {

                msj = ex.Message;
            }
            return new DocumentResponse {
                documents = new List<DocumentReceived>(),
                meta = new Meta { count = 0, countPage = 0, currentPage = 0 },
                result = new Result {code = "1001", message= msj }
            };
       
        }


        /// <summary>
        /// Busca dentro de todos los documentos recibidos
        /// </summary>
        /// <param name="search">Termino de busqueda</param>
        /// <param name="startDate">Fecha de Inicio para la busqueda</param>
        /// <param name="endDate">Fecha Final de la busqueda</param>
        /// <param name="pageNumber">Numero de Pagina</param>
        /// <param name="pageSize">Filas por pagina</param>
        /// <param name="documentType">Tipo de Documento</param>
        /// <param name="token">Token del emisor</param>
        /// <param name="deductible">Deducibles</param>
        /// <returns></returns>
        protected async Task<List<T>> GetDocumentsReceived<T>(string token, string search, DateTime? startDate, DateTime? endDate, string documentType, string deductible,  int? pageNumber = 1, int? pageSize = null, Func<DocumentReceived, T> selector = null)
        {
            var documents = new List<DocumentReceived>();
            try
            {
                var clientType = "1";
                var state = "0";
                deductible = deductible ?? "-1";
                var from = startDate.HasValue ? $"{startDate:yyyy-MM-dd}" :$"{DateTime.Now:yyyy-MM-dd}";
                var to = endDate.HasValue ? $"{endDate:yyyy-MM-dd}" : $"{DateTime.Now.AddMonths(-1):yyyy-MM-dd}";
                var serviceUrl = $"{Constants.ServiceUrl}/documents?client_token={token}&from={from}&to={to}&cod_doc={documentType}&state={state}&deductible={deductible}&page_number={pageNumber}&search={search}&client_Type={clientType}";
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(serviceUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.GetContentAsync<DocumentResponse>();
                        documents = result.documents;
                    }
                    
                }
            }           
            catch (Exception ex){}

            return await Task.FromResult(documents.Select(selector).ToList());
        }

        /// <summary>
        /// Busca dentro de todos los documentos recibidos
        /// </summary>
        /// <param name="search">Termino de busqueda</param>
        /// <param name="startDate">Fecha de Inicio para la busqueda</param>
        /// <param name="endDate">Fecha Final de la busqueda</param>       
        /// <param name="documentType">Tipo de Documento</param>       
        /// <param name="deductible">Deducibles</param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> ExportReceiveds(string search, DateTime? startDate, DateTime? endDate, string documentType, string deductible)
        {
           
            try
            {
                var issuer = this.Issuer;
                deductible = string.IsNullOrWhiteSpace(deductible) ? "-1": deductible;
                startDate = startDate.HasValue ? startDate : DateTime.Now;
                endDate = endDate.HasValue ? endDate : DateTime.Now.AddMonths(-1);
                var deductibleId = Convert.ToInt32(deductible);
                documentType = documentType ?? "0";
                var filter = new FilterDocumentReceived
                {
                    Ruc = issuer.RUC.Substring(0, 10),
                    DateStart = startDate.Value,
                    DateEnd = endDate.Value,
                    CodTypeDoc = documentType != "0" ? documentType : null                    
                };
                if (deductibleId > 0)
                {
                    filter.DeductibleId = deductibleId;
                }

                var request = await Task.FromResult(_appService.GetDocumentosReceived(filter));
                if (request.IsSuccess)
                {
                    var _receiveds = request.Entity.ToList();

                    // Ahora generamos el archivo segun el tipo especificado:
                    HttpResponseMessage result = this.Request.CreateResponse(HttpStatusCode.OK);
                    var stream = new MemoryStream(_receiveds.ToExcelDocumentsReceived());
                    result.Content = new StreamContent(stream);
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/excel");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = $"documentos_recibidos_{issuer.RUC}.xlsx"
                    };

                    return result;
                }

                return Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, $"Error al descargar la exportación de documentos recibidos");
            }
            catch (HttpResponseException ex)
            {
                return ex.Response;
            }
            catch (Exception ex)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, $"{ex.Message} {ex.InnerException?.Message}", $"Hubo un error al procesar la descarga: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtener Documento recibido
        /// </summary>
        /// <param name="id">Identificador del documento</param>    
        /// <returns></returns>
        protected async Task<HttpResponseMessage> GetDocumentReceivedById(long id)
        {
            try
            {
                var result = await Task.FromResult(_appService.GetDocumentosReceivedById(id));
                if (result.IsSuccess)
                {
                    return Request.CreateResponse<DocumentReceived>(HttpStatusCode.OK, result.Entity.ToDocumentReceivedDto());
                }                

                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
            }
            catch (Exception ex)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), $"Hubo un error al buscar el documento con el valor {id}!");
            }
        }

        /// <summary>
        /// Obtener Documento recibido
        /// </summary>
        /// <param name="id">Identificador del documento</param>    
        /// <returns></returns>
        protected OperationResult DeleteDocumentReceivedById(long id)
        {
          return _appService.CancelDocument(id);                
        }

        /// <summary>
        /// Obtener Documento recibido
        /// </summary>
        /// <param name="id">Identificador del documento</param>       
        /// <param name="token">Token del emisor</param>       
        /// <returns></returns>
        protected async Task<HttpResponseMessage> GetDocumentReceivedDetailById(string token, long id)
        {
            try
            {
                var serviceUrl = $"{Constants.ServiceUrl}/documents/{id}/detail?client_token={token}&client_type=1";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(serviceUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.GetContentAsync<DocumentResponseDetail>();
                        var document = result.document;
                        return Request.CreateResponse<DocumentReceived>(HttpStatusCode.OK, document);
                    }
                }

                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
            }
            catch (Exception ex) {

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), $"Hubo un error al buscar el documento con el valor {id}!");
            }

        }


        /// <summary>
        /// Obtener Documento recibido
        /// </summary>
        /// <param name="id">Identificador del documento</param> 
        /// <returns></returns>
        protected async Task<HttpResponseMessage> GetDocumentReceivedDetailById(long id)
        {
            try
            {
                var result = await Task.FromResult(_appService.GetDocumentReceivedDetailById(id, null));
                if(result.IsSuccess)
                {
                    var doc = new DocumentResponseDetail
                    {
                        document = result.Entity.ToDocumentResponseDetail(),
                        result = new Result{
                            code = "200",
                            message = "Ok"
                        }
                    };
                    return Request.CreateResponse<DocumentResponseDetail>(HttpStatusCode.OK, doc);
                }

                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
            }
            catch (Exception ex)
            {

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), $"Hubo un error al buscar el documento con el valor {id}!");
            }

        }

        /// <summary>
        /// Tipo sustento del documento
        /// </summary>
        /// <param name="id">Identificador del documento</param>       
        /// <param name="supportTypeCode">código del tipo sustento</param>   
        /// <param name="emissionType">tipo de emisión</param>  
        /// <returns></returns>
        protected async Task<HttpResponseMessage> SustenanceTypeDocument(long id, string supportTypeCode, int emissionType)
        {
            try
            {
                var result = await Task.FromResult(_appService.AssignSupportType(id, supportTypeCode, emissionType));
                if (result)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, $"Se clasificó el tipo sustento del de docuemnto {id} con éxito.");
                }

                return Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, $"No se casificó el tipo sustento del documento");
            }
            catch (Exception ex)
            {

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), $"Hubo un error al clasificar el tipo sustento del documento {id}!");
            }

        }

        /// <summary>
        /// Procesar el xml documnetos emitido
        /// </summary>
        /// <param name="id">Identificador del documento</param>      
        /// <param name="token">token del usuario</param>  
        /// <returns></returns>
        protected async Task<HttpResponseMessage> ProcesDocumentXmlById(string token, long id)
        {
            try
            {
                var xml = _documentsService.GetXmlDocument(id);               
                if (!string.IsNullOrWhiteSpace(xml))
                {
                    var xmlBase64 = "";
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    var settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = Encoding.UTF8;
                    using (var ms = new MemoryStream())
                    {                       
                        using (XmlWriter writer = XmlWriter.Create(ms, settings))
                        {
                            doc.Save(writer);
                        }
                        xmlBase64 = Convert.ToBase64String(ms.ToArray());
                    }
                    //se envia el documento en formato xml para ser procesado
                    if (!string.IsNullOrWhiteSpace(xmlBase64))
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            var request = new
                            {
                                Token = token,
                                FileName = $"{id}.xml",
                                File = xmlBase64,
                                client_token = token
                            };
                            var response = client.PostAsJsonAsync($"{Constants.ServiceIssueUrl}/documents/sendXMLInvoice", request).Result;                         
                            if (response.IsSuccessStatusCode)
                            {
                                var model = response.GetContent<SendXMLInvoiceResponse>();
                                if (model != null)
                                {
                                    if (model.Result.code == "100")
                                    {
                                        return Request.CreateResponse<DocumentReceived>(HttpStatusCode.OK, model.Document);
                                    }
                                    else {
                                        return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, model.Result.message);
                                    }
                                }
                            }
                        }
                    }                    
                }

                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento Id # {id} solicitado.", "No existe el documento!");
            }
            catch (Exception ex)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), $"Hubo un error al generar el xml del documento Id # {id}!");
            }

        }

        /// <summary>
        /// Este metodo procesa los documentos de la consulta y actualiza el estado en la base de datos antes de ser ejecutados
        /// </summary>
        /// <param name="documents"></param>
        private void ValidateDocuments(IQueryable<Document> documents)
        {
            var pending = documents.Where(p => string.IsNullOrEmpty(p.AuthorizationDate) || p.AuthorizationDate == "-").Select(x => x.Id).ToList();

            foreach (var id in pending)
            {
                DocumentStatusInfo auth = _engineService.GetDocumentInfo(id);

                if (auth != null)
                {
                    _documentsService.UpdateDocumentStatus(Issuer.Id, id, auth);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messages"></param>
        protected void Log(params object[] messages)
        {
            Logger.Log($"DOCUMENT.{Issuer?.RUC}.ERROR", messages);
        }

        /// <summary>
        /// Devuelve el mensaje con todos los errores de validacion existentes
        /// </summary>
        /// <returns></returns>
        protected string GetValidationErrors()
        {
            // SI HUBO UN ERROR DE VALIDACION:
            var text = new StringBuilder();

            if (!ModelState.IsValid)
            {
                text.AppendLine("Error de validacion:");
                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        text.AppendLine($"{item.Key} ({item.Value.Value?.AttemptedValue}): {error.ErrorMessage}.  {error.Exception}");
                    }
                }
            }
            return text.ToString();
        }

        /// <summary>
        /// Devuelve el archivo de la nota de venta
        /// </summary>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> DownloadSaleNoteFile(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var doc = await Task.FromResult(_appService.GetDocumentosReceivedById(Convert.ToInt64(id)));
                    if (doc.IsSuccess)
                    {
                        if (!string.IsNullOrWhiteSpace(doc.Entity.Pdf))
                        {
                            var source = Path.Combine(Constants.PDFSaleNoteFilesLocation, doc.Entity.Pdf);
                            if (File.Exists(source))
                            {
                                var fileTypes = doc.Entity.Pdf.Split('.');
                                var _fileType = "pdf";
                                if(fileTypes.Length > 1)
                                {
                                    _fileType = fileTypes[1];
                                }
                                var _file = File.ReadAllBytes(source);
                                HttpResponseMessage result = this.Request.CreateResponse(HttpStatusCode.OK);
                                var stream = new MemoryStream(_file);
                                result.Content = new StreamContent(stream);
                                result.Content.Headers.ContentType = new MediaTypeHeaderValue(_fileType.Equals("pdf") ? "application/pdf" : "image/jpeg");
                                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                                {
                                    FileName = doc.Entity.Pdf
                                };
                                return result;
                            }
                            
                        }
                    }
                }
                return Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, $"No se encontró el archivo de la nota de venta");
            }
            catch (HttpResponseException ex)
            {
                return ex.Response;
            }
            catch (Exception ex)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, $"{ex.Message} {ex.InnerException?.Message}", $"Hubo un error al procesar el documento: {ex.Message}");
            }
        }

        protected static byte[] GetDocumentFile(Document doc, string extension)
        {
            if (!string.IsNullOrEmpty(doc.AccessKey))
            {
                var prefix = doc.GetDocumentType()
                                .GetPrefixValue();

                var file = $"{prefix}{doc.AccessKey}.{extension}";

                var source = Path.Combine(Constants.XMLFilesLocation, "autorizados", file);

                if (!File.Exists(source))
                {
                    source = Path.Combine(Constants.XMLFilesLocation, "firmados", file);

                    if (!File.Exists(source))
                    {
                        source = Path.Combine(Constants.XMLFilesLocation, "original", file);
                    }
                }

                if (File.Exists(source))
                {
                    return File.ReadAllBytes(source);
                }
            }

            return default;
        }

        private string SaveFile(string fname, byte[] file, string path = null)
        {

            try
            {
                if (path == null)
                {
                    path = Constants.PDFSaleNoteFilesLocation;
                }
                // Si es un subdirectorio de recursos entonces lo configuramos:
                else if (!path.Contains("\\") && !path.Contains("/"))
                {
                    path = Path.Combine(Constants.ResourcesLocation, path);
                }

                if (!string.IsNullOrEmpty(fname))
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var filename = Path.Combine(path, fname);

                    if (file != null && file.Length > 0)
                    {                        
                        File.WriteAllBytes(filename, file);
                    }

                    // Si se logro guardar el archivo se devuelve el nombre del archivo:
                    return fname;
                }
            }
            catch (Exception) { }

            return default;
        }

        /// <summary>
        /// Devuelve el emisor Actual
        /// </summary>
        /// <returns></returns>
        protected IssuerDto Issuer
        {
            get
            {
                __issuer = HttpContext.Current.Session.GetAuthenticatedIssuerSession();
                
                return __issuer;
            }
        }

        IssuerDto __issuer;

        /// <summary>
        /// Devuelve la informacion del login
        /// </summary>
        /// <returns></returns>
        protected UserRolEnum UserRol
        {
            get
            {
                __userRol = HttpContext.Current.Session.GetAuthenticatedUserRol();
                return __userRol;
            }
        }

        UserRolEnum __userRol;

        /// <summary>
        /// Devuelve el punto de establecimiento y emision
        /// </summary>
        /// <returns></returns>
        protected IssuePoint IssuePoint
        {
            get
            {
                __issuePoint = HttpContext.Current.Session.GetissuePointCode();

                return __issuePoint;
            }
        }

        IssuePoint __issuePoint;
    }
}
