using System;
using System.Collections;
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
    /// NOTA DE CREDITO: Servicio API para la administracion de Notas de Crédito.
    /// </summary>
    [EcuafactExpressAuthorize]
    [DisplayName("Nota de Crédito")]
    public class CreditNoteController : DocumentAPIControllerBase<CreditNoteRequestModel>
    {
        /// <summary>
        /// NOTA DE CREDITO: Servicio API para la administracion de Notas de Crédito.
        /// </summary>
        /// <param name="contributorsService"></param>
        /// <param name="catalogsService"></param>
        /// <param name="productsService"></param>
        /// <param name="documentsService"></param>
        /// <param name="taxesService"></param>
        /// <param name="subscriptionService"></param>
        /// <param name="appService"></param> 
        public CreditNoteController(IContributorsService contributorsService, ICatalogsService catalogsService, 
            IProductsService productsService, IDocumentsService documentsService, ITaxesService taxesService, ISubscriptionService subscriptionService, IAppService appService) 
            : base(contributorsService, catalogsService, productsService, documentsService, taxesService, subscriptionService, appService)
        {
        }
                 
        /// <summary>
        /// Nota de Credito
        /// </summary>
        protected override string DocumentType => DocumentTypeEnum.CreditNote.GetCoreValue();


        /// <summary>
        ///     Obtener Nota de Credito
        /// </summary>
        /// <remarks>
        ///     Devuelve una Nota de Credito electronica por id o numero de documento
        /// </remarks>
        /// <param name="id">Id de la nota de credito o numero del documento en formato XXX-XXX-XXXXXXXXX</param>
        /// <returns></returns>
        [HttpGet, Route("creditnote/{id}")]
        public CreditNoteDto GetCreditNoteById(string id)
        {
            if (id.Contains("-"))
            {
                return GetDocument(id).ToCreditNote();
            }
            else
            {
                long idValue = 0;
                if (long.TryParse(id, out idValue))
                {
                    return GetDocumentById(idValue).ToCreditNote();                
                }
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
        }

        /// <summary>
        ///     Buscar Notas de Credito
        /// </summary>
        /// <remarks>
        ///     Realiza la busqueda de notas de credito usando una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de notas de credito</returns>
        [HttpGet, Route("creditnote")]
        public async Task<IPagedList<CreditNoteDto>> GetCreditNotes(string search = null, DateTime? startDate = null, DateTime? endDate = null, DocumentStatusEnum? status = null, int? page = 1, int? pageSize = null, bool includeDeleted = false)
        {
            return await GetDocuments(search, startDate, endDate, DocumentType, status, page, pageSize, includeDeleted, document => document.ToCreditNote()); 
        }

        /// <summary>
        ///     Crear Nota de Credito
        /// </summary>
        /// <remarks>
        ///     Permite crear una nueva factura electrónica.
        /// </remarks>
        /// <param name="requestModel">datos de la factura</param>
        /// <returns>Devuelve la factura electrónica creada.</returns>
        [HttpPost, Route("creditnote")]
        [ResponseType(typeof(CreditNoteDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Nota de Credito!", typeof(OperationResult))]
        public HttpResponseMessage PostCreditNote(CreditNoteRequestModel requestModel)
        {
            try
            {
                var result = Save(requestModel);

                return Request.CreateResponse(HttpStatusCode.Created, result.ToCreditNote(), JsonCustomFormatter.Default);
            }
            catch (HttpResponseException ex)
            {
                Log(ex.Response, requestModel);

                return ex.Response;
            }
            catch (Exception ex)
            {
                Log(ex, requestModel);

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la Nota de Credito!");
            }
        }

        /// <summary>
        ///     Actualizar Nota de Credito
        /// </summary>
        /// <remarks>
        ///     Permite actualizar los datos de una nota de crédito existente, siempre y cuando no este emitida o en proceso.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <returns>La nota de crédito electronica modificada</returns>
        [HttpPut, Route("creditnote/{id}")]
        [ResponseType(typeof(CreditNoteDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Nota de Crédito!", typeof(OperationResult))]
        public HttpResponseMessage PutCreditNote(long id, CreditNoteRequestModel requestModel)
        {
            try
            {
                this.AllowFinalConsumer = true;

                var result = Update(id, requestModel);

                return Request.CreateResponse(HttpStatusCode.Created, result.ToCreditNote(), JsonCustomFormatter.Default);
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

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la Nota de Crédito!");
            }
        }


        #region ********* Document API Overrides ********* 


        /// <summary>
        /// Proceso que interpreta los requerimientos y genera un nuevo documento adaptado al tipo
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document GenerateDocument(CreditNoteRequestModel requestModel)
        {
            var document = requestModel.ToDocument(DocumentType);
            document.CreditNoteInfo = document.CreditNoteInfo.Build(requestModel);

            return document;
        }

        /// <summary>
        /// Cambia la informacion de un documento con la especificada en un requerimiento.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document ChangeDocument(Document document, CreditNoteRequestModel requestModel)
        {
            document = document.MapTo(requestModel, DocumentType);
            document.CreditNoteInfo = document.CreditNoteInfo.Build(requestModel); 

            return document;
        }
          
        /// <summary>
        /// Validacion de los datos del modelo de documento
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        protected override bool ValidateRequest(CreditNoteRequestModel requestModel, out string errormsg)
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


        static List<TotalTax> GetTotalTaxes(List<CreditNoteDetailModel> details)
        {

            // Llenamos el total de impuestos
            var totalTaxes = new List<TotalTax>();
            foreach (var item in details.Select(model => model.Taxes))
            {
                item.ForEach(tax =>
                {
                    // Buscamos el impuesto
                    var taxItem = totalTaxes.Find(x => x.PercentageTaxCode == tax.PercentageCode);

                    // Si existe en la lista general del total de impuestos se suman los valores respectivos
                    if (taxItem != null)
                    {
                        taxItem.TaxableBase += tax.TaxableBase;
                        taxItem.TaxValue += tax.TaxValue;
                    }
                    else
                    {
                        // De lo contrario se tiene que agregar una nueva linea del total de impuestos
                        totalTaxes.Add(
                            new TotalTax
                            {
                                PercentageTaxCode = tax.PercentageCode,
                                TaxRate = tax.Rate,
                                TaxCode = tax.Code,
                                TaxableBase = tax.TaxableBase,
                                TaxValue = tax.TaxValue,
                                AditionalDiscount = 0M
                            });
                    }
                });
            }

            return totalTaxes;
        }
        
        #endregion

    }
}
