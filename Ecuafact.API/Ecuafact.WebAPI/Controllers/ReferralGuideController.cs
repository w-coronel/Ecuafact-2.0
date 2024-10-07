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
    /// GUIAS DE REMISION: Servicio API para la administracion de Guias de Remision.
    /// </summary>
    [EcuafactExpressAuthorize]
    [DisplayName("Guias de Remisión")]

    public class ReferralGuideController : DocumentAPIControllerBase<ReferralGuideRequestModel>
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
        public ReferralGuideController(IContributorsService contributorsService, ICatalogsService catalogsService, IProductsService productsService, IDocumentsService documentsService,
            ITaxesService taxesService, ISubscriptionService subscriptionService, IAppService appService) 
            : base(contributorsService, catalogsService, productsService, documentsService, taxesService, subscriptionService, appService)
        {
        }

        /// <summary>
        /// GUIA DE REMISION
        /// </summary>
        protected override string DocumentType => DocumentTypeEnum.ReferralGuide.GetCoreValue();



        /// <summary>
        ///     Obtener Guia de Remisión
        /// </summary>
        /// <remarks>
        ///     Devuelve una Guia de Remisión por ID o numero de documento
        /// </remarks>
        /// <param name="id">Id de la Guia de Remisión o numero del documento en formato XXX-XXX-XXXXXXXXX</param>
        /// <returns></returns>
        [HttpGet, Route("referralguide/{id}")]
        public ReferralGuideDto GetReferralGuideById(string id)
        {
            if (id.Contains("-"))
            {
                return GetDocument(id).ToReferralGuide();
            }
            else
            {
                long idValue = 0;
                if (long.TryParse(id, out idValue))
                {                     
                    return GetDocumentById(idValue).ToReferralGuide();
                }
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, $"No existe el documento ID # {id} solicitado.", "No existe el documento!");
        }

        /// <summary>
        ///     Buscar Guía de Remisión
        /// </summary>
        /// <remarks>
        ///     Realiza la busqueda de Guías de Remisión usando una serie de filtros
        /// </remarks>
        /// <returns>Devuelve una lista de Guías de Remisión</returns>
        [HttpGet, Route("referralguide")]
        public async Task<IPagedList<ReferralGuideDto>> GetReferralGuides(string search = null, DateTime? startDate = null, DateTime? endDate = null, DocumentStatusEnum? status = null, int? page = 1, int? pageSize = null, bool includeDeleted = false)
        {
            return await GetDocuments(search, startDate, endDate, DocumentType, status, page, pageSize, includeDeleted, document => document.ToReferralGuide()); 
        }



        /// <summary>
        ///     Crear Guía de Remisión
        /// </summary>
        /// <remarks>
        ///     Permite crear una nueva Guía de Remisión.
        /// </remarks>
        /// <param name="requestModel">datos del comprobante</param>
        /// <returns>Devuelve la Guía de Remisión creada.</returns>
        [HttpPost, Route("referralguide")]
        [ResponseType(typeof(ReferralGuideDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Guía de Remisión.", typeof(OperationResult))]
        public HttpResponseMessage PostGuide(ReferralGuideRequestModel requestModel)
        {
            try
            {
                if (requestModel.ReferenceDocumentId == -1)
                {
                    this.AllowFinalConsumer = true;
                }
                var result = Save(requestModel);
                return Request.CreateResponse(HttpStatusCode.Created, result.ToReferralGuide(), JsonCustomFormatter.Default);
            }
            catch (HttpResponseException ex)
            {
                Log(ex.Response, requestModel);

                return ex.Response;
            }
            catch (Exception ex)
            {
                Log(ex, requestModel);

                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.ToString(), "Se produjo un error al guardar la Guia de Remision!");
            }
        }


        /// <summary>
        ///     Actualizar Guía de Remisión
        /// </summary>
        /// <remarks>
        ///     Permite actualizar los datos de una Guía de Remisión existente, siempre y cuando no este emitida o en proceso.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <returns>La retención electronica modificada</returns>
        [HttpPut, Route("referralguide/{id}")]
        [ResponseType(typeof(ReferralGuideDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Documento Invalido!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar la Guía de Remisión!", typeof(OperationResult))]
        public HttpResponseMessage PutReferralGuide(long id, ReferralGuideRequestModel requestModel)
        {
            try
            {
                this.AllowFinalConsumer = true;

                var result = Update(id, requestModel);

                return Request.CreateResponse(HttpStatusCode.Created, result.ToReferralGuide(), JsonCustomFormatter.Default);
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



        /// <summary>
        /// Generador del Documento para la Guia de Remision
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document GenerateDocument(ReferralGuideRequestModel requestModel)
        {
            if (requestModel.ReferenceDocumentId == -1)
            {
                requestModel.IdentificationType = null;
                requestModel.Identification = null;
                requestModel.ContributorName = null;
                requestModel.ContributorId = 0;
                requestModel.ReferenceDocumentId = null;
                requestModel.Address = null;
            }
            var document = requestModel.ToDocument(DocumentType);
            document.ReferralGuideInfo = document.ReferralGuideInfo.Build(requestModel);

            return document;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        protected override Document ChangeDocument(Document document, ReferralGuideRequestModel requestModel)
        {
            if (requestModel.ReferenceDocumentId == -1)
            {
                requestModel.IdentificationType = null;
                requestModel.Identification = null;
                requestModel.ContributorName = null;
                requestModel.ContributorId = 0;
                requestModel.ReferenceDocumentId = null;
                requestModel.Address = null;
            }
            document.MapTo(requestModel, DocumentType);
            document.ReferralGuideInfo = document.ReferralGuideInfo.Build(requestModel);

            return document;
        }



        /// <summary>
        /// Validacion de los datos del modelo de documento
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="errormsg"></param>
        /// <returns></returns>
        protected override bool ValidateRequest(ReferralGuideRequestModel requestModel, out string errormsg)
        {
            errormsg = "OK";

            if (string.IsNullOrEmpty(requestModel.Address))
            {
                errormsg = "Se requiere la dirección del contribuyente";
                return false;
            }

            if (string.IsNullOrEmpty(requestModel.DestinationAddress))
            {
                errormsg = "Se requiere la dirección de destino";
                return false;
            }

            if (string.IsNullOrEmpty(requestModel.OriginAddress))
            {
                errormsg = "Se requiere la dirección de origen";
                return false;
            }

            if (string.IsNullOrEmpty(requestModel.ShipmentRoute))
            {
                errormsg = "Se requiere la ruta del transporte";
                return false;
            }

            if (requestModel.Details == null || !requestModel.Details.Any())
            {
                errormsg = "El documento debe incluir por lo menos un detalle valido!";
                return false;
            }

            // Verifico la informacion en los detalles
            foreach (var item in requestModel.Details)
            {
                var product = _productsService.GetIssuerProduct(item.ProductId, Issuer.Id);

                if (product != null)
                {
                    item.MainCode = string.IsNullOrEmpty(item.MainCode) ? product.MainCode : item.MainCode;
                    item.AuxCode = string.IsNullOrEmpty(item.AuxCode) ? product.AuxCode : item.AuxCode;
                    item.Description = string.IsNullOrEmpty(item.Description) ? product.Name : item.Description;
                }

                if (string.IsNullOrEmpty(item.MainCode))
                {
                    errormsg = "El codigo principal de uno de los productos en el detalle no ha sido especificado!";
                    return false;
                }

                if (string.IsNullOrEmpty(item.Description))
                {
                    errormsg = "El nombre o descripción de uno de los productos en el detalle no ha sido especificado!";
                    return false;
                }
            }



            if (requestModel.ContributorId == 0 && string.IsNullOrEmpty(requestModel.Identification))
            {
                errormsg = "El documento debe incluir la identificacion del contribuyente!";
                return false;
            }


            if (requestModel.DriverId > 0)
            {
                var driver = _contributorsService.GetContributorById(requestModel.DriverId, Issuer.Id);

                if (driver != null)
                {
                    requestModel.DriverIdentification = string.IsNullOrEmpty(requestModel.DriverIdentification) ? driver.Identification : requestModel.DriverIdentification;
                    requestModel.DriverIdentificationType = string.IsNullOrEmpty(requestModel.DriverIdentificationType) ? driver.IdentificationType.SriCode : requestModel.DriverIdentificationType;
                    requestModel.DriverName = string.IsNullOrEmpty(requestModel.DriverName) ? driver.BussinesName ?? driver.TradeName : requestModel.DriverName;
                }
            }


            if (requestModel.RecipientId > 0)
            {
                var recipient = _contributorsService.GetContributorById(requestModel.RecipientId, Issuer.Id);

                if (recipient != null)
                {
                    requestModel.RecipientIdentification = string.IsNullOrEmpty(requestModel.RecipientIdentification) ? recipient.Identification : requestModel.RecipientIdentification;
                    requestModel.RecipientIdentificationType = string.IsNullOrEmpty(requestModel.RecipientIdentificationType) ? recipient.IdentificationType.SriCode : requestModel.RecipientIdentificationType;
                    requestModel.RecipientName = string.IsNullOrEmpty(requestModel.RecipientName) ? recipient.BussinesName ?? recipient.TradeName : requestModel.RecipientName;
                }
            }

            if (!IdenfiticationTypeIsValid(requestModel.RecipientIdentificationType))
            {
                errormsg = "El tipo de identificación del destinatario es incorrecto. Use los codigos disponibles en el catalogo de tipos de identificación.";
                return false;
            }

            if (!IdenfiticationTypeIsValid(requestModel.DriverIdentificationType))
            {
                errormsg = "El tipo de identificación del conductor es incorrecto. Use los codigos disponibles en el catalogo de tipos de identificación.";
                return false;
            }

            if (string.IsNullOrEmpty(requestModel.ReferenceDocumentNumber) && string.IsNullOrEmpty(requestModel.ReferenceDocumentDate) && string.IsNullOrEmpty(requestModel.ReferenceDocumentAuth))
            {
                ModelState.Clear();
                return true;
            }

            return base.ValidateRequest(requestModel, out errormsg);
        }

    }
}
