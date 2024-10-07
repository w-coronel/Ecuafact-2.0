using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models;
using Swashbuckle.Swagger.Annotations;
using X.PagedList;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// CONTRIBUYENTES
    /// </summary>
    [EcuafactExpressAuthorize]
    [DisplayName("Contribuyentes")]
    public class ContributorsController : ApiController
    {
        private readonly IContributorsService _contributorsService;
        private readonly ICatalogsService _catalogsService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contributorsService"></param>
        /// <param name="catalogsService"></param>
        public ContributorsController(IContributorsService contributorsService, ICatalogsService catalogsService)
        {
            _contributorsService = contributorsService;
            _catalogsService = catalogsService;
        }

        /// <summary> 
        /// Obtener Contribuyente
        /// </summary>
        /// <remarks>
        ///     Obtiene los datos de un contribuyente por Numero de Identidad o ID.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("contributors/{id}")]
        public ContributorDto GetContributor(string id)
        {
            try
            {
                var contributor = _contributorsService.GetContributorByRUC(id, issuer.Id) ??
                    _contributorsService.GetContributorById(Convert.ToInt64(id), issuer.Id);

                if (contributor != null)
                {
                    return contributor.ToContributorDto();
                }
            }
            catch (Exception) { }

            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, "No existe el contribuyente solicitado", "No existe el contribuyente solicitado");
        }
                

        /// <summary>
        /// Buscar Contribuyentes
        /// </summary>
        /// <remarks>
        ///     Busca contribuyentes que cumplan con los datos especificados
        /// </remarks>
        /// <param name="search">Valor que se utiliza para filtrar los datos</param>
        /// <param name="contributorType"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="showDisabled">Mostrar eliminados</param>
        /// <returns></returns>
        [HttpGet, Route("contributors")]
        public IPagedList<ContributorDto> SearchContributors(string search = null, ContributorTypeEnum contributorType = ContributorTypeEnum.All, int? page = 1, int? pageSize = 10, bool showDisabled = false)
        {
            var contributors = _contributorsService.GetContributors(issuer.Id);

            if (!showDisabled)
            {
                contributors = contributors.Where(model => model.IsEnabled);
            }

            switch (contributorType)
            {
                case ContributorTypeEnum.Customer:
                    contributors = contributors.Where(model => model.IsCustomer);
                    break;
                case ContributorTypeEnum.Supplier:
                    contributors = contributors.Where(model => model.IsSupplier);
                    break;
                case ContributorTypeEnum.Driver:
                    contributors = contributors.Where(model => model.IsDriver);
                    break;
            }

            if (!string.IsNullOrEmpty(search))
            {
                contributors = contributors.Where(model => model.Identification.Contains(search)
                    || model.TradeName.Contains(search)
                    || model.BussinesName.Contains(search));
            }

            contributors = contributors.OrderBy(model => model.Id);

            page = page ?? 1;
            pageSize = pageSize ?? 10;

            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            var lst = contributors.ToPagedList(page.Value, pageSize.Value);

            return lst.Select(model => model.ToContributorDto());
        }


        /// <summary>
        /// Crear Contribuyente
        /// </summary>
        /// <remarks>
        ///     Agrega un nuevo contribuyente para el emisor actual
        /// </remarks>
        /// <param name="request"></param>
        /// <returns>Return a HttpResponseMessage response</returns>
        [HttpPost, Route("contributors")]
        [ResponseType(typeof(ContributorDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Petición Invalida!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar el contribuyente", typeof(OperationResult))]
        public HttpResponseMessage PostContributor(ContributorRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Hubo un error al guardar la solicitud. {error?.ErrorMessage}", error?.Exception?.ToString());
            }

            var model = request.ToContributor();

            model.IssuerId = issuer.Id;
            model.IsEnabled = true;

            if (string.IsNullOrEmpty(request.BussinesName))
            {
                request.BussinesName = request.TradeName;
            }

            var result = _contributorsService.Add(model);

            if (!result.IsSuccess)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.BadRequest, result.DevMessage, result.UserMessage);
            }

            var response = Request.CreateResponse(HttpStatusCode.Created, result.Entity.ToContributorDto());
            return response;
        }

        /// <summary>
        /// Crear Contribuyente masivamente 
        /// </summary>
        /// <remarks>
        ///     Permite crear masivamente Contribuyente con los datos especificados.
        /// </remarks>
        /// <returns></returns>
        [HttpPost, Route("contributors/fileimport")]
        public HttpResponseMessage Contributors(ContributorImportModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Hubo un error al importar clientes. {error?.ErrorMessage}", error?.Exception?.ToString());
            }
            model.IssuerId = issuer.Id;
            var addProductResult = _contributorsService.ImportContributor(model.ImportContributor());
            if (!addProductResult.IsSuccess)
            {
                return Request.BuildHttpErrorResponse((HttpStatusCode)addProductResult.StatusCode, addProductResult.DevMessage, addProductResult.UserMessage);
            }
            var response = Request.CreateResponse(HttpStatusCode.Created, true);
            return response;
        }

        /// <summary>
        /// Exporta los productos en formato excel
        /// </summary>
        /// <remarks>
        ///     Permite exporta los productos en formato excel.
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("contributors/export")]
        public async Task<HttpResponseMessage> ExporContributors()
        {
            try
            {               
                var contributors = await _contributorsService.GetContributors(issuer.Id).ToListAsync();
                if (contributors.Count > 0)
                {
                    // Ahora generamos el archivo segun el tipo especificado:
                    HttpResponseMessage result = this.Request.CreateResponse(HttpStatusCode.OK);
                    var stream = new MemoryStream(contributors.ToExcelContributors());
                    result.Content = new StreamContent(stream);
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/excel");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = $"{issuer.RUC}_Clientes.xlsx"
                    };

                    return result;
                }
                return Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, $"Error al descargar la exportación de clientes]");
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
        /// Actualizar Contribuyente
        /// </summary>
        /// <remarks>
        ///     Modifica el contribuyente especificado para el emisor actual
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut, Route("contributors/{id}")]
        [ResponseType(typeof(ContributorDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Petición Invalida!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar el contribuyente", typeof(OperationResult))]
        public HttpResponseMessage PutContributor(long id, ContributorRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Hubo un error al guardar la solicitud. {error?.ErrorMessage}", error?.Exception?.ToString());
            }

            var contributor = _contributorsService.GetContributorById(id, issuer.Id);

            if (contributor == null)
                throw new HttpResponseException(Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, "Contribuyente no existe", "No existe el contribuyente solicitado"));
             
            var model = request.ToContributor(contributor);
            
            var updatedContributor = _contributorsService.Update(model);

            if (!updatedContributor.IsSuccess)
            {
                return Request.BuildHttpErrorResponse(updatedContributor.StatusCode, updatedContributor.DevMessage, updatedContributor.UserMessage);
            }

            var response = Request.CreateResponse(HttpStatusCode.Created, updatedContributor.Entity.ToContributorDto());
            return response;
        }

        /// <summary>
        /// Eliminar Contribuyente
        /// </summary>
        /// <remarks>
        ///     Elimina el contribuyente especificado para el emisor actual
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, Route("contributors/{id}")]
        [ResponseType(typeof(ContributorDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Petición Invalida!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar el contribuyente", typeof(OperationResult))]
        public HttpResponseMessage DeleteContributor(long id)
        {
            var contributor = _contributorsService.GetContributorById(id, issuer.Id);

            if (contributor == null)
                throw new HttpResponseException(Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, "Contribuyente no existe", "No existe el contribuyente solicitado"));

            contributor.IsEnabled = false;

            var updatedContributor = _contributorsService.Update(contributor);

            if (!updatedContributor.IsSuccess)
            {
                return Request.BuildHttpErrorResponse(updatedContributor.StatusCode, updatedContributor.DevMessage, updatedContributor.UserMessage);
            }

            var response =  Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private IssuerDto issuer => HttpContext.Current.Session.GetAuthenticatedIssuerSession();


        #region Metodos obsoletos


        /// <summary>
        /// Devuelve una lista de los clientes
        /// </summary>
        /// <param name="search">Valor que se utiliza para filtrar los datos</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>Lista de Clientes</returns>
        [HttpGet, Route("Customers")]
        [ApiExplorerSettings(IgnoreApi = true), Obsolete]
        public IPagedList<ContributorDto> SearchCustomers(string search = null, int? page = null, int? pageSize = null)
        {
            return SearchContributors(search, ContributorTypeEnum.Customer, page, pageSize);
        }


        /// <summary>
        /// Devuelve una lista de los proveedores
        /// </summary>
        /// <param name="search">Valor que se utiliza para filtrar los datos</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>Lista de Clientes</returns>
        [HttpGet, Route("Suppliers")]
        [ApiExplorerSettings(IgnoreApi = true), Obsolete]
        public IPagedList<ContributorDto> SearchSuppliers(string search = null, int? page = null, int? pageSize = null)
        {
            return SearchContributors(search, ContributorTypeEnum.Supplier, page, pageSize);
        }


        /// <summary>
        /// Devuelve una lista de los transportistas
        /// </summary>
        /// <param name="search">Valor que se utiliza para filtrar los datos</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>Lista de transportistas</returns>
        [HttpGet, Route("Drivers")]
        [ApiExplorerSettings(IgnoreApi = true), Obsolete]
        public IPagedList<ContributorDto> SearchDrivers(string search = null, int? page = null, int? pageSize = null)
        {
            return SearchContributors(search, ContributorTypeEnum.Driver, page, pageSize);
        }

        #endregion
    }
}
