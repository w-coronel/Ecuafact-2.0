using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using X.PagedList;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// TRANSPORTISTAS
    /// </summary>
    [EcuafactExpressAuthorize]
    public class CarrierController : ApiController
    {
        private static IIssuersService _issuersService;
        private static ISubscriptionService _subscriptionService;
        private static IUserService _userService;
        private IssuerDto Issuer => this.GetAuthenticatedIssuer();

        /// <summary>
        /// Informacion del Establecimiento y Punto emision transportistas
        /// </summary>
        /// <param name="issuersService"></param>
        /// <param name="userService"></param>        
        public CarrierController(IIssuersService issuersService, IUserService userService, ISubscriptionService subscriptionService)
        {
            _issuersService = issuersService;
            _userService = userService;
            _subscriptionService = subscriptionService;
        }

        [HttpGet, Route("establishment")]
        public IPagedList<Establishments> SearchEstablishments(string search = null, int? page = null, int? pageSize = null)
        {
            var establishments = _issuersService.GetEstablishments(Issuer.Id);
            if (!string.IsNullOrEmpty(search))
            {
                establishments = establishments.Where(model => model.Code.Contains(search)
                    || model.Name.Contains(search)
                    || model.Address.Contains(search));
            }

            establishments = establishments.OrderBy(model => model.Id);

            page = page ?? 1;
            pageSize = pageSize ?? establishments.Count();

            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            var lst = establishments.ToPagedList(page.Value, pageSize.Value);

            return lst;
        }

        /// <summary>
        /// Crear Establecimiento  
        /// </summary>
        /// <remarks>
        ///     Permite crear Establecimiento con los datos especificados.
        /// </remarks>
        /// <returns></returns>
        [HttpPost, Route("establishment")]
        public HttpResponseMessage AddEstablishment(EstablishmentsModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Error al crear el establecimiento. {error?.ErrorMessage}", error?.Exception?.ToString());
            }

            model.IssuerId = Issuer.Id;

            var _establishment = _issuersService.AddEstablishments(model.ToEstablishment());
            if (_establishment.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.Created, _establishment.Entity);
            }
            return Request.BuildHttpErrorResponse((HttpStatusCode)_establishment.StatusCode, _establishment.DevMessage, _establishment.UserMessage);

        }

        /// <summary>
        /// Actualizar Establecimiento
        /// </summary>
        /// <remarks>
        ///     Permite modificar el establecimiento con los datos especificados.
        /// </remarks>  
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut, Route("establishment/{id}")]
        public HttpResponseMessage PutEstablishment(long id, EstablishmentsModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Hubo un error al guardar el establecimiento. {error?.ErrorMessage}", error?.Exception?.ToString());
            }
            var response = _issuersService.UpdateEstablishments(model.ToUpdateEstablishment(id));
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.Created, response.Entity);
            }
            return Request.BuildHttpErrorResponse(response.StatusCode, response.DevMessage, response.UserMessage);
        }


        /// <summary>
        /// Obtener Establecimiento
        /// </summary>
        /// <remarks>
        ///     Devuelve el establecimiento por el ID especificado
        /// </remarks>
        /// <param name="id">Id del establecimiento</param>
        /// <returns>Return a establishment of Issuer by Id</returns>
        [HttpGet, Route("establishment/{id}")]
        public HttpResponseMessage GetEstablishmentById(long id)
        {

            var establishment = _issuersService.GetEstablishmentsById(id);

            if (establishment.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, establishment.Entity);

            }
            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, "Establecimiento no existe", "El establecimiento solicitado no existe");
        }

        [HttpGet, Route("issuePoint/{id}/Search")]
        public IPagedList<IssuePoint> SearchIssuePoint(long id, string search = null, int? page = null, int? pageSize = null)
        {
            var issuePoint = _issuersService.GetIssuePoint(id);
            if (!string.IsNullOrEmpty(search))
            {
                issuePoint = issuePoint.Where(model => model.Code.Contains(search)
                    || model.Name.Contains(search)
                    || model.CarrierRUC.Contains(search)
                    || model.CarPlate.Contains(search));
            }

            issuePoint = issuePoint.OrderBy(model => model.Id);

            page = page ?? 1;
            pageSize = pageSize ?? issuePoint.Count();

            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            var lst = issuePoint.ToPagedList(page.Value, pageSize.Value);

            return lst;
        }



        /// <summary>
        /// Obtener punto de emisión
        /// </summary>
        /// <remarks>
        ///     Devuelve los puntos de emisión por el emisor
        /// </remarks>        
        /// <returns>Return a issuePoints of Issuer</returns>
        [HttpGet, Route("issuePoints/issuer")]
        public HttpResponseMessage GetIssuePointByIssuer()
        {

            var issuePoint = _issuersService.GetIssuePointByIssuer(Issuer.Id).ToList();

            if (issuePoint?.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.OK, issuePoint);

            }
            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, "No hay registrados puntos de emisión", "No hay registrados puntos de emisión");
        }


        /// <summary>
        /// Crear punto de emisión
        /// </summary>
        /// <remarks>
        ///     Permite crear un punto de emisión
        /// </remarks>
        /// <returns></returns>
        [HttpPost, Route("issuePoint")]
        public HttpResponseMessage AddIssuePoint(IssuePointModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Error al crear el punto emisión. {error?.ErrorMessage}", error?.Exception?.ToString());
            }
            model.IssuerId = Issuer.Id;
            var _issuePoint = _issuersService.AddIssuePointCarrier(model.ToIssuePoint());
            if (_issuePoint.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.Created, _issuePoint.Entity);
            }
            return Request.BuildHttpErrorResponse((HttpStatusCode)_issuePoint.StatusCode, _issuePoint.DevMessage, _issuePoint.UserMessage);

        }

        /// <summary>
        /// Actualizar Punto Emisión
        /// </summary>
        /// <remarks>
        ///     Permite modificar el punto emisión con los datos especificados.
        /// </remarks>  
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut, Route("issuePoint/{id}")]
        public HttpResponseMessage PutIssuePoint(long id, IssuePointModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Hubo un error al guardar el punto de emisión. {error?.ErrorMessage}", error?.Exception?.ToString());
            }
            model.IssuerId = Issuer.Id;
            var response = _issuersService.UpdateIssuePointCarrie(model.ToUpdateIssuePoint(id));
            if (response.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.Created, response.Entity);
            }
            return Request.BuildHttpErrorResponse(response.StatusCode, response.DevMessage, response.UserMessage);
        }


        /// <summary>
        /// Obtener Punto Emisión
        /// </summary>
        /// <remarks>
        ///     Devuelve el punto emisión por el ID especificado
        /// </remarks>
        /// <param name="id">Id del producto</param>
        /// <returns>Return a issuePoint of Issuer by Id</returns>
        [HttpGet, Route("issuePoint/{id}")]
        public HttpResponseMessage GetIssuePointById(long id)
        {

            var issuePoint = _issuersService.GetIssuePointById(id);

            if (issuePoint.IsSuccess)
            {
                return Request.CreateResponse(HttpStatusCode.OK, issuePoint.Entity);

            }
            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, "Punto de emisión no existe", "El punto de emisión solicitado no existe");
        }


        /// <summary>
        /// Obtener informacion de un emisor por el Numero de RUC
        /// </summary>
        /// <param name="ruc"></param>
        /// <returns></returns>
        [Route("carrier/{ruc}/issuers")]
        public CarrierIssuer GetIssuerByRUC(string ruc = null)
        {
            if (string.IsNullOrEmpty(ruc))
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.Forbidden, "El parametro ruc debe ser obligatorio", "El parametro ruc debe ser obligatorio");
            }
            var issuer = _issuersService.GetIssuer(ruc);
            if (issuer != null)
            {
                var issuerDto = issuer?.ToIssuerDto();
                var subscription = _subscriptionService.GetSubscription(issuerDto.RUC);
                var response = new CarrierIssuer{
                    IssuerDto = issuerDto,
                    SubscriptionDto = subscription?.ToSubscriptionDto()
                };
                return response;
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.Forbidden, $"No se encontro informacón del ruc {ruc}", $"No se encuentra registrado el ruc {ruc}");
        }

        /// <summary>
        /// Eliminar punto de emisión
        /// </summary>
        /// <remarks>
        ///     Permite eliminar un punto de emisión especificado.
        /// </remarks>
        /// <param name="id">Id del punto emision</param>
        /// <returns></returns>
        [HttpDelete, Route("carrier/issuePoint/{id}")]
        public HttpResponseMessage DeleteIssuePoint(long? id)
        {
            if (id == null)
            {
                throw new HttpResponseException(Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, "Punto emisión no existe", "El punto emisíon solicitado no existe"));
            }
            var issuePoint = _issuersService.GetIssuePointById(id.Value);
            if (issuePoint == null)
            {
                throw new HttpResponseException(Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, "Punto emisión no existe", "El punto emisíon solicitado no existe"));
            }
            var result = _issuersService.DeleteIssuePointCarrie(issuePoint.Entity);
            if (result.IsSuccess){
                return Request.CreateResponse(HttpStatusCode.OK);
                
            }
            return Request.BuildHttpErrorResponse(result.StatusCode, result.DevMessage, result.UserMessage);
        }

    }
}
