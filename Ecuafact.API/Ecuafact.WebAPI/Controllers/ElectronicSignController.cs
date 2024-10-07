using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using X.PagedList;

namespace Ecuafact.WebAPI.Controllers
{
    
    public class ElectronicSignController : ApiController
    {
        private IssuerDto Issuer =>this.GetAuthenticatedIssuer();
        private bool AdminAccess => this.IsAdmin();

        private IPurchaseOrderService __purchaseOrderService;
        private IEntityRepository<ElectronicSign> __esignRepository;
        private IIssuersService __issuerService;
        private IEntityRepository<ReferredUser> __referredRepository;
        private ISubscriptionService _subscriptionService;

        public ElectronicSignController(IPurchaseOrderService poService, IEntityRepository<ElectronicSign> esignRepository, 
            IIssuersService issuerService, IEntityRepository<ReferredUser> referredRepository, ISubscriptionService subscriptionService)
        {
            __purchaseOrderService = poService;
            __esignRepository = esignRepository;
            __issuerService = issuerService;
            __referredRepository = referredRepository;
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// Obtener Firma Electronica
        /// </summary>
        /// <remarks>
        /// Devuelve la firma electronica especificada.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize(AdminAccess=true)]
        [HttpGet, Route("electronicSign/{id}")]
        public async Task<OperationResult<ElectronicSign>> GetElectronicSign(long id)
        { 
            var result = __purchaseOrderService.GetElectronicSignById(id);

            if (result.IsSuccess)
            {
                if (AdminAccess || result.Entity.IssuerId == Issuer.Id || result.Entity.Identification.Substring(0, 10) == Issuer.RUC.Substring(0, 10))
                {
                    if (result.Entity.Status == ElectronicSignStatusEnum.Approved || result.Entity.Status == ElectronicSignStatusEnum.Error)
                    {
                        if (result.Entity.InvoiceId > 0)
                        {
                            return await __purchaseOrderService.StatusSolicitud(result.Entity);
                        }
                    }

                    return await Task.FromResult(result);
                }
            }
            
            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, "La firma electronica no existe!"); 
        }

        /// <summary>
        /// Buscar firmas electronicas
        /// </summary>
        /// <remarks>
        /// Permite la busqueda de firmas electronicas a través de una serie de filtros
        /// </remarks>
        /// <param name="search"></param>
        /// <param name="ruc"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize(AdminAccess = true)]
        [HttpGet, Route("electronicSign")]
        public IPagedList<ElectronicSign> SearchElectronicSign(string search = null, string ruc = null, DateTime? startDate = null, DateTime? endDate = null, ElectronicSignStatusEnum? status = null, int? page = 1, int? pageSize = null)
        {
            var list = __esignRepository.All;

            // Solo si no es acceso administrativo permite la busqueda de todas las firmas electronicas.
            if (!AdminAccess)
            {
                if (Issuer == null)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "No Autorizado");
                }

                list = list.Where(m => m.IssuerId == Issuer.Id || m.RUC == Issuer.RUC /*m.Identification.Substring(0, 10) == Issuer.RUC.Substring(0, 10)*/);
            }

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(p => p.Identification.Contains(search)
                                || p.FirstName.Contains(search)
                                || p.LastName.Contains(search)
                                || p.Email.Contains(search)
                                || p.Address.Contains(search));
            }

            if (!string.IsNullOrEmpty(ruc))
            {
                list = list.Where(p => p.Identification.Substring(0, 10) == ruc.Substring(0, 10));
            }

            if (startDate != null)
            {
                list = list.Where(p => p.CreatedOn >= startDate.Value);
            }

            if (endDate != null)
            {
                list = list.Where(p => p.CreatedOn <= endDate.Value);
            }

            if (status != null)
            {
                list = list.Where(p => p.Status == status);
            }

            page = page ?? 1;
            pageSize = pageSize ?? list.Count();

            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            return list.OrderByDescending(p => p.Id).ToPagedList(page.Value, pageSize.Value);
        }

        /// <summary>
        /// Procesar firma electronica
        /// </summary>
        /// <remarks>
        /// Permite procesar la solicitud de firma electronica y enviarla al proveedor de certificados digitales
        /// </remarks>
        /// <param name="electronicSignId"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize(AdminAccess=true)]
        [Route("electronicSign/{electronicSignId}/process")]
        public async Task<OperationResult<ElectronicSign>> ProcessElectronicSign(long electronicSignId)
        {
            var electronicSign = __purchaseOrderService.GetElectronicSignById(electronicSignId);

            if (electronicSign.IsSuccess)
            {
                if (AdminAccess || electronicSign.Entity.IssuerId == Issuer.Id)
                {
                    var elecSing = await __purchaseOrderService.Process(electronicSign.Entity);
                    return elecSing;
                }

                throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "Usted no esta autorizado para procesar esta solicitud!");
            }

            throw Request.BuildHttpErrorException(electronicSign);
        }

        /// <summary>
        /// Estado firma electronica
        /// </summary>
        /// <remarks>
        /// Permite ver el estado que se encuentra la firma electronica en el proveedor
        /// </remarks>
        /// <param name="electronicSignId"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize(AdminAccess = true)]
        [Route("electronicSign/{electronicSignId}/statusProcess")]
        public async Task<OperationResult<ElectronicSign>> StatusProcessElectronicSign(long electronicSignId)
        {
            var electronicSign = __purchaseOrderService.GetElectronicSignById(electronicSignId);

            if (electronicSign.IsSuccess)
            {
                if (AdminAccess || electronicSign.Entity.IssuerId == Issuer.Id)
                {
                    return await __purchaseOrderService.StatusProcess(electronicSign.Entity);
                }

                throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "Usted no esta autorizado para procesar esta solicitud!");
            }

            throw Request.BuildHttpErrorException(electronicSign);
        }


        /// <summary>
        /// Estado firma electronica
        /// </summary>
        /// <remarks>
        /// Permite ver el estado que se encuentra la firma electronica en el proveedor
        /// </remarks>        
        /// <returns></returns>
        [EcuafactExpressAuthorize(AdminAccess = true)]
        [Route("electronicSign/status/solicitud")]
        public async Task<OperationResult<List<ElectronicSign>>> StatusSolicitudsElectronicSign()
        {
            return await __purchaseOrderService.StatusProcessSolicituds();
        }

        /// <summary>
        /// Solicitud Firma Electronica
        /// </summary>
        /// <remarks>
        /// Permite solicitar una nueva firma electronica
        /// </remarks>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpPost, Route("electronicSign")]
        public ElectronicSign ElectronicSign(ElectronicSignRequest request)
        {
            if (request is null)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "La solicitud especificada es inválida");
            }

            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Hubo un error al guardar la solicitud. {error?.ErrorMessage}", error?.Exception?.ToString());
            }
            request.VerificationType = VerificationTypeEnum.Ecuanexus;
            request.SignatureValidy = SignatureValidyEnum.OneYear;
            request.FileFormat = FileFormatEnum.File;

            var result = SaveElectronicSign(request);
            if (result != null)
            {
                if (result.Entity != null)
                {

                    return result.Entity;
                }
                else
                {
                    throw Request.BuildHttpErrorException(result);
                }
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se pudo procesar la orden de compra");
        }



        /// <summary>
        /// Edición Firma Electronica
        /// </summary>
        /// <remarks>
        /// Permite editar la firma electronica
        /// </remarks>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpPut, Route("electronicSign/{id}")]
        public ElectronicSign ElectronicSign(long id, ElectronicSignRequest request)
        {
            if (request is null)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "La solicitud especificada es inválida");
            }

            var result = UpdateElectronicSign(id, request);

            if (result != null)
            {
                if (result.Entity != null)
                {

                    return result.Entity;
                }
                else
                {
                    throw Request.BuildHttpErrorException(result);
                }
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se pudo procesar la solicitud");
        }

        /// <summary>
        /// Firma Electronica pendiente
        /// </summary>
        /// <remarks>
        /// Devuelve la ultima firma electronica pendiente o en proceso.
        /// </remarks>
        /// <param name="ruc"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpGet, Route("electronicSign/pending")]
        public OperationResult<ElectronicSign> ExistsPendingOrder(string ruc)
        {
            if (string.IsNullOrEmpty(ruc))
            {
                ruc = Issuer?.RUC;
            }

            var result = __purchaseOrderService.GetElectronicSignByRUC(ruc, true);
            if (result.IsSuccess)
            {
                result.Entity.PurchaseOrder = __purchaseOrderService.GetPurchaseOrderById(result.Entity.PurchaseOrderId).Entity;               
            }

            return result;
        }

        /// <summary>
        /// Valida el email si existe en varias solicitudes
        /// </summary>
        /// <remarks>
        /// Devuelve la cantidad de solicitudes que se le asignado el email
        /// </remarks>
        /// <param name="email"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpGet, Route("electronicSign/{email}/requestedAmount")]
        public long ExistsEmailRequest(string email)
        {
            var ruc = Issuer?.RUC;
            return __purchaseOrderService.GetElectronicSignByEmail(ruc, email);            
        }

        /// <summary>
        /// Firma Electronica verificar estado en el proveedor
        /// </summary>
        /// <remarks>
        /// Devuelve la ultima firma electronica pendiente o en proceso.
        /// </remarks>
        /// <param name="ruc"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpGet, Route("electronicSign/status")]
        public async Task<OperationResult<ElectronicSign>> StatusElectronicSign(string ruc)
        {
            if (string.IsNullOrEmpty(ruc))
            {
                ruc = Issuer?.RUC;
            }            

            var esign = __esignRepository.FindBy(e=> e.RUC == ruc && e.InvoiceId > 0 && e.Status != ElectronicSignStatusEnum.Processed).OrderByDescending(e=> e.Id).FirstOrDefault();
            if (esign != null)
            {
                if(esign.Status == ElectronicSignStatusEnum.Approved || esign.Status == ElectronicSignStatusEnum.Error)
                {
                    if (!esign.RequestResult.Contains("Error al Procesar la Firma Electronica"))
                    {
                        return await __purchaseOrderService.StatusSolicitud(esign);
                    }
                }
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, "No hay solicitudes pendientes!");
        }

        [EcuafactExpressAuthorize]
        [HttpPost, Route("ElectronicSign/Config")]
        public HttpResponseMessage ConfigElectronicSign(CertificateConf request)
        {
            var response = new {result = false,
                               message = "Error al procesar la solicitud",
                               statusCode = HttpStatusCode.BadRequest };
            try
            {
                if (request != null)
                {                    
                    var issuer = __issuerService.GetIssuer(request.Ruc);
                    if (issuer != null)
                    {
                        try
                        {
                            if (issuer.SetElectronicSign(request.CertificateRaw, request.CertificatePass))
                            {
                                issuer.Certificate = $"{issuer.RUC}_Firma.p12"; ;
                                issuer.CertificatePass = request.CertificatePass;                               
                                __issuerService.UpdateIssuer(issuer);
                                SaveFile(issuer.Certificate, request.CertificateRaw, Constants.EngineLocation, true);
                                SaveFile(issuer.Certificate, request.CertificateRaw, Constants.EngineLocation2, true);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"ELECTRONICSIGN.CONFIG.{request.Ruc}.ERROR", $"Error al configurar la firma electronica en el emisor: {issuer.RUC} - {issuer.BussinesName}", ex);
                        }

                        response = new { result = true, message = "La firma electronica se configuro con exito!", statusCode = HttpStatusCode.OK };
                    }
                }
            }
            catch (Exception ex)
            {
                response = new  { result = false, message = $"Hubo un error al procesar su solicitud: {ex}", statusCode = HttpStatusCode.InternalServerError };
            }
            finally
            {
                Logger.Log($"ELECTRONICSIGN.CONFIG.{request.Ruc}", $" RUC: {request.Ruc}",
                    "Certificado:", JsonConvert.SerializeObject(request),
                    "Resultado: ", JsonConvert.SerializeObject(response));
            }

            return Request.CreateResponse(response.statusCode, response);
        }

        private OperationResult<ElectronicSign> SaveElectronicSign(ElectronicSignRequest request)
        {
            var validation = ValidateRequest(request);

            #region Validar modelo
            if (!validation.IsSuccess)
            {
                return new OperationResult<ElectronicSign>(validation)
                {
                    //DevMessage = $"{validation.DevMessage} :: {JsonConvert.SerializeObject(request)}"
                    DevMessage = $"{validation.DevMessage}"

                };
            }

            // Solo se puede procesar una solicitud por usuario
            //var exists = __purchaseOrderService.GetElectronicSignByRUC(request.Identification, true).IsSuccess;
            // Solo se puede procesar una solicitud por RUC
            var exists = __purchaseOrderService.GetElectronicSignByRUC(request.RUC, true).IsSuccess;
            if (exists)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.Conflict,
                    $"Ya existe otra solicitud de firma electronica pendiente para el RUC # {request.RUC}");
            }

            #endregion

            #region Variables
            decimal discount = 0;
            decimal totalDiscount = 0;
            long benReferenCodId = 0;
            bool omitirPago = false;
            bool generarFactura = false;
            bool facturaConvenio = false;
            string facturaRuc = "";
            string refCode = "";
            BeneficiaryReferenceCode descuentoCod = null;
            ReferenceCodes codPromocional = null;
            #endregion

            #region Firma electronica

            // Primero Guardamos los documentos usando la identidad del usuario
            var basename = $"{request.Identification}";
            var cedFrontFileName = GetFilename(basename, "CEDF", "png"); // cedula frontal type png
            var cedBackFileName = GetFilename(basename, "CEDB", "png"); // cedula reverso type png
            var selfieName = GetFilename(basename, "SELF", "png");  // selfie con la cedula type png
            var cedFileName = GetFilename(basename, "CED"); // Cedula type pdf
            var rucFileName = GetFilename(basename, "RUC"); // Ruc type pdf
            var autFileName = GetFilename(basename, "AUT");  // autorizacion e representante legal type pdf
            var desFileName = GetFilename(basename, "DES"); // nombramiento type pdf
            var conFilename = GetFilename(basename, "CON"); // constitucion type pdf
            var votFileName = GetFilename(basename, "VOT"); // voleta de votación del RL type pdf
            var authorizationFileName = GetFilename(basename, "EAUT"); // carta de autización 
            var authorizationAgeFileName = ""; // Video de autorizacion mayores a 75 años

            var eSign = new ElectronicSign
            {
                Identification = request.Identification,
                DocumentType = request.DocumentType,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Email2 = request.Email2,
                Address = request.Address,
                CedulaFrontFile = cedFrontFileName,
                CedulaBackFile = cedBackFileName,
                SelfieFile = selfieName,
                RucFile = rucFileName,
                ConstitutionFile = conFilename,
                DesignationFile = desFileName,
                ElectionsTicketFile = votFileName,
                IdentificationCardFile = cedFileName,
                AuthorizationLegalRepFile = autFileName,
                AuthorizationFile = authorizationFileName,
                RUC = request.RUC,
                BusinessName = request.BusinessName,
                BusinessAddress = request.BusinessAddress,
                City = request.City,
                Province = request.Province,
                Country = request.Country,
                Phone = request.Phone,
                Phone2 = request.Phone2,
                FileFormat = request.FileFormat,
                SignatureValidy = request.SignatureValidy,
                SignType = request.SignType,
                Skype = request.Skype,
                VerificationType = request.VerificationType,
                CreatedOn = DateTime.Now,
                Status = ElectronicSignStatusEnum.Saved,
                IssuerId = Issuer.Id,
                Sexo = request.Sexo,
                BirthDate = ((int)SupplierElectronicSignEnum.Uanataca == Constants.ElectronicSign.SupplierElectronicSign) ? request.BirthDate : "1900/01/01",
                WorkPosition = ((int)SupplierElectronicSignEnum.Uanataca == Constants.ElectronicSign.SupplierElectronicSign) ? request.WorkPosition : "Representante Legal",
                ActivateSubscription = true,
                LicenceTypeId = request.LicenceTypeId,
                FingerPrintCode = request.FingerPrintCode

            };
            #endregion

            #region Renovaciones vigentes
            //Validar si hay una renovacion vigente cancelada para relacionar el pago con la solicitud de firma electronoca y enviar  a procesar

            var purchaseSubscriptionPayed = _subscriptionService.GetPurchaseSubscriptionActive(request.RUC);
            if (purchaseSubscriptionPayed.IsSuccess)
            {
                eSign.PurchaseOrder = purchaseSubscriptionPayed.Entity.PurchaseOrder;
                eSign.InvoiceId = purchaseSubscriptionPayed.Entity.InvoiceId;
                eSign.InvoiceNumber = purchaseSubscriptionPayed.Entity.InvoiceNumber;
                eSign.InvoiceResult = purchaseSubscriptionPayed.Entity.InvoiceResult;
                eSign.InvoiceDate = purchaseSubscriptionPayed.Entity.InvoiceDate;
                omitirPago = true;
                eSign.ActivateSubscription = false;
            }
            else
            {

                #region Codigo de prpmoción o descuento
                //Verifica si el usuario en vía código promocional
                if (!string.IsNullOrEmpty(request.InvoiceInfo.DiscountCode))
                {
                    var referenceCode = __purchaseOrderService.GetReferenceCodes(request.InvoiceInfo.DiscountCode);
                    if (referenceCode.IsSuccess)
                    {
                        codPromocional = referenceCode.Entity;
                        discount = codPromocional.DiscountRate;
                        omitirPago = codPromocional.SkipPaymentOrder;
                        generarFactura = omitirPago;                       
                        refCode = codPromocional.Code;
                        var agreementInvoice = codPromocional?.AgreementInvoice ?? false;
                        if (agreementInvoice && !string.IsNullOrEmpty(codPromocional?.RUC))
                        {
                            request.InvoiceInfo.Name = codPromocional.BusinessName;
                            request.InvoiceInfo.Identification = codPromocional.RUC;
                            request.InvoiceInfo.Address = codPromocional.Address;
                            request.InvoiceInfo.Email = codPromocional.Email;
                            request.InvoiceInfo.Phone = codPromocional.Phone;
                        }
                    }
                }
                else
                {
                    //Verifica si el Ruc tiene codigo de promoción o descuento 
                    var resultDescuentoCod = __purchaseOrderService.GetBeneficiaryReferenceCode(request.RUC);
                    if (resultDescuentoCod.IsSuccess)
                    {
                        descuentoCod = resultDescuentoCod.Entity;
                        discount = descuentoCod.ReferenceCodes.DiscountRate;
                        benReferenCodId = descuentoCod.Id;
                        omitirPago = descuentoCod.ReferenceCodes.SkipPaymentOrder;
                        generarFactura = omitirPago;
                        var agreementInvoice = descuentoCod?.ReferenceCodes?.AgreementInvoice ?? false;
                        if (agreementInvoice && !string.IsNullOrEmpty(descuentoCod?.ReferenceCodes?.RUC))
                        {
                            request.InvoiceInfo.Name = descuentoCod.ReferenceCodes.BusinessName;
                            request.InvoiceInfo.Identification = descuentoCod.ReferenceCodes.RUC;
                            request.InvoiceInfo.Address = descuentoCod.ReferenceCodes.Address;
                            request.InvoiceInfo.Email = descuentoCod.ReferenceCodes.Email;
                            request.InvoiceInfo.Phone = descuentoCod.ReferenceCodes.Phone;
                        }
                    }
                }

                //se realiza los respectivos calculos del valor de la solicitud de firma electronica
                decimal precioFirma = request.InvoiceInfo.Price;
                if (discount > 0)
                {
                    totalDiscount = decimal.Round(precioFirma * (discount / 100), 2);
                    precioFirma -= decimal.Round(precioFirma * (discount / 100), 2);
                }
                var ivaProduct = decimal.Round(precioFirma * 0.12M, 2);
                var total = precioFirma + ivaProduct;

                #endregion

                #region Facturar la solicitud a nombre de:
                //verifica si la orden y la factura se le hace a la entidad del convenio y no al emisor que esta solicitando la firma
                
                #endregion

                #region generamos la orden de compra 

                if (eSign.PurchaseOrder == null)
                {
                    var order = new PurchaseOrder
                    {
                        BusinessName = request.InvoiceInfo?.Name ?? request.BusinessName,
                        Identification = request.InvoiceInfo?.Identification ?? request.RUC ?? request.Identification,
                        Address = request.InvoiceInfo?.Address ?? request.BusinessAddress,
                        Email = request.InvoiceInfo?.Email ?? request.Email,
                        Phone = request.InvoiceInfo?.Phone ?? request.Phone,
                        IssuedOn = DateTime.Today,
                        Products = request.InvoiceInfo.Product ?? "Firma Electrónica",
                        Subtotal0 = 0,
                        Subtotal12 = precioFirma,
                        IVA = ivaProduct,
                        ICE = 0,
                        Interests = 0,
                        Additional = 0,
                        Total = total,
                        ZIP = "000",
                        Status = PurchaseOrderStatusEnum.Saved,
                        IssuerId = Issuer.Id,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        City = request.City,
                        Province = request.Province,
                        Country = request.Country,
                        TotalDiscount = totalDiscount,
                        Discount = discount,
                        BeniReferCodeId = benReferenCodId,
                        CreatedOn = DateTime.Now,
                        DiscountCode = refCode
                    };

                    eSign.PurchaseOrder = order;
                }

                #endregion
            }
            #endregion

            #region Generar la solciitud de firma electrónica
            // Un solo metodo de grabacion asegura que se guarde la informacion completa:
            var result = __purchaseOrderService.BuyElectronicSign(eSign);

            if (result.IsSuccess)
            {
                SaveFile(cedFrontFileName, request.CedulaFrontRaw, request.Identification);
                SaveFile(cedBackFileName, request.CedulaBackRaw, request.Identification);
                SaveFile(selfieName, request.SelfieRaw, request.Identification);
                SaveFile(cedFileName, request.IdentificationCardRaw, request.Identification);
                SaveFile(rucFileName, request.RucRaw, request.Identification);
                SaveFile(autFileName, request.AuthorizationLegalRepRaw, request.Identification);
                SaveFile(rucFileName, request.ElectionsTicketRaw, request.Identification);
                SaveFile(desFileName, request.DesignationRaw, request.Identification);
                SaveFile(conFilename, request.ConstitutionRaw, request.Identification);
                if (request.AuthorizationRaw?.Length > 0) 
                {                   
                    SaveFile(authorizationFileName, request.AuthorizationRaw, request.Identification);
                }
                if(request.AuthorizationAgeRaw?.Length > 0)
                {
                    authorizationAgeFileName = GetFilename(basename, "AAGE", request.AuthorizationAgeFormat);
                    SaveFile(authorizationAgeFileName, request.AuthorizationAgeRaw, request.Identification);
                    eSign.AuthorizationAgeFile = authorizationAgeFileName;
                }

                // omitir la pasarela de pago y generación de la factura
                if (omitirPago)
                {
                    if (descuentoCod != null)
                    {
                        eSign.PurchaseOrder.ReferenceCodes = descuentoCod?.ReferenceCodes;
                    }
                    else if (codPromocional != null)
                    {
                        eSign.PurchaseOrder.ReferenceCodes = codPromocional;
                    }

                    //Se envia a procesar la solcitud de firma
                    __purchaseOrderService.ElecsignProcess(eSign);

                    //Permitie generar la factura de la solicitud 
                    __purchaseOrderService.GenerateInvoices(eSign);

                    //Se actualiza la orden de compra de la suscripción, como solicitud de firma enviada y procesada
                    if (purchaseSubscriptionPayed.IsSuccess)
                    {
                        purchaseSubscriptionPayed.Entity.RequestElectronicSign = RequestElectronicSignEnum.Sent;
                        purchaseSubscriptionPayed.Entity.RequestElectronicSignMsg = RequestElectronicSignEnum.Sent.GetDisplayValue();
                        _subscriptionService.UpdatePurchaseSubscription(purchaseSubscriptionPayed.Entity);
                    }

                    //Actualiza el usuario benificiiario del codigo promocional
                    if (descuentoCod != null)
                    {
                        descuentoCod.IssuerId = eSign.IssuerId;
                        __purchaseOrderService.UpdateBeneficiaryReferenceCode(descuentoCod);
                    }
                }
                //Regsitra el usuario benificiiario del codigo promocional
                if (codPromocional != null)
                {
                    var model = new BeneficiaryReferenceCode()
                    {
                        DiscountCode = codPromocional.Code,
                        Identification = request.RUC,
                        ReferenceCodId = codPromocional.Id,
                        IssuerId = Issuer.Id,
                        Status = ReferenceCodeStatusEnum.Applied,
                        StatusMsg = $"Aplicado:Código de descuento por {codPromocional.Description}",
                        CreatedOn = DateTime.Now
                    };
                    __purchaseOrderService.AddBeneficiaryReferenceCode(model);
                }

            }

            #endregion

            return result;
        }

        private OperationResult<ElectronicSign> UpdateElectronicSign(long id, ElectronicSignRequest request)
        {

            var validation = UpdateValidateRequest(request);
            if (!validation.IsSuccess)
            {
                return new OperationResult<ElectronicSign>(validation)
                {                    
                    DevMessage = $"{validation.DevMessage}",
                    UserMessage = $"{validation.DevMessage}",
                };
            }

            var _eSign = __purchaseOrderService.GetElectronicSignById(id);
            if (_eSign.IsSuccess)
            {
                // Primero Guardamos los documentos usando la identidad del usuario

                var basename = $"{request.Identification}";                

                if (request.CedulaFrontRaw != null && request.CedulaFrontRaw.Length > 0)
                    _eSign.Entity.CedulaFrontFile = GetFilename(basename, "CEDF", "png");
                if (request.CedulaBackRaw != null && request.CedulaBackRaw.Length > 0)
                    _eSign.Entity.CedulaBackFile = GetFilename(basename, "CEDB", "png");
                if (request.SelfieRaw != null && request.SelfieRaw.Length > 0)
                    _eSign.Entity.SelfieFile = GetFilename(basename, "SELF", "png");
                if (request.RucRaw != null && request.RucRaw.Length > 0)
                    _eSign.Entity.RucFile = GetFilename(basename, "RUC");
                if (request.ConstitutionRaw != null && request.ConstitutionRaw.Length > 0)
                    _eSign.Entity.ConstitutionFile = GetFilename(basename, "CON");
                if (request.DesignationRaw != null && request.DesignationRaw.Length > 0)
                    _eSign.Entity.DesignationFile = GetFilename(basename, "DES");
                if (request.AuthorizationRaw != null && request.AuthorizationRaw.Length > 0)
                    _eSign.Entity.AuthorizationFile = GetFilename(basename, "EAUT");
                if (request.AuthorizationAgeRaw  != null && request.AuthorizationAgeRaw.Length > 0)
                    _eSign.Entity.AuthorizationAgeFile = GetFilename(basename, "AAGE", request.AuthorizationAgeFormat);


                _eSign.Entity.Identification = request.Identification;
                _eSign.Entity.DocumentType = request.DocumentType;
                _eSign.Entity.FirstName = request.FirstName;
                _eSign.Entity.LastName = request.LastName;
                _eSign.Entity.Email = request.Email;
                _eSign.Entity.Email2 = request.Email2;
                _eSign.Entity.Address = request.Address;
                _eSign.Entity.Phone = request.Phone;
                _eSign.Entity.Phone2 = request.Phone2;
                _eSign.Entity.Sexo = request.Sexo;
                _eSign.Entity.BirthDate = request.BirthDate;
                _eSign.Entity.WorkPosition = request.WorkPosition;
                _eSign.Entity.FileFormat = FileFormatEnum.File;
                _eSign.Entity.SignatureValidy = SignatureValidyEnum.OneYear;
                _eSign.Entity.FingerPrintCode = request.FingerPrintCode;
                _eSign.Entity.SignType = request.SignType;
                //_eSign.Entity.Status = _eSign.Entity?.InvoiceId == 0 ? ElectronicSignStatusEnum.Saved : _eSign.Entity.Status;


                // Un solo metodo de grabacion asegura que se guarde la informacion completa:
                var result = __purchaseOrderService.UpdateElectronicSign(_eSign.Entity);

                if (result.IsSuccess)
                {
                    if (request.CedulaFrontRaw != null && request.CedulaFrontRaw.Length > 0)
                        SaveFile(result.Entity.CedulaFrontFile, request.CedulaFrontRaw, result.Entity.Identification);
                    if (request.CedulaBackRaw != null && request.CedulaBackRaw.Length > 0)
                        SaveFile(result.Entity.CedulaBackFile, request.CedulaBackRaw, result.Entity.Identification);
                    if (request.SelfieRaw != null && request.SelfieRaw.Length > 0)
                        SaveFile(result.Entity.SelfieFile, request.SelfieRaw, result.Entity.Identification);
                    if (request.RucRaw != null && request.RucRaw.Length > 0)
                        SaveFile(result.Entity.RucFile, request.RucRaw, result.Entity.Identification);
                    if (request.ConstitutionRaw != null && request.ConstitutionRaw.Length > 0)
                        SaveFile(result.Entity.ConstitutionFile, request.DesignationRaw, result.Entity.Identification);
                    if (request.DesignationRaw != null && request.DesignationRaw.Length > 0)
                        SaveFile(result.Entity.DesignationFile, request.ConstitutionRaw, result.Entity.Identification);
                    if (request.AuthorizationRaw != null && request.AuthorizationRaw.Length > 0)
                        SaveFile(result.Entity.AuthorizationFile, request.AuthorizationRaw, result.Entity.Identification);
                    if (request.AuthorizationAgeRaw != null && request.AuthorizationAgeRaw.Length > 0)
                        SaveFile(result.Entity.AuthorizationAgeFile, request.AuthorizationAgeRaw, result.Entity.Identification);


                    if (result.Entity.Status == ElectronicSignStatusEnum.Error && result.Entity.RequestResult != null)
                    {
                        if (result.Entity.InvoiceId > 0 && result.Entity.RequestResult.Contains("Error al Procesar la Firma Electronica"))
                        {
                             __purchaseOrderService.ElecsignProcess(result.Entity);                                         
                        }
                    }
                    else
                    {
                        _eSign.Entity.Status = _eSign.Entity.InvoiceId > 0 ? ElectronicSignStatusEnum.Approved : ElectronicSignStatusEnum.Saved;
                        __purchaseOrderService.UpdateElectronicSign(_eSign.Entity);
                    }
                }

                return result;
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.Conflict, _eSign.DevMessage);
        }

        private OperationResult ValidateRequest(ElectronicSignRequest request)
        {
            if (request == null)
            {
                return new OperationResult(false, HttpStatusCode.BadRequest, "La solicitud especificada no es válida")
                {
                    DevMessage = $"La solicitud especificada no es válida: {JsonConvert.SerializeObject(request)}"
                };
            }

            // Documentos necesarios para el proveedor Uanataca
            if ((int)SupplierElectronicSignEnum.Uanataca == Constants.ElectronicSign.SupplierElectronicSign)
            {

                if (!FileIsImage(request.CedulaFrontRaw))
                {
                    return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Cédula frontal] no es un archivo .JPJ o PNG válido");
                }

                if (!FileIsImage(request.CedulaBackRaw))
                {
                    return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Cédula reverso] no es un archivo .JPJ o PNG válido");
                }                
            }  
            
            // Documentos necesarios para el proveedor GSV
            if ((int)SupplierElectronicSignEnum.GSV == Constants.ElectronicSign.SupplierElectronicSign)
            {

                if (!FileIsPDF(request.IdentificationCardRaw))
                {
                    return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Cédula de Identidad del Representante Legal] no es un archivo .PDF válido");
                }

                if (!FileIsPDF(request.AuthorizationLegalRepRaw))
                {
                    return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Autorización para la Emisión de Firma Electrónica] no es un archivo .PDF válido");
                }

                if (!FileIsPDF(request.ElectionsTicketRaw))
                {
                    return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Certificado de Votación del Representante Legal] no es un archivo .JPJ o PNG válido");
                }
            }

            if (!FileIsImage(request.SelfieRaw))
            {
                return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Selfie] no es un archivo .JPJ o PNG válido");
            }

            if (!FileIsPDF(request.RucRaw))
            {
                return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Copia del RUC] no es un archivo .JPJ o PNG válido");
            }

            if (request.SignType == RucTypeEnum.Juridical)
            {
                if (!FileIsPDF(request.ConstitutionRaw))
                {
                    return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Copia del Estatuto de Constitución Jurídica] no es un archivo .PDF válido");
                }

                if (!FileIsPDF(request.DesignationRaw))
                {
                    return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Copia del Nombramiento del Representante Legal] no es un archivo .PDF válido");
                }
            }         

            return new OperationResult(true);
        }

        private OperationResult UpdateValidateRequest(ElectronicSignRequest request)
        {
            if (request == null)
            {
                return new OperationResult(false, HttpStatusCode.BadRequest, "La solicitud especificada no es válida")
                {
                    DevMessage = $"La solicitud especificada no es válida: {JsonConvert.SerializeObject(request)}"
                };
            }

            if(request.CedulaFrontRaw != null && request.CedulaFrontRaw.Length > 0)
            {
                if (!FileIsImage(request.CedulaFrontRaw))
                {
                    return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Cédula frontal] no es un archivo .JPJ o PNG válido");
                }
            }

            if (request.CedulaBackRaw != null && request.CedulaBackRaw.Length > 0)
            {
                if (!FileIsImage(request.CedulaBackRaw))
                {
                    return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Cédula reverso] no es un archivo .JPJ o PNG válido");
                }
            }

            if (request.SelfieRaw != null && request.SelfieRaw.Length > 0)
            {
                if (!FileIsImage(request.SelfieRaw))
                {
                    return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Selfie] no es un archivo .JPJ o PNG válido");
                }
            }

            if (request.RucRaw != null && request.RucRaw.Length > 0)
            {
                if (!FileIsPDF(request.RucRaw))
                {
                    return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Copia del RUC] no es un archivo .JPJ o PNG válido");
                }
            }


            if (request.SignType == RucTypeEnum.Juridical)
            {
                if (request.ConstitutionRaw != null && request.ConstitutionRaw.Length > 0)
                {
                    if (!FileIsPDF(request.ConstitutionRaw))
                    {
                        return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Copia del Estatuto de Constitución Jurídica] no es un archivo .PDF válido");
                    }
                }

                if (request.DesignationRaw != null && request.DesignationRaw.Length > 0)
                {
                    if (!FileIsPDF(request.DesignationRaw))
                    {
                        return new OperationResult(false, HttpStatusCode.BadRequest, "El documento adjunto para [Copia del Nombramiento del Representante Legal] no es un archivo .PDF válido");
                    }
                }                
            }



            return new OperationResult(true);
        }

        private bool FileIsPDF(byte[] file)
        {
            // Si no es Nulo y es PDF
            // Los Bits son:
            // 0 : 37 : %
            // 1 : 80 : P
            // 2 : 68 : D
            // 3 : 70 : F
            return (file != null && file.Length > 4 && file[0] == 37 && file[1] == 80 && file[2] == 68 && file[3] == 70);
        }

        private bool FileIsImage(byte[] file)
        {
            // Si no es Nulo y es PDF
            // Los Bits son:
            var png = new byte[] { 137, 80, 78, 71 };    // png
            var png2 = new byte[] { 0, 0, 0, 36, 102 };    // png
            var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
            var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon
            var jpeg3 = new byte[] { 0, 0, 0, 40, 102 }; // jpeg3 
            var jpeg4 = new byte[] { 255, 216, 255, 219 }; // jpeg4 

            if (png.SequenceEqual(file.Take(png.Length)))
                return true;
            if (png2.SequenceEqual(file.Take(png2.Length)))
                return true;
            if (jpeg.SequenceEqual(file.Take(jpeg.Length)))
                return true;
            if (jpeg2.SequenceEqual(file.Take(jpeg2.Length)))
                return true;
            if (jpeg3.SequenceEqual(file.Take(jpeg3.Length)))
                return true;
            if (jpeg4.SequenceEqual(file.Take(jpeg4.Length)))
                return true;

            return false;
        }

        private decimal GetPrice(ElectronicSignRequest request)
        {
            var price = Constants.ElectronicSign.Price;
             
            var referred =  __referredRepository.All.FirstOrDefault(p => p.RUC.Contains(request.Identification));

            if (referred != null)
            {
                if (referred.SpecialPrice > 0)
                {
                    price = referred.SpecialPrice;
                }
                else
                {
                    if (referred.Discount>0)
                    {
                        price = price - decimal.Round(price * referred.Discount, 2);
                    }
                }
            }

            if (price == 0)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.ServiceUnavailable,
                    "No se puede procesar la solicitud porque no se ha configurado el precio para el producto!",
                    "No se puede procesar la solicitud");
            }

            return decimal.Round(price, 2);
        }

        private static string GetFilename(string id, string type, string extension = "pdf") => $"{id}_{type}.{extension}";

        private string SaveFile(string fname, string text, string path = null, bool backup = false)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return SaveFile(fname, bytes, path, backup);
        }

        private string SaveFile(string fname, byte[] file, string path = null, bool backup = false)
        {
            
            try
            {
                if (path == null)
                {
                    path = Constants.ResourcesLocation;
                }
                // Si es un subdirectorio de recursos entonces lo configuramos:
                else if(!path.Contains("\\") && !path.Contains("/"))
                {
                    path = Path.Combine(Constants.ResourcesLocation, path);
                }

                if (!string.IsNullOrEmpty(fname))
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var filename = Path.Combine(path, fname);

                    if (file != null && file.Length > 0)
                    {
                        if (backup && File.Exists(filename))
                        {
                            // Guarda una copia en caso de que ya exista alguna firma:
                            var backupfile = Path.Combine(path, $"backup-{DateTime.Now.ToFileTime()}-{fname}");
                            File.Copy(filename, backupfile, true);
                        }

                        File.WriteAllBytes(filename, file);
                    }

                    // Si se logro guardar el archivo se devuelve el nombre del archivo:
                    return fname;
                }
            }
            catch (Exception) { }

            return default;
        }

       

        /// <summary>
        /// [Internal] Uanataca: Realiza la carga de la firma electronica por el Servicio de Emision de la firma electrónica.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
#if DEBUG
        [ApiExplorerSettings(IgnoreApi = true)]
#endif
        [AllowAnonymous]
        [HttpPost]
        [Route("ElectronicSign/Upload")]
        public HttpResponseMessage UploadElectronicSign(ElectronicSignFileRequest request)
        {
            // Error predeterminado:
            var objResponse = new UploadElectronicSignResult
            {
                result = false,
                message = "Los datos enviados son inválidos",
                statusCode = HttpStatusCode.BadRequest
            };

            try
            {
                if (request != null)
                {
                    if (Constants.Apikey == request?.apikey && Constants.Uid == request?.uid)
                    {
                        // se decodifica la clave del certificado electronico
                        var base64EncodedBytes = System.Convert.FromBase64String(request.pss);
                        var claveCertificado = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                        // La idea es que se guarde en un registro de log de archivos recibidos, esten o no registrados.
                        var result = __purchaseOrderService.GetElectronicSignByToken(request.token);                        
                        if (result.IsSuccess)
                        {
                            string msg;
                            // Luego se actualiza la firma electronica.
                            var esign = result.Entity;
                            var certfilename = GetFilename(esign.RUC, "firma", "p12");
                            var passfilename = GetFilename(esign.RUC, "clave", "txt");
                            // Guardamos el archivo recibido y previamente se realiza su backup.
                            SaveFile(passfilename, claveCertificado, esign.RUC, true);
                            SaveFile(certfilename, request.p12, esign.RUC, true);
                            SaveFile(certfilename, request.p12, Constants.EngineLocation, true);
                            SaveFile(certfilename, request.p12, Constants.EngineLocation2, true);

                            if (request.p12 != null && request.p12.Length > 0)
                            {
                                esign.CertificateFile = certfilename;
                                esign.CertificatePass = claveCertificado;                                
                                esign.ReceivedDate = DateTime.Now;
                                esign.Status = ElectronicSignStatusEnum.Processed;
                                // Ahora se tiene que realizar la instalación del archivo P12 en el Emisor de Ecuafact.
                                var issuer = __issuerService.GetIssuer(result.Entity.RUC)?? __issuerService.GetIssuer(result.Entity.IssuerId);
                                if (issuer != null)
                                {
                                    try
                                    {
                                        if (issuer.SetElectronicSign(request.p12, claveCertificado))
                                        {
                                            issuer.Certificate = certfilename;
                                            issuer.CertificatePass = claveCertificado;
                                            esign.CertificateExpirationDate = issuer.CertificateExpirationDate;
                                            __issuerService.UpdateIssuer(issuer);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Log($"ELECTRONICSIGN.UPLOAD.{esign.RUC}.ERROR",$"Error al configurar la firma electronica en el emisor: {issuer.RUC} - {issuer.BussinesName}", ex);
                                    }
                                }
                            }
                            else
                            {
                                Logger.Log($"ELECTRONICSIGN.UPLOAD.{esign.RUC}.ERROR",$"Error al configurar la firma electronica en el emisor: {esign.Identification} - {esign.BusinessName}");                               
                            }
                            HostingEnvironment.QueueBackgroundWorkItem(ct => SendMail(esign));                          
                            msg = "La firma electronica se ha guardado con exito!";
                            objResponse = new UploadElectronicSignResult { result = true, message = msg, statusCode = HttpStatusCode.OK};
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.token) && result.StatusCode == HttpStatusCode.NotFound)
                            {
                                objResponse = new UploadElectronicSignResult { result = true, message = "La firma electronica se ha recibido pero no se encontro la solicitud especificada!", statusCode = HttpStatusCode.Created};
                            }
                            else
                            {
                                objResponse = new UploadElectronicSignResult { result = false, message = result.DevMessage, statusCode = result.StatusCode};
                            } 
                        }
                    }
                    else
                    {
                        objResponse = new UploadElectronicSignResult { result = false, message = $"No Autorizado", statusCode = HttpStatusCode.Unauthorized }; 
                    }
                } 
            }
            catch (Exception ex)
            {
                objResponse = new UploadElectronicSignResult { result = false, message = $"Hubo un error al procesar su solicitud: {ex}", statusCode = HttpStatusCode.InternalServerError };
            }
            finally
            {
                Logger.Log($"ELECTRONICSIGN.UPLOAD.{request.token}", $" APIKEY: {Constants.ElectronicSign.ApiKey}", 
                    "Certificado:", JsonConvert.SerializeObject(request), 
                    "Resultado: ", JsonConvert.SerializeObject(objResponse));
            }

            return Request.CreateResponse(objResponse.statusCode, objResponse);
        }

        private static void SendMail(ElectronicSign esign)
        {
            var email = Emailer.Create("esign-upload", "ECUAFACT: Su firma electronica ha sido generada", esign.Email);
            email.Parameters.Add("USUARIO", esign.Identification);
            email.Parameters.Add("NOMBRE", esign.BusinessName);
            email.Send();
        }

        private class UploadElectronicSignResult 
        {
            public HttpStatusCode statusCode { get; set; }
            public string date { get; set; } = DateTime.Now.ToString();
            public string message { get; set; }
            public bool result { get; set; }            
        }
    }


}
