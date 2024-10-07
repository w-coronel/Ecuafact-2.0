using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models.Authentication;
using Ecuafact.WebAPI.Domain.Services;
using System.Web;
using System.Linq;
using System.Configuration;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.Filters;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Threading.Tasks;
using System.Web.Hosting;
using static Ecuafact.WebAPI.Models.Authentication.LoginResponseModel;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// SERVICIO DE SEGURIDAD
    /// </summary>
    [AllowAnonymous]
    
    public class SessionController : ApiController
    {
        private readonly IRequestSessionsService SessionService;
        private readonly IIssuersService IssuerService;
        private readonly IUserService IUserService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionService"></param>
        /// <param name="issuerService"></param>   
        /// <param name="iuserService"></param>
        public SessionController(IRequestSessionsService sessionService, IIssuersService issuerService, IUserService iuserService)
        {
            SessionService = sessionService;
            IssuerService = issuerService;
            IUserService = iuserService;
        }
        
        /// <summary>
        /// Proceso de autenticacion para utilizar los servicios, se requiere un token pre-establecido anteriormente.
        /// Se requiere el parametro -Authentication- para verificar la seguridad con el token encriptado de autenticacion.
        /// </summary> 
        [HttpPost]
        [Route("Authenticate")]
        public async Task<IHttpActionResult> Authenticate()
        {
            try
            {
                var securityToken = Request.Headers?.Authorization?.Parameter;
                if (string.IsNullOrEmpty(securityToken))
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, Constants.MSG_INVALIDCREDENTIALS);
                
                var result = await Request.PerformLoginAsync(securityToken);

                if (result.IsSuccess)
                {                   
                   
                    return Ok(result.Entity);
                }

                throw Request.BuildHttpErrorException(result);
            } 
            catch (HttpResponseException ex)
            {
                return ResponseMessage(ex.Response);
            }
            catch (Exception ex)
            {
                if (ex is EntityException || ex is DbException)
                {
                    return ResponseMessage(Request.BuildHttpErrorResponse(HttpStatusCode.ServiceUnavailable, ex.Message.ToString(), "Servicio no disponible."));
                }
                
                return ResponseMessage(Request.BuildHttpErrorResponse(HttpStatusCode.Unauthorized, ex.Message.ToString(), ex.Message));
            }
        }

        /// <summary>
        /// Obtiene la sesion requerida para el emisor del usuario autorizado.
        /// Se requiere el parametro -Authentication- para verificar la seguridad con el token provisto por el servicio de autenticacion.
        /// </summary>
        /// <param name="id">Emisor</param>
        /// <returns>Informacion para el uso dentro de la sesion accesos y token para el emisor.</returns>
        [EcuafactAppAuthorize]
        [Route("RequestSession")]
        public IHttpActionResult RequestSession(string id = null)
        {
            try
            {
                var userToken = Request.Headers?.Authorization?.Parameter;

                if (string.IsNullOrEmpty(userToken))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, Constants.MSG_INVALIDCREDENTIALS);
                }
                // Ahora buscamos al emisor para generar la sesion 
                // 1. Verificamos si el emisor esta autorizado: 
                var result = Request.ValidateClientToken(userToken);               
                if (result.IsSuccess)
                {                   
                    var credentials = result.Entity;
                    if (credentials.Client != null)
                    {
                        // Revisamos el id si es valido
                        if (string.IsNullOrEmpty(id))
                        {
                            id = credentials.Issuers?.Find(model => model.RUC.StartsWith(credentials.Client.Username))?.RUC;

                            if (string.IsNullOrEmpty(id))
                            {
                                id = credentials.Issuers?.FirstOrDefault()?.RUC;

                                if (string.IsNullOrEmpty(id))
                                {
                                    id = credentials.Client?.Username?.Trim();

                                    /////////// VALIDACION EN CASO DE QUE ID ENVIADO NO SEA RUC ////////////////
                                    int x; // Solo se usa para validar si es numero
                                    if (int.TryParse(id, out x))
                                    {
                                        // Agrega 001 si no es RUC
                                        id = $"{id}{(id.Length < 13 ? "" : "001")}";
                                    }
                                    ///////////////////////////////////////////////////////////////////////////
                                }
                            } 
                        }


                        if (!id.StartsWith(credentials.Client?.Username))
                        {
                            // Si el emisor no es el mismo cliente entonces
                            // asignamos el token:
                            userToken = Request.GetIssuerToken(id) ?? Request.GetIssuerToken(id.Substring(0, 10));

                            if (string.IsNullOrEmpty(userToken))
                            {
                                // Quiere decir que el usuario no esta registrado.
                                var issuer = IssuerService.GetIssuer(id);

                                // Entonces lo registramos:
                                var user = Request.RegisterUser(issuer);

                                if (user != null)
                                {
                                    userToken = user.Client.ClientToken;
                                }
                                else
                                {
                                    userToken = $"{Guid.NewGuid()}-{id}";
                                }
                            }
                        }

                        // 2.  Obtenemos el registro de la sesion del usuario
                        var session = SessionService.GenerateSession(credentials.Client.Username, id, userToken);

                        if (session != null)
                        {                            
                            var userSessionDto = session.ToSessionDto(result.Entity);                          
                            userSessionDto.Subscription = Request.ValidateActivationAccount(id).Entity;                            
                            Request.ValidateCertificado(userSessionDto.Issuer);
                            return Ok(userSessionDto);
                        }
                    }
                } 

                throw Request.BuildHttpErrorException(result);
                
            }
            catch (HttpResponseException ex)
            {
                return ResponseMessage(ex.Response);
            }
            catch (Exception ex)
            {
                var response = Request.BuildHttpErrorResponse(HttpStatusCode.Unauthorized, ex.ToString(), ex.GetMessage());

                return ResponseMessage(response);
            } 
        }


        /// <summary>
        /// Validacion del token generado por el cliente autenticado.
        /// Se requiere el parametro -Authentication- para verificar la seguridad con el token de seguridad.
        /// </summary>
        /// <returns></returns>
        [Route("ValidateToken")]
        public IHttpActionResult ValidateToken()
        {
            try
            {
                var securityToken = Request.Headers?.Authorization?.Parameter;

                if (string.IsNullOrEmpty(securityToken))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, Constants.MSG_INVALIDCREDENTIALS);
                }

                var result = Request.ValidateClientToken(securityToken);

                if (result.IsSuccess)
                {                  
                    return Ok<LoginResponseModel>(result.Entity);
                }
                else
                {
                    throw Request.BuildHttpErrorException(result);
                }
                
            }
            catch (HttpResponseException ex)
            {
                return ResponseMessage(ex.Response);
            }
            catch (Exception ex)
            {
                var response = Request.BuildHttpErrorResponse(HttpStatusCode.Unauthorized, ex.Message.ToString(), ex.Message);
                return ResponseMessage(response);
            }
        }

        /// <summary>
        /// Proceso de autenticacion para utilizar los servicios, se requiere un token pre-establecido anteriormente.
        /// Se requiere el parametro -Authentication- para verificar la seguridad con el token encriptado de autenticacion.
        /// <returns></returns>
        /// </summary> 
        [HttpPost]
        [EcuafactExpressAuthorize(AdminAccess = true)]
        [Route("ValidateSubscription")]
        public async Task<IHttpActionResult> ValidateSubscription(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    var Issuer = this.GetAuthenticatedIssuer();
                    id = Issuer?.RUC;
                }

                var result = Request.ValidateActivationAccount(id);

                if (result.IsSuccess)
                {
                    return Ok<SubscriptionDto>(result.Entity);
                }
                else
                {
                    throw Request.BuildHttpErrorException(result);
                }

            }
            catch (HttpResponseException ex)
            {
                return ResponseMessage(ex.Response);
            }
            catch (Exception ex)
            {
                var response = Request.BuildHttpErrorResponse(HttpStatusCode.Unauthorized, ex.Message.ToString(), ex.Message);
                return ResponseMessage(response);
            }
        }



        /// <summary>
        /// Proceso para registrar los usuarios que se registran por campañas.        
        /// </summary> 
        [HttpPost]
        [Route("usercampaign")]
        public async Task<IHttpActionResult> UserCampaign(UserCampaignsRequest request)
        {
            try
            {
                var result = IUserService.SetUserCampaigns(request.ToUserCampaigns());
                if (result.IsSuccess)
                {
                    return  Ok(result.Entity);
                }

                throw Request.BuildHttpErrorException(result);
            }
            catch (HttpResponseException ex)
            {
                return ResponseMessage(ex.Response);
            }
            catch (Exception ex)
            {
                var response = Request.BuildHttpErrorResponse(HttpStatusCode.Unauthorized, ex.Message.ToString(), ex.Message);
                return ResponseMessage(response);
            }
        }


        /// <summary>
        /// Permite registrar un usuario 
        /// </summary>
        /// <returns></returns>
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterClientModel model)
        {
            try
            {
                if(model == null)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, $"La información enviada es invalida y no se pudo procesar! Por favor revise la estructura. {model}", "La información enviada es invalida!");
                }

                if (!string.IsNullOrWhiteSpace(model.codProducto) && !string.IsNullOrWhiteSpace(model.numeroPedido)) {

                    await Request.RegisterUserPayment(model);
                }

                var result = await Request.RegisterUsers(model);               
                return Content(result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.InternalServerError, result.Entity);
            }
            catch (HttpResponseException ex)
            {
                return ResponseMessage(ex.Response);
            }
            catch (Exception ex)
            {
                var response = Request.BuildHttpErrorResponse(HttpStatusCode.Unauthorized, ex.Message.ToString(), ex.Message);
                return ResponseMessage(response);
            }
        }


        /// <summary>
        /// Proceso para desvincular un emisor de una cuenta        
        /// </summary> 
        [HttpPost]
        [Route("disassociate")]
        public async Task<OperationResult> UnlinkAccount(UserPermissionsModel model)
        {
            return await Request.RevokePermissions(model);
        }

    }
}