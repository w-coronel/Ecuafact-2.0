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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using X.PagedList;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// Liquidación de Compras: Servicio API para la administracion de Liquidación de Compras Electronicas.
    /// </summary>
    [EcuafactExpressAuthorize]
    [DisplayName("Liquidacion de Compras")]
    public class SettlementController : DocumentAPIControllerBase<SettlementRequestModel>
    {
        /// <summary>
        /// Liquidación de Compras: Servicio API para la administracion de Liquidación de Compras Electronicas.
        /// </summary>
        /// <param name="contributorsService"></param>
        /// <param name="catalogsService"></param>
        /// <param name="productsService"></param>
        /// <param name="documentsService"></param>
        /// <param name="taxesService"></param>
        /// <param name="subscriptionService"></param>
        /// <param name="appService"></param> 
        public SettlementController(IContributorsService contributorsService, ICatalogsService catalogsService,
            IProductsService productsService, IDocumentsService documentsService, ITaxesService taxesService, ISubscriptionService subscriptionService, IAppService appService)
            : base(contributorsService, catalogsService, productsService, documentsService, taxesService, subscriptionService, appService)
        {
        }

        /// <summary>
        /// LIQUIDACION DE COMPRAS
        /// </summary>
        protected override string DocumentType => DocumentTypeEnum.PurchaseSettlement.GetCoreValue();

        /// <summary>
        ///     Obtener Liquidación de Compras
        /// </summary>
        /// <remarks>
        ///     Devuelve una Liquidación de Compras electronica por id o numero de documento
        /// </remarks>
        /// <param name="id">Id de la Liquidación de Compras o numero del documento en formato XXX-XXX-XXXXXXXXX</param>
        /// <returns></returns>
        [HttpGet, Route("settlement/{id}")]
        public SettlementDto GetSettlementById(string id)
        {
            if (id.Contains("-"))
            {
                return GetDocument(id).ToSettlement();
            }
            else
            {
                long idValue = 0;
                if (long.TryParse(id, out idValue))
                {
                    return GetDocumentById(idValue).ToSettlement();
                }
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
        }

        /// <summary>
        ///     Buscar Liquidación de Compras
        /// </summary>
        /// <remarks>
        ///     Realiza la busqueda de las Liquidación de Compras usando una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de Liquidación de Compras</returns>
        [HttpGet, Route("settlement")]
        public async Task<IPagedList<SettlementDto>> GetSettlements(string search = null, DateTime? startDate = null, DateTime? endDate = null, DocumentStatusEnum? status = null, int? page = 1, int? pageSize = null, bool includeDeleted = false)
        {
            return await GetDocuments(search, startDate, endDate, DocumentType, status, page, pageSize, includeDeleted, document => document.ToSettlement());
        }

        /// <summary>
        ///     Crear Liquidación de Compras
        /// </summary>
        /// <remarks>
        ///     Permite crear una nueva Liquidación de Compras electrónica.
        /// </remarks>
        /// <param name="requestModel">datos de la Liquidación de Compras</param>
        /// <returns>Devuelve la Liquidación de Compras electrónica creada.</returns>
        [HttpPost, Route("settlement")]
        [ResponseType(typeof(SettlementDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Liquidación de Compras!", typeof(OperationResult))]
        public HttpResponseMessage PostSettlement(SettlementRequestModel requestModel)
        {
            try
            {
                this.AllowFinalConsumer = true;

                var result = Save(requestModel);
                
                return Request.CreateResponse(HttpStatusCode.Created, result.ToSettlement(), JsonCustomFormatter.Default);
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

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la Liquidación de Compras!");
            }
        }

        /// <summary>
        ///     Actualizar Liquidación de Compras
        /// </summary>
        /// <remarks>
        ///     Permite actualizar los datos de una Liquidación de Compras existente, siempre y cuando no este emitida o en proceso.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <returns>La Liquidación de Compras electronica modificada</returns>
        [HttpPut, Route("settlement/{id}")]
        [ResponseType(typeof(SettlementDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Liquidación de Compras!", typeof(OperationResult))]
        public HttpResponseMessage PutSettlement(long id, SettlementRequestModel requestModel)
        {
            try
            {
                this.AllowFinalConsumer = true;
                var result = Update(id, requestModel);
                return Request.CreateResponse(HttpStatusCode.Created, result.ToSettlement(), JsonCustomFormatter.Default);
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

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la Liquidación de Compras!");
            }
        }


        /// <summary>
        ///     Liquidación de Compras por contribuyente
        /// </summary>
        /// <remarks>
        ///     Devuelve las Liquidación de Compras de un contribuyente especificado por una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de Liquidación de Compras</returns>
        [HttpGet, Route("contributors/{contributorId}/settlements")]
        public IEnumerable<SettlementDto> GetSettlementsByContributor(long contributorId, string search = null, bool authorized = false)
        {
            var issuer = this.Issuer;

            if (search == null)
            {
                search = string.Empty;
            }

            var status = authorized ? DocumentStatusEnum.Authorized : (DocumentStatusEnum?)null;
            var documents = _documentsService
               .GetIssuerDocuments(issuer.Id, DocumentType, status, false)
               .Where(doc => doc.ContributorId == contributorId &&
               (
                   search == "" ||
                   doc.ContributorName.Contains(search) ||
                   doc.ContributorIdentification.Contains(search) ||
                   doc.Sequential.Contains(search)
               ));

            return documents.ToList().Select(m => m.ToSettlement());
        }


        #region ********* Document API Overrides ********* 

        /// <summary>
        /// Proceso que interpreta los requerimientos y genera un nuevo documento adaptado al tipo
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document GenerateDocument(SettlementRequestModel requestModel)
        {
            var document = requestModel.ToDocument(DocumentType);
            document.SettlementInfo = document.SettlementInfo.Build(requestModel);
            return document;
        }

        /// <summary>
        /// Cambia la informacion de un documento con la especificada en un requerimiento.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document ChangeDocument(Document document, SettlementRequestModel requestModel)
        {
            document = document.MapTo(requestModel, DocumentType);
            document.SettlementInfo = document.SettlementInfo.Build(requestModel); 
            return document;
        }

        /// <summary>
        /// Validacion de los datos del modelo de documento
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        protected override bool ValidateRequest(SettlementRequestModel requestModel, out string errormsg)
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