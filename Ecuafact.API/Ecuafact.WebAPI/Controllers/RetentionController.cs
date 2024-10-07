using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.Models.Dtos;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using X.PagedList;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// COMPROBANTES DE RENTENCION: Servicio API para la administracion de Retenciones.
    /// </summary>
    [EcuafactExpressAuthorize]
    [DisplayName("Retenciones")]
    public class RetentionController : DocumentAPIControllerBase<RetentionRequestModel>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contributorsService"></param>
        /// <param name="catalogsService"></param>
        /// <param name="productsService"></param>
        /// <param name="documentsService"></param>
        /// <param name="taxesService"></param>
        /// <param name="subscriptionService"></param>
        /// <param name="appService"></param> 
        public RetentionController(IContributorsService contributorsService, ICatalogsService catalogsService, 
            IProductsService productsService, IDocumentsService documentsService, ITaxesService taxesService, ISubscriptionService subscriptionService, IAppService appService) 
            : base(contributorsService, catalogsService, productsService, documentsService, taxesService, subscriptionService, appService)
        {
        }

        //public const string RETENTION_DOCUMENT_CODE = "07";
        /// <summary>
        /// Retenciones
        /// </summary>
        protected override string DocumentType => DocumentTypeEnum.RetentionReceipt.GetCoreValue();


        /// <summary>
        ///     Obtener Comprobante de Retención
        /// </summary>
        /// <remarks>
        ///     Devuelve un Comprobante de Retención por ID o numero de documento
        /// </remarks>
        /// <param name="id">Id del Comprobante de Retención o numero del documento en formato XXX-XXX-XXXXXXXXX</param>
        /// <returns></returns>
        [HttpGet, Route("retention/{id}")]
        public RetentionDto GetRetentionById(string id)
        {
            if (id.Contains("-"))
            {
                return GetDocument(id).ToRetention();
            }
            else
            {
                long idValue = 0;
                if (long.TryParse(id, out idValue))
                {
                    return GetDocumentById(idValue).ToRetention();
                }
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
        }


        /// <summary>
        ///     Buscar Comprobantes de Retención
        /// </summary>
        /// <remarks>
        ///     Realiza la busqueda de Comprobantes de Retención usando una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de Comprobantes de Retención</returns>
        [HttpGet, Route("retention")]
        public async Task<IPagedList<RetentionDto>> GetRetentions(string search = null, DateTime? startDate = null, DateTime? endDate = null, DocumentStatusEnum? status = null, int? page = 1, int? pageSize = null, bool includeDeleted = false)
        {
            return await GetDocuments(search, startDate, endDate, DocumentType, status, page, pageSize, includeDeleted, document => document.ToRetention()); 
        }


        /// <summary>
        ///     Crear Comprobante de Retención
        /// </summary>
        /// <remarks>
        ///     Permite crear un nuevo Comprobante de Retención.
        /// </remarks>
        /// <param name="requestModel">datos del comprobante</param>
        /// <returns>Devuelve el comprobante de retención creado.</returns>
        [HttpPost, Route("retention")]
        [ResponseType(typeof(RetentionDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Retención.", typeof(OperationResult))]
        public HttpResponseMessage PostRetention(RetentionRequestModel requestModel)
        {
            try
            {
                var result = Save(requestModel);

                return Request.CreateResponse(HttpStatusCode.Created, result.ToRetention(), JsonCustomFormatter.Default);
            }
            catch (HttpResponseException ex)
            {
                Log(ex.Response, requestModel);

                return ex.Response;
            }
            catch (Exception ex)
            {
                Log(ex, requestModel);

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la Retencion!");
            }

        }

        /// <summary>
        ///     Actualizar Comprobante de Retención
        /// </summary>
        /// <remarks>
        ///     Permite actualizar los datos de un comprobante de retención existente, siempre y cuando no este emitida o en proceso.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <returns>La retención electronica modificada</returns>
        [HttpPut, Route("retention/{id}")]
        [ResponseType(typeof(RetentionDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Retención!", typeof(OperationResult))]
        public HttpResponseMessage PutRetention(long id, RetentionRequestModel requestModel)
        {
            try
            {
                this.AllowFinalConsumer = true;

                var result = Update(id, requestModel);

                return Request.CreateResponse(HttpStatusCode.Created, result.ToRetention(), JsonCustomFormatter.Default);
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

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la Retención!");
            }
        }

        #region ********* Document API Overrides ********* 


        /// <summary>
        /// Generador de Documentos
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document GenerateDocument(RetentionRequestModel requestModel)
        {
            var document = requestModel.ToDocument(DocumentType);
            document.RetentionInfo = document.RetentionInfo.Build(requestModel);
             
            return document;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document ChangeDocument(Document document, RetentionRequestModel requestModel)
        {
            document = document.MapTo(requestModel, DocumentType);
            document.RetentionInfo = document.RetentionInfo.Build(requestModel);

            return document;
        }

        /// <summary>
        /// Valida los datos del requerimiento
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        protected override bool ValidateRequest(RetentionRequestModel requestModel, out string errormsg)
        {

            if (string.IsNullOrWhiteSpace(requestModel.SupportCode))
            {
                errormsg = "El documento debe incluir el tipo de Sustento del Comprobante!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(requestModel.RelatedParty))
            {
                errormsg = "El documento debe incluir parte relacionada!";
                return false;
            }
            requestModel.Details?.ForEach(x =>
            {
                x.ReferenceDocumentCode = x.ReferenceDocumentCode ?? requestModel.ReferenceDocumentCode;
                x.ReferenceDocumentDate = x.ReferenceDocumentDate ?? requestModel.ReferenceDocumentDate;
                x.ReferenceDocumentNumber = x.ReferenceDocumentNumber ?? requestModel.ReferenceDocumentNumber;
            });

            ValidateRetentionDetails(requestModel.Details);
            return base.ValidateRequest(requestModel, out errormsg);
        }

        private void ValidateRetentionDetails(List<RetentionDetailModel> details)
        {
            foreach (var item in details)
            {
                var tax = _taxesService.GetTax(item.RetentionTaxId) ?? _taxesService.GetTaxByCode(item.RetentionTaxCode);

                if (tax != null)
                {
                    item.RetentionTaxCode = tax.SriCode;
                    item.TaxTypeCode = _catalogsService.GetTaxTypes().First(o => o.Id == tax.TaxTypeId).SriCode;
                }
                else
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest,
                        $"El detalle de impuestos esta incorrecto {JsonConvert.SerializeObject(item)}",
                        $"El codigo de impuesto: [{item.RetentionTaxId} | {item.RetentionTaxCode}] esta incorrecto.");
                }

                // Validamos los detalles de impuestos: especialmente los valores
                var itemValue = decimal.Round(item.TaxBase * item.TaxRate / 100, 2);
                var diferencia = decimal.Round(itemValue - item.TaxValue, 2);

                if (diferencia > 1 || diferencia < -1)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest,
                        $"El calculo del valor de los impuestos esta incorrecto {JsonConvert.SerializeObject(item)}",
                        $"El valor: [{item.TaxValue}] no coincide con el valor calculado [{itemValue}].");
                }
            }
        }

        #endregion
    }
}
