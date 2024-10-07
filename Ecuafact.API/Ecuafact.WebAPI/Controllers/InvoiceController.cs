using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.Models.Dtos;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using X.PagedList;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// FACTURAS: Servicio API para la administracion de Facturas Electronicas.
    /// </summary>
    [EcuafactExpressAuthorize]
    [DisplayName("Facturación")]
    public class InvoiceController : DocumentAPIControllerBase<InvoiceRequestModel>
    {
        /// <summary>
        /// FACTURAS: Servicio API para la administracion de Facturas Electronicas.
        /// </summary>
        /// <param name="contributorsService"></param>
        /// <param name="catalogsService"></param>
        /// <param name="productsService"></param>
        /// <param name="documentsService"></param>
        /// <param name="taxesService"></param>
        /// <param name="subscriptionService"></param>
        /// <param name="appService"></param> 
        public InvoiceController(IContributorsService contributorsService, ICatalogsService catalogsService,
            IProductsService productsService, IDocumentsService documentsService, ITaxesService taxesService, ISubscriptionService subscriptionService, IAppService appService)
            : base(contributorsService, catalogsService, productsService, documentsService, taxesService, subscriptionService, appService)
        {
        }

        /// <summary>
        /// FACTURA
        /// </summary>
        protected override string DocumentType => DocumentTypeEnum.Invoice.GetCoreValue();

        /// <summary>
        ///     Obtener factura
        /// </summary>
        /// <remarks>
        ///     Devuelve una factura electronica por id o numero de documento
        /// </remarks>
        /// <param name="id">Id de la factura o numero del documento en formato XXX-XXX-XXXXXXXXX</param>
        /// <returns></returns>
        [HttpGet, Route("invoice/{id}")]
        public InvoiceDto GetInvoiceById(string id)
        {
            if (id.Contains("-"))
            {
                return GetDocument(id).ToInvoice();
            }
            else
            {
                long idValue = 0;
                if (long.TryParse(id, out idValue))
                {
                    return GetDocumentById(idValue).ToInvoice();                    
                }
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
        }

        /// <summary>
        ///     Buscar facturas
        /// </summary>
        /// <remarks>
        ///     Realiza la busqueda de las facturas usando una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de facturas</returns>
        [HttpGet, Route("invoice")]
        public async Task<IPagedList<InvoiceDto>> GetInvoices(string search = null, DateTime? startDate = null, DateTime? endDate = null, DocumentStatusEnum? status = null, int? page = 1, int? pageSize = null, bool includeDeleted = false)
        {
            return await GetDocuments(search, startDate, endDate, DocumentType, status, page, pageSize, includeDeleted, document => document.ToInvoice(),null,null, true);
        }

        /// <summary>
        ///     Crear Factura
        /// </summary>
        /// <remarks>
        ///     Permite crear una nueva factura electrónica.
        /// </remarks>
        /// <param name="requestModel">datos de la factura</param>
        /// <returns>Devuelve la factura electrónica creada.</returns>
        [HttpPost, Route("invoice")]
        [ResponseType(typeof(InvoiceDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Factura!", typeof(OperationResult))]
        public HttpResponseMessage PostInvoice(InvoiceRequestModel requestModel)
        {
            try
            {
                this.AllowFinalConsumer = true;

                var result = Save(requestModel);
                
                return Request.CreateResponse(HttpStatusCode.Created, result.ToInvoice(), JsonCustomFormatter.Default);
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

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la Factura!");
            }
        }

        /// <summary>
        ///     Actualizar factura
        /// </summary>
        /// <remarks>
        ///     Permite actualizar los datos de una factura existente, siempre y cuando no este emitida o en proceso.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <returns>La factura electronica modificada</returns>
        [HttpPut, Route("invoice/{id}")]
        [ResponseType(typeof(InvoiceDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Factura!", typeof(OperationResult))]
        public HttpResponseMessage PutInvoice(long id, InvoiceRequestModel requestModel)
        {
            try
            {
                this.AllowFinalConsumer = true;

                var result = Update(id, requestModel);

                return Request.CreateResponse(HttpStatusCode.Created, result.ToInvoice(), JsonCustomFormatter.Default);
            }
            catch (HttpResponseException ex)
            {
                Log(ex.Response, requestModel);

                return ex.Response;
            }
            catch (Exception ex)
            {
                Log("Request Message: ", Request?.Content?.ReadAsStringAsync()?.Result);
                Log(ex, requestModel);

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la Factura!");
            }
        }


        /// <summary>
        ///     Facturas por contribuyente
        /// </summary>
        /// <remarks>
        ///     Devuelve las facturas de un contribuyente especificado por una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de facturas</returns>
        [HttpGet, Route("contributors/{contributorId}/invoices")]
        public IEnumerable<InvoiceDto> GetInvoicesByContributor(long contributorId, string search = null, bool authorized = false, bool authorizeDate = false)
        {
            var issuer = this.Issuer;

            if (search == null)
            {
                search = string.Empty;
            }

            var status = authorized ? DocumentStatusEnum.Validated : (DocumentStatusEnum?)null;
            var documents = _documentsService
               .GetIssuerDocuments(issuer.Id, DocumentType, status, authorizeDate)
               .Where(doc => doc.ContributorId == contributorId &&
               (
                   search == "" ||
                   doc.ContributorName.Contains(search) ||
                   doc.ContributorIdentification.Contains(search) ||
                   doc.Sequential.Contains(search)
               ));

            return documents.ToList().Select(m => m.ToInvoice());
        }

        /// <summary>
        ///     Facturas por emisor
        /// </summary>
        /// <remarks>
        ///     Devuelve las facturas de un emisor especificado por una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de facturas</returns>
        [HttpGet, Route("invoices/issuers")]
        public async Task<IPagedList<InvoiceDto>> GetInvoicesByIssuers(string search = null, int? page = 1, int? pageSize = null, bool authorized = false, bool authorizeDate = false)
        {
            var issuer = this.Issuer;

            if (search == null)
            {
                search = string.Empty;
            }
            var documents = _documentsService.GetAllIssuerDocuments(issuer.Id);
            var status = authorized ? DocumentStatusEnum.Validated : (DocumentStatusEnum?)null;
            Expression<Func<Document, bool>> where =
                model => ((status == null && model.Status > 0) || model.Status == status)
                    && (string.IsNullOrEmpty(DocumentType) || model.DocumentTypeCode == DocumentType)
                    && (string.IsNullOrEmpty(search) ||
                        (model.Sequential.Contains(search) ||
                        (!string.IsNullOrEmpty(model.ContributorName) && model.ContributorName.ToUpper().Contains(search)) ||
                        (!string.IsNullOrEmpty(model.ContributorIdentification) && model.ContributorIdentification.Contains(search))
                    ));

            documents = documents.Where(where).OrderByDescending(p => p.IssuedOn).Include(p => p.InvoiceInfo);

            page = page ?? 1;
            pageSize = pageSize ?? 10;  //documents.Count();

            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            var lis = await documents.ToPagedListAsync(page.Value, pageSize.Value);

            return lis.Select(d => d.ToInvoice());
        }


        #region ********* Document API Overrides ********* 

        /// <summary>
        /// Proceso que interpreta los requerimientos y genera un nuevo documento adaptado al tipo
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document GenerateDocument(InvoiceRequestModel requestModel)
        {
            var document = requestModel.ToDocument(DocumentType);
            document.InvoiceInfo = document.InvoiceInfo.Build(requestModel);           
            return document;
        }

        /// <summary>
        /// Cambia la informacion de un documento con la especificada en un requerimiento.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document ChangeDocument(Document document, InvoiceRequestModel requestModel)
        {
            document = document.MapTo(requestModel, DocumentType);
            document.InvoiceInfo = document.InvoiceInfo.Build(requestModel); 
            return document;
        }

        /// <summary>
        /// Validacion de los datos del modelo de documento
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        protected override bool ValidateRequest(InvoiceRequestModel requestModel, out string errormsg)
        {
            if (!ValidateDetails(requestModel.Details, out errormsg))
            {
                return false;
            }

            errormsg = "OK";

            if (requestModel.Details == null || !requestModel.Details.Any())
            {
                errormsg = "El documento debe incluir por lo menos un detalle valido!";
                return false;
            }
              
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
                        Name = DEFAULT_PAYMENT_NAME,
                        PaymentMethodCode = DEFAULT_PAYMENT_METHOD,
                        Term = 0,
                        TimeUnit = "dias",
                        Total = requestModel.Total
                    }
                };
            }

            return base.ValidateRequest(requestModel, out errormsg);
        }

        #endregion
    }
}