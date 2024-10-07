using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// IMPUESTOS: Administracion de los impuestos utilizados por los comprobantes de Retencion
    /// </summary>
    [EcuafactExpressAuthorize]
    [DisplayName("Catálogo Impuestos")]
    public class TaxesController : ApiController
    { 
        private readonly ITaxesService _taxesService;
        private readonly ICatalogsService _catalogsService;
        
        /// <summary>
        /// Impuestos
        /// </summary>
        /// <param name="taxesService"></param>
        /// <param name="catalogsService"></param>
        public TaxesController(ITaxesService taxesService, ICatalogsService catalogsService)
        {
            _taxesService = taxesService;
            _catalogsService = catalogsService;

        }

        /// <summary>
        /// Buscar Impuestos
        /// </summary>
        /// <remarks>
        ///     Devuelve los impuestos existentes aplicando un filtro por codigo o descripcion del impuesto
        /// </remarks>
        /// <param name="searchTerm">Filtro por codigo o descripcion del impuesto</param>
        /// <returns></returns>
        [HttpGet, Route("taxes")]
        public IEnumerable<RetentionTax> SearchTaxes(string searchTerm = null)
        { 
            // Si el termino de busqueda es vacio o ALL o indica el * devuelve todos los impuestos de ese tipo
            if (string.IsNullOrEmpty(searchTerm) || (searchTerm.ToLower() == "all" || searchTerm == "*"))
            {
                return _taxesService.GetAllTaxes().ToList(); 
            }

            var taxes = _taxesService.SearchTaxes(searchTerm).ToList();

            if (taxes.Any())
            {
                return taxes;
            }

            throw new HttpResponseException(Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, "Consulta sin resultados", "La consulta solicitada no devolvio resultados con los parámetros proporcionados"));
        }

        /// <summary>
        /// Obtener Impuesto
        /// </summary>
        /// <remarks>
        ///     Devuelve un impuesto por ID
        /// </remarks>
        /// <param name="id">tax id</param>
        /// <returns>Return a tax of Issuer by Id</returns>
        [HttpGet, Route("taxes/{id}")]
        public RetentionTax GetTax(long id)
        { 
            var tax = _taxesService.GetTax(id);

            if (tax == null)
                throw new HttpResponseException(Request.BuildHttpErrorResponse(HttpStatusCode.NotFound,"Impuesto no existe","El impuesto solicitado no existe"));

            return tax;
        }

        /// <summary>
        /// Crear Impuesto
        /// </summary>
        /// <remarks>
        ///     Permite la creación de un nuevo impuesto
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Route("taxes")]
        
        public HttpResponseMessage PostTax(RetentionTax model)
        {    
            if (!AuthorizeService())
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "No Autorizado");
            } 

            var addTaxResult = _taxesService.AddTax(model);

            if (!addTaxResult.IsSuccess)
                return Request.BuildHttpErrorResponse((HttpStatusCode)addTaxResult.StatusCode,addTaxResult.DevMessage, addTaxResult.UserMessage );
            
            var response = Request.CreateResponse(HttpStatusCode.Created, addTaxResult.Entity );
            return response;
        }


        /// <summary>
        /// Modificar Impuesto
        /// </summary>
        /// <remarks>
        /// Permite modificar el impuesto especificado
        /// </remarks>
        /// <param name="id">Tax Id</param>
        /// <param name="model">The Request model info</param>
        /// <returns>Returns a tax updated object</returns>
        [HttpPut, Route("taxes/{id}")]
        public HttpResponseMessage PutTax(long id, RetentionTax model)
        {
            if (!AuthorizeService())
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "No Autorizado");
            }
             
            var tax = _taxesService.GetTax(id );

            if (tax== null)
                throw new HttpResponseException(Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, "Impuesto no existe", "El impuesto solicitado no existe"));

            var updatedTax = _taxesService.UpdateTax(model);

            if (!updatedTax.IsSuccess)
                return Request.BuildHttpErrorResponse((HttpStatusCode)updatedTax.StatusCode, updatedTax.DevMessage, updatedTax.UserMessage);

            var response = Request.CreateResponse(HttpStatusCode.Created, updatedTax.Entity );
            return response;
        }


        private bool AuthorizeService()
        {
            return Request.Headers.TryGetValues("APIKEY", out var apiKey) && apiKey.FirstOrDefault() == Constants.ServiceToken;
        }
    }
}
