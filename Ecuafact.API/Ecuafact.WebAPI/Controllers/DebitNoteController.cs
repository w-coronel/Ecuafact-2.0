using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// NOTA DE DEBITO: Servicio API para la administracion de Notas de Débito.
    /// </summary>
    [EcuafactExpressAuthorize]
    [DisplayName("Nota de Débito")]
    public class DebitNoteController : DocumentAPIControllerBase<DebitNoteRequestModel>
    {
        /// <summary>
        /// NOTA DE DEBITO: Servicio API para la administracion de Notas de Débito.
        /// </summary>
        /// <param name="contributorsService"></param>
        /// <param name="catalogsService"></param>
        /// <param name="productsService"></param>
        /// <param name="documentsService"></param>
        /// <param name="taxesService"></param>
        /// <param name="subscriptionService"></param>
        /// <param name="appService"></param> 
        public DebitNoteController(IContributorsService contributorsService, ICatalogsService catalogsService,
            IProductsService productsService, IDocumentsService documentsService, ITaxesService taxesService, ISubscriptionService subscriptionService, IAppService appService)
            : base(contributorsService, catalogsService, productsService, documentsService, taxesService, subscriptionService, appService)
        {
        }

        /// <summary>
        /// Nota de Débito
        /// </summary>
        protected override string DocumentType => DocumentTypeEnum.DebitNote.GetCoreValue();


        /// <summary>
        ///     Obtener Nota de Débito
        /// </summary>
        /// <remarks>
        ///     Devuelve una Nota de Débito electronica por id o numero de documento
        /// </remarks>
        /// <param name="id">Id de la nota de Débito o numero del documento en formato XXX-XXX-XXXXXXXXX</param>
        /// <returns></returns>
        [HttpGet, Route("debitnote/{id}")]
        public DebitNoteDto GetDebitNoteById(string id)
        {
            if (id.Contains("-"))
            {
                return GetDocument(id).ToDebitNote();
            }
            else
            {
                long idValue = 0;
                if (long.TryParse(id, out idValue))
                {
                   return GetDocumentById(idValue).ToDebitNote();
                }
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
        }

        /// <summary>
        ///     Buscar Notas de Debito
        /// </summary>
        /// <remarks>
        ///     Realiza la busqueda de notas de débito usando una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de notas de débito</returns>
        [HttpGet, Route("debitnote")]
        public async Task<IPagedList<DebitNoteDto>> GetDebitNotes(string search = null, DateTime? startDate = null, DateTime? endDate = null, DocumentStatusEnum? status = null, int? page = 1, int? pageSize = null, bool includeDeleted = false)
        {
            return await GetDocuments(search, startDate, endDate, DocumentType, status, page, pageSize, includeDeleted, document => document.ToDebitNote());
        }

        /// <summary>
        ///     Crear Nota de Debito
        /// </summary>
        /// <remarks>
        ///     Permite crear una nueva nota debito electrónica.
        /// </remarks>
        /// <param name="requestModel">datos de la nota debito</param>
        /// <returns>Devuelve la nota debito electrónica creada.</returns>
        [HttpPost, Route("debitnote")]
        [ResponseType(typeof(DebitNoteDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Nota de Débito!", typeof(OperationResult))]
        public HttpResponseMessage PostDebitNote(DebitNoteRequestModel requestModel)
        {
            try
            {
                var result = Save(requestModel);

                return Request.CreateResponse(HttpStatusCode.Created, result.ToDebitNote(), JsonCustomFormatter.Default);
            }
            catch (HttpResponseException ex)
            {
                Log(ex.Response, requestModel);

                return ex.Response;
            }
            catch (Exception ex)
            {
                Log(ex, requestModel);

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la Nota de Débito!");
            }
        }

        /// <summary>
        ///     Actualizar Nota de Débito
        /// </summary>
        /// <remarks>
        ///     Permite actualizar los datos de una nota de débito existente, siempre y cuando no este emitida o en proceso.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <returns>La nota de débito electronica modificada</returns>
        [HttpPut, Route("debitnote/{id}")]
        [ResponseType(typeof(DebitNoteDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Nota de Débito!", typeof(OperationResult))]
        public HttpResponseMessage PutDebitNote(long id, DebitNoteRequestModel requestModel)
        {
            try
            {
                this.AllowFinalConsumer = true;

                var result = Update(id, requestModel);

                return Request.CreateResponse(HttpStatusCode.Created, result.ToDebitNote(), JsonCustomFormatter.Default);
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

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la Nota de Débito!");
            }
        }


        #region ********* Document API Overrides ********* 


        /// <summary>
        /// Proceso que interpreta los requerimientos y genera un nuevo documento adaptado al tipo
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document GenerateDocument(DebitNoteRequestModel requestModel)
        {
            var document = requestModel.ToDocument(DocumentType);
            document.DebitNoteInfo = document.DebitNoteInfo.Build(requestModel);

            return document;
        }

        /// <summary>
        /// Cambia la informacion de un documento con la especificada en un requerimiento.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document ChangeDocument(Document document, DebitNoteRequestModel requestModel)
        {
            document = document.MapTo(requestModel, DocumentType);
            document.DebitNoteInfo = document.DebitNoteInfo.Build(requestModel);

            return document;
        }

        /// <summary>
        /// Validacion de los datos del modelo de documento
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        protected override bool ValidateRequest(DebitNoteRequestModel requestModel, out string errormsg)
        {            
            errormsg = "OK";

            if (requestModel.Details == null || !requestModel.Details.Any())
            {
                errormsg = "El documento debe incluir por lo menos un motivo valido!";
                return false;
            }

            requestModel.Details.ForEach(d =>{d.TaxCode = "2";});

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