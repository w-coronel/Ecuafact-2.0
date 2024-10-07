using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Ecuafact.WebAPI.Controllers
{

    /// <summary>
    /// Benificiarios
    /// </summary>
    [EcuafactExpressAuthorize]
    //[DisplayName("Convenios")]
    public class AgreementController : ApiController
    {

        private readonly IAgreementService _agreementService;

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="agreementService"></param>
        public AgreementController(IAgreementService agreementService)
        {
            _agreementService = agreementService;            
        }

        /// <summary> 
        /// Obtener Benificiarios
        /// </summary>
        /// <remarks>
        ///     Obtiene los datos de un benificiario por  ID.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("beneficiary/{id}")]
        public BeneficiaryDto GetBeneficiary(long id)
        {
            try
            {
                var beneficiary = _agreementService.GetBeneficiary(id);                   

                if (beneficiary.IsSuccess)
                {
                    return beneficiary.Entity.ToBeneficiaryDto();
                }
            }
            catch (Exception) { }

            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, "No existe el benificiario solicitado", "No existe el benificiario solicitado");
        }

        /// <summary> 
        /// Obtener Benificiarios
        /// </summary>
        /// <remarks>
        ///     Obtiene los datos de un benificiario por  el codigo del convenio.
        /// </remarks>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet, Route("beneficiary/{code}")]
        public IEnumerable<BeneficiaryDto> GetBeneficiary(string code)
        {
            try
            {
                var veneficio = _agreementService.GetAgreementByCode(code);
                if (veneficio.IsSuccess)
                {
                    var beneficiary = _agreementService.GetBeneficiaryByAgreementId(veneficio.Entity.Id);

                    if (beneficiary.IsSuccess)
                    {
                        return beneficiary.Entity.Select(s => s.ToBeneficiaryDto());
                    }
                }

                
            }
            catch (Exception) { }

            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, "No existe el benificiario solicitado", "No existe el benificiario solicitado");
        }



        /// <summary>
        /// Crear Benificiario
        /// </summary>
        /// <remarks>
        ///     Agrega un nuevo benificiario para el convenio
        /// </remarks>
        /// <param name="request"></param>
        /// <returns>Return a HttpResponseMessage response</returns>
        [HttpPost, Route("Beneficiary")]
        [ResponseType(typeof(ContributorDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Petición Invalida!", typeof(OperationResult))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Se produjo un error al guardar el benificiario", typeof(OperationResult))]
        public HttpResponseMessage PostBeneficiary(BeneficiaryRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Hubo un error al guardar la solicitud. {error?.ErrorMessage}", error?.Exception?.ToString());
            }

            var convenio = _agreementService.GetReferenceCodeByAgreementCode(request.CodeAgreement);

            if (!convenio.IsSuccess)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.BadRequest, convenio.DevMessage, convenio.UserMessage);
            }

            var model = request.ToBeneficiary(convenio.Entity);
        
            var result = _agreementService.AddBeneficiary(model);

            if (!result.IsSuccess)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.BadRequest, result.DevMessage, result.UserMessage);
            }

            var response = Request.CreateResponse(HttpStatusCode.Created, result.Entity.ToBeneficiaryDto());
            return response;
        }
    }
}
