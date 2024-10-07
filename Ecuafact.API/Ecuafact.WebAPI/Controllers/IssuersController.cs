using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.SRI;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.Models.Authentication;
using ImageMagick;
using Newtonsoft.Json;
using Swashbuckle.Swagger;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// INFORMACION DEL EMISOR
    /// </summary> 
    [EcuafactAppAuthorize]
    public class IssuersController : ApiController
    {
        private static IIssuersService _issuersService;
        private static IUserService _userService;
        private static IAppService _appService;
        private static IPurchaseOrderService _purchaseOrderService;

        /// <summary>
        /// Informacion del Emisor
        /// </summary>
        /// <param name="issuersService"></param>
        public IssuersController(IIssuersService issuersService, IUserService userService, IAppService appService, IPurchaseOrderService purchaseOrderService)
        {
            _issuersService = issuersService;
            _userService = userService;
            _appService = appService;
            _purchaseOrderService = purchaseOrderService;
        }

        /// <summary>
        /// Obtener informacion de un emisor por el Numero de RUC
        /// </summary>
        /// <param name="ruc"></param>
        /// <returns></returns>
        [Route("issuers")]
        public IssuerDto GetIssuerByRUC(string ruc = null)
        {
            var username = this.GetAuthenticatedUser()?.Client?.Username;
            if (string.IsNullOrEmpty(ruc))
            {
                ruc = username ?? this.GetAuthenticatedUser()?.Issuers?.FirstOrDefault()?.RUC;
            }
            
            var permissions = _userService.HasPermissions(username, ruc);

            if (permissions.IsSuccess)
            { 
                var issuer = _issuersService.GetIssuer(ruc);
                var issuerDto = issuer?.ToIssuerDto();
                var elctsing = _purchaseOrderService.GetElectronicSignByRUC(ruc);
                if (elctsing.IsSuccess && issuerDto != null)
                {
                    issuerDto.viewRequests = elctsing.IsSuccess;
                }               
                
                return issuerDto;
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.Forbidden, permissions?.DevMessage ?? "No Autorizado", "Usted no tiene permisos para acceder a este emisor");
        }

        /// <summary>
        /// Obtener informacion de un emisor por ID Unico
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("issuers/{id}")]
        public IssuerDto GetIssuerById(long id)
        {
            var issuer = _issuersService.GetIssuer(id);

            return issuer.ToIssuerDto();
        }              

        /// <summary>
        /// Crear Emisor
        /// </summary>
        /// <remarks>
        /// Genera a partir de la información del SRI el perfil del RUC de una empresa.
        /// </remarks>
        /// <param name="model">Datos del nuevo emisor</param>
        /// <returns></returns>
        [HttpPost, Route("issuers")]
        public async Task<OperationResult<IssuerDto>> PostIssuer(IssuerCreateModel model)
        {
            try
            {
                var user = this.GetAuthenticatedUser();
                
                // Si es el propio usuario, no se valida el password del SRI.
                var IsValid = false;

                if ((model.RUC == user.Client.Username && user.Client.SRIConnected) || model.RUC == model.SRIPassword)
                {
                    IsValid = true;
                }
                else
                {
                    var login = await Request.LoginSRIAsync(model.RUC, model.SRIPassword);
                    IsValid = login.IsSuccess;
                }


                if (IsValid)
                {
                    // Verificamos si existe el emisor, entonces solamente se debe asignar los permisos al usuario actual.
                    var emisor = _issuersService.GetIssuer(model.RUC)?.ToIssuerDto();

                    if (emisor != null)
                    {
                        // Si ya existe el emisor: Solo asigno los permisos necesarios para este usuario
                        _userService.SetPermissions(user?.Client?.Username, emisor.Id, UserRolEnum.Issuer);
                    }
                    else
                    {
                        // Si no existe el emisor, cargamos los datos desde el SRI
                        var sri = _appService.SearchByRUC(model.RUC);

                        if (!sri.IsSuccess)
                        {
                            if (user.Client?.Username?.Substring(0, 10) == model.RUC.Substring(0, 10))
                            {
                                sri.Entity = new SRIContrib
                                {
                                    RUC = user.Client.Username.Length == 10 ? user.Client?.Username + "001" : user.Client?.Username,
                                    BusinessName = user.Client.BusinessName,
                                    TradeName = "",
                                    CreatedOn = DateTime.Today,
                                    Status = SRIContribStatus.Pasive,
                                    Establishments = new List<Establishment>
                                    {
                                        new Establishment
                                        {
                                            EstablishmentNumber = "1",
                                            CommercialName = "",
                                            Street = user.Client.StreetAddress
                                        }
                                    }
                                };
                            }
                            else
                            {
                                return new OperationResult<IssuerDto>(false, HttpStatusCode.NotFound, "El RUC especificado no pudo ser registrado. Comuniquese con Soporte Tecnico para realizar el registro.");
                            }
                        }

                        var issuer = sri.Entity.ToIssuer();
                        issuer.Email = user.Client.Email;
                        issuer.IsRimpe = _appService.IsRimpe(issuer.RUC).IsSuccess;

                        var result = _issuersService.AddIssuer(issuer, user?.Client?.Username);

                        if (!result.IsSuccess)
                        {
                            throw new Exception("Hubo un error al registrar el ruc especificado!");
                        }

                        emisor = result.Entity.ToIssuerDto();
                    }

                    return new OperationResult<IssuerDto>(true, HttpStatusCode.Created, emisor);
                }

                return new OperationResult<IssuerDto>(false, HttpStatusCode.Unauthorized, Constants.MSG_INVALIDCREDENTIALS);
            }
            catch (Exception ex)
            {
                Logger.Log($"Emisor.CREATE.{model?.RUC}.{DateTime.Now.ToFileTime()}", ex.ToString());
                return new OperationResult<IssuerDto>(ex);
            }
        }

        /// <summary>
        /// Metodo Antiguo para post "/emisor"
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>         
        private IHttpActionResult OldPostIssuer(IssuerRequestModel model)
        {
            try
            {
                var validation = HttpContext.Current.Session.GetAuthenticatedUser();

                if (!validation.IsSuccess)
                {
                    throw Request.BuildHttpErrorException(validation);
                }
                 
                if (model.IsSpecialContributor && !model.IsAccountingRequired)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest,
                        "Si usted es Contribuyente especial debe seleccionar obligado a llevar contabilidad");
                }

                if (model.IsSpecialContributor && string.IsNullOrEmpty(model.ResolutionNumber?.Trim()))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest,
                        "Si usted es Contribuyente especial debe especificar el numero de resolucion");
                }

                if (_issuersService.Exists(model.RUC))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest,
                        $"El emisor especificado ya existe [{model.RUC}] : {JsonConvert.SerializeObject(model)}",
                        $"Ya existe otro emisor con el RUC: {model.RUC}.");
                }

                var issuer = model.ToIssuer();
                issuer.Country = "ECUADOR";
                InstallFiles(model, issuer);

                if (string.IsNullOrEmpty(issuer.Logo))
                {
                    issuer.Logo = "no_logo.jpg";
                }


                // Solo se habilita el proceso de emision si se configura la firma electronica.
                // Caso contrario se crea al emisor deshabilitado.

                if (issuer.SetElectronicSign(model))
                {
                    issuer.IsEnabled = true; 
                }
                else
                {
                    issuer.IsEnabled = false;
                }

                var username = validation.Entity?.Client?.Username;
                var result = _issuersService.AddIssuer(issuer, username);

                if (result.IsSuccess)
                {
                    // Si el emisor no se ha registrado aún para recepción se crea una cuenta:
                    if (!ExistsAccount(result.Entity?.RUC))
                    {
                        Request.RegisterUser(result.Entity);
                    }

                    issuer.EmailActivation();

                    return Content(HttpStatusCode.Created, result.Entity.ToIssuerDto());
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
                var response = Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, ex.Message.ToString(), ex.Message);

                return ResponseMessage(response);
            }
        }

        private bool ExistsAccount(string ruc)
        {
            try
            {
                return (Request.GetIssuerToken(ruc) != null);
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Modifica la informacion del Emisor
        /// </summary>
        /// <param name="id">Id del Emisor</param>
        /// <param name="model">Informacion actualizada del Emisor</param>
        /// <returns></returns>
        [HttpPut, Route("issuers/{id}")] 
        public IHttpActionResult PutIssuer(long id, IssuerRequestModel model)
        {
            try
            {
                var validation = HttpContext.Current.Session.GetAuthenticatedUser();

                if (!validation.IsSuccess)
                {
                    throw Request.BuildHttpErrorException(validation);
                }
                 
                if (model.IsSpecialContributor && !model.IsAccountingRequired)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest,
                        "Si usted es Contribuyente especial debe seleccionar obligado a llevar contabilidad");
                }

                if (model.IsSpecialContributor && string.IsNullOrEmpty(model.ResolutionNumber?.Trim()))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest,
                        "Si usted es Contribuyente especial debe especificar el numero de resolucion");
                }


                if (!_issuersService.Exists(id))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.NotFound,
                        $"El emisor especificado no existe [{id}] : {JsonConvert.SerializeObject(model)}",
                        "No existe el emisor especificado.");
                }

                
                var issuer = _issuersService.GetIssuer(id);

                var isActive = issuer.IsEnabled;

                var original = issuer.ToIssuerDto();

                issuer.Update(model);

                if (!issuer.SetElectronicSign(model))
                {
                    // Rollback the certificate info when user does not require certificate.
                    issuer.Certificate = original.Certificate;
                    issuer.CertificatePass = original.CertificatePass;
                }

                InstallFiles(model, issuer);
                var username = validation.Entity?.Client?.Username;
                var result = _issuersService.UpdateIssuer(issuer, username);

                if (result.IsSuccess)
                {
                    // Si el emisor no se ha registrado aún para recepción se crea una cuenta:
                    if (!ExistsAccount(result.Entity?.RUC))
                    {
                        Request.RegisterUser(result.Entity);
                    }

                    if (!isActive && issuer.IsEnabled)
                    {
                        issuer.EmailActivation();
                    }

                    return Content(HttpStatusCode.OK, result.Entity.ToIssuerDto());
                }
                else
                {
                    throw Request.BuildHttpErrorException(result);
                }

            }
            catch (HttpResponseException ex)
            {
                Logger.Log($"Emisor.UPDATE.{model.RUC}.{DateTime.Now.ToFileTime()}", ex.Response.Content.ReadAsStringAsync().ToString());
                return ResponseMessage(ex.Response);
            }
            catch (Exception ex)
            {
                var response = Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, $"{ex.Message} {ex.InnerException?.Message}", ex.Message);

                return ResponseMessage(response);
            }
        }

        private void InstallFiles(IssuerRequestModel model, Issuer issuer)
        {
            // Cada vez que se sube un nuevo certificado debe validarse su autenticidad!
            if (issuer.IsEnabled && model.CertificateFile != null && model.CertificateFile.Length > 0)
            {
                issuer.Certificate = $"{issuer.RUC}_Firma.p12";

                if (issuer.SetElectronicSign(model))
                {
                    // Guardamos el archivo P12 válido
                    SaveFile(Path.Combine(Constants.EngineLocation, issuer.Certificate), model.CertificateFile);
                    SaveFile(Path.Combine(Constants.EngineLocation2, issuer.Certificate), model.CertificateFile);
                    issuer.IsEnabled = true;
                }
            }
             
            if (model.LogoFile != null && model.LogoFile.Length > 0)
            {
                issuer.Logo = $"{issuer.RUC}_Logo.jpg";
                SaveImage(Path.Combine(Constants.EngineLocation, issuer.Logo), model.LogoFile);
                SaveImage(Path.Combine(Constants.EngineLocation2, issuer.Logo), model.LogoFile);
            }           
        } 

        private static void SaveFile(string filename, byte[] file)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
            }

            File.WriteAllBytes(filename, file);
        }

        private static void SaveImage(string filename, byte[] file)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
            }

            using (var oMagickImage = new MagickImage(file))
            {
                var _size = new MagickGeometry(340, 151){
                    IgnoreAspectRatio = true
                };
                oMagickImage.Resize(_size);
                oMagickImage.Write(filename);
            }
        }

    }
}
