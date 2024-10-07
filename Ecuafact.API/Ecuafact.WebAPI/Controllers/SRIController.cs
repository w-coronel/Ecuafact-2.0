using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Domain.Entities.Engine;
using Ecuafact.WebAPI.Domain.Entities.SRI;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.Models.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Windows.Forms;

namespace Ecuafact.WebAPI.Controllers
{
    
    public class SRIController : ApiController
    {
        private IAppService AppService;

        public SRIController(IAppService appService)
        {
            AppService = appService;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("sri/ruc")]
        [EcuafactAppAuthorize]
        public async Task<OperationResult<SRIContrib>> GetRUC(RucRequestModel model)
        {
            try
            {
                if (model == null)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "Solicitud inválida!");
                }

                var result = AppService.SearchByRUC(model.RUC);

                if (result.IsSuccess)
                {
                    return (result);
                }

                throw Request.BuildHttpErrorException(result);
            }
            catch (HttpResponseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, ex.ToString(), ex.Message);
            }
        }
         
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("sri/connect")]
        public async Task<IHttpActionResult> Connect()
        {
            try
            {
                var securityToken = Request.Headers?.Authorization?.Parameter;

                if (string.IsNullOrEmpty(securityToken))
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, Constants.MSG_INVALIDCREDENTIALS);

                var result = await LoginAsync(securityToken);

                if (result.IsSuccess)
                {
                    var process = AppService.SRIPassword(result.Entity.Login.User, result.Entity.Login.Password);

                    if (process.IsSuccess)
                    {
                        return Ok(process);
                    }
                    else
                    {
                        throw Request.BuildHttpErrorException(process);
                    }
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

        [HttpPost]
        [Route("sri/validate-user")]
        public async Task<OperationResult<sriLoginResult>> LoginAsync(LoginInfo data)
        {
            try
            {
                // Validacion de Seguridad
                var securityToken = Request.Headers?.Authorization?.Parameter;

                if (string.IsNullOrEmpty(securityToken))
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, Constants.MSG_INVALIDCREDENTIALS);

                // Validamos que el token de seguridad pertenezca a algun usuario
                var security = Request.ValidateClientToken(securityToken);

                // Entonces validamos si es un servicio
                if (!security.IsSuccess && securityToken != Constants.ServiceToken)
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, Constants.MSG_INVALIDCREDENTIALS);
                
                return await Request.LoginSRIAsync(data.User, data.Password);
            }
            catch (Exception ex)
            {
                return new OperationResult<sriLoginResult>(ex);
            }
        }


        private async Task<OperationResult<LoginRequestModel>> LoginAsync(string token)
        {
            var data = Request.BasicAuthTokenDecode(token);
            var loginInfo = Request.ValidateClientToken(data?.Login?.User);
            //var loginInfo = await Request.PerformLoginAsync(token);

            if (loginInfo?.Entity?.Client == null || !loginInfo.IsSuccess)
            {
                return new OperationResult<LoginRequestModel>(false, HttpStatusCode.Unauthorized, "No Autorizado");
            }

            data.Login.User = loginInfo.Entity.Client.Username;

            var result = await Request.LoginSRIAsync(data?.Login?.User, data?.Login?.Password);

            if (result.IsSuccess)
            {
                return new OperationResult<LoginRequestModel>(true, HttpStatusCode.OK) { Entity = data };
            }

            return new OperationResult<LoginRequestModel>(false, HttpStatusCode.Unauthorized) { UserMessage = result?.UserMessage ?? "No autorizado" };
        }
    }
}