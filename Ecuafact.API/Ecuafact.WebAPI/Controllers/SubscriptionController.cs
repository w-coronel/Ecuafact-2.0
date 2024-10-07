using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.SRI;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using X.PagedList;
using static Ecuafact.WebAPI.Models.Authentication.LoginResponseModel;

namespace Ecuafact.WebAPI.Controllers
{
    public class SubscriptionController : ApiController
    {

        private IssuerDto Issuer => this.GetAuthenticatedIssuer();
        private bool AdminAccess => this.IsAdmin();
        private IPurchaseOrderService __purchaseOrderService;
        private IIssuersService __issuerService;
        private ISubscriptionService _subscriptionService;
        private readonly ICatalogsService _catalogsService;

        public SubscriptionController(ISubscriptionService subscriptionService, IPurchaseOrderService poService, IIssuersService issuerService, ICatalogsService catalogsService)
        {
            __purchaseOrderService = poService;
            __issuerService = issuerService;
            _subscriptionService = subscriptionService;
            _catalogsService = catalogsService;
        }


        /// <summary>
        /// Obtener suscripción del emisor
        /// </summary>
        /// <remarks>
        /// Devuelve la suscripción especificada.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize(AdminAccess = true)]
        [HttpGet, Route("Subscription/{id}")]
        public async Task<OperationResult<Subscription>> GetSubscription(long id)
        {
            var result = await Task.FromResult(_subscriptionService.GetSubscription(id));

            if (result != null)
            {
                return new OperationResult<Subscription>(true, HttpStatusCode.OK, result);

            }
            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, "La suscripción no existe!");
        }


        /// <summary>
        /// Obtener si hay una suscripción vigente
        /// </summary>
        /// <remarks>
        /// Devuelve si la subscripcion incluye firma
        /// </remarks>
        /// <param name="ruc"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize(AdminAccess = true)]
        [HttpGet, Route("Subscription/{ruc}/pendingrequest")]
        public async Task<OperationResult<PurchaseSubscription>> GetPendingRequest(string ruc)
        {
            var result = _subscriptionService.GetPurchaseSubscriptionActive(ruc);
            if (result.IsSuccess)
            {
                return await Task.FromResult(result);
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, "Ya se envio la solicitud de firma electrónica!");
        }


        /// <summary>
        /// Suscripciones pendientes
        /// </summary>
        /// <remarks>
        /// Devuelve la ultima orden de la suscripcion pendiente o en proceso.
        /// </remarks>
        /// <param name="ruc"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpGet, Route("Subscription/pending")]
        public OperationResult<PurchaseSubscription> ExistsPendingOrder(string ruc)
        {
            if (string.IsNullOrEmpty(ruc))
            {
                ruc = Issuer?.RUC;
            }
            var result = _subscriptionService.GetPurchaseBySubscriptionbyRuc(ruc);
            if (result.IsSuccess)
            {
                return result;
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se encuentra la orden de compra pendientes");
        }

        /// <summary>
        /// Suscripciones pendientes
        /// </summary>
        /// <remarks>
        /// Devuelve la ultima orden de la suscripcion pendiente o en proceso.
        /// </remarks>
        /// <param name="ruc"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpGet, Route("Subscription/orderpayment")]
        public OperationResult<PurchaseSubscription> ValidatePaymentOrder(string ruc)
        {
            if (string.IsNullOrEmpty(ruc))
            {
                ruc = Issuer?.RUC;
            }
            var result = _subscriptionService.GetPurchaseBySubscriptionValidatePayment(ruc);
            if (result.IsSuccess)
            {
                return result;
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se encuentra la orden de compra pendientes");
        }

        /// <summary>
        /// Validar plan de suscripción
        /// </summary>
        /// <remarks>
        /// Valida si el plan seleccionado es basico
        /// </remarks>
        /// <param name="LicenceTypeId"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpGet, Route("Subscription/{LicenceTypeId}/LicenceType")]
        public async Task<OperationResult<SubscriptionDto>> ValidatePlanSubscription(int LicenceTypeId = 0)
        {
            if (LicenceTypeId > 0)
            {
                return await ProcessBasicSubscription(LicenceTypeId);
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se encontro registro del plan");
        }


        /// <summary>
        /// Buscar suscripciones
        /// </summary>
        /// <remarks>
        /// Permite la busqueda suscripciones a través de una serie de filtros
        /// </remarks>
        /// <param name="search"></param>
        /// <param name="ruc"></param>      
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize(AdminAccess = true)]
        [HttpGet, Route("Subscription")]
        public async Task<OperationResult<IPagedList<Subscription>>> SearchSubscription(string search = null, string ruc = null, SubscriptionStatusEnum? status = null, int? page = 1, int? pageSize = null)
        {
            var list = _subscriptionService.GetISubscriptions();
            var cantPag = 0;
            // Solo si no es acceso administrativo permite la busqueda de todas las suscripciones.
            if (!AdminAccess)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "No Autorizado");
            }

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(p => p.Issuer.BussinesName.Contains(search)
                                || p.Issuer.Email.Contains(search)
                                || p.Issuer.MainAddress.Contains(search)
                                || p.RUC.Contains(search));
            }

            if (!string.IsNullOrEmpty(ruc))
            {
                list = list.Where(p => p.RUC.Substring(0, 10) == ruc.Substring(0, 10));
            }

            if (status != null)
            {
                list = list.Where(p => p.Status == status);
            }

            page = page ?? 1;
            pageSize = pageSize ?? list.Count();
            cantPag = list.Count();

            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 1;
            }

            var lst = list.OrderBy(p => p.Id).ToPagedList(page.Value, pageSize.Value);
            return await Task.FromResult(new OperationResult<IPagedList<Subscription>>(true, HttpStatusCode.OK, $"Cantidad:{cantPag}")
            {
                Entity = lst,
                DevMessage = cantPag.ToString()
            });
        }


        [HttpPost]
        [EcuafactExpressAuthorize(AdminAccess = true)]
        [Route("Subscription/{purchaseOrderId}/process")]
        public async Task<OperationResult<PurchaseSubscription>> GenerateInvoice(int purchaseOrderId, string bank = null)
        {
            //var purchaseOrder = _subscriptionService.GetSubscriptionByPurchase(purchaseOrderId);
            //if (purchaseOrder.IsSuccess)
            //{
            //    if (purchaseOrder.Entity.IssuerId != Issuer.Id)
            //    {
            //        throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "Usted no esta autorizado para generar este documento");
            //    }
            //    purchaseOrder.Entity.PurchaseOrder.Bank = bank;
            //    return await _subscriptionService.GenerateInvoice(purchaseOrder.Entity);
            //}
            //throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se encuentra la orden de compra");
            return await Request.SubscriptionProcess(purchaseOrderId, Issuer, bank);
        }

        [HttpPost]
        [EcuafactExpressAuthorize(AdminAccess = true)]
        [Route("Subscription/purchaseOrder")]
        public async Task<OperationResult<PurchaseSubscription>> GeneratePurchaseOrder(SubscriptionRequest request)
        {
            var result = await SavePurchaseSubscription(request);
            if (result.IsSuccess)
            {
                return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.Created, result.Entity);
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se pudo procesar la orden de compra");
        }

        /// <summary>
        ///Solicitar plan de facturación electrónica
        /// </summary>
        /// <remarks>       
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize(AdminAccess = true)]
        [HttpPost, Route("Subscription/Licence/Payment")]
        public async Task<OperationResult<Subscription>> PaymentPlanSubscription(RequestPalnModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = ModelState.GetError();
                    return new OperationResult<Subscription>(false, HttpStatusCode.InternalServerError, $"Error al generar la solicitud. {error?.ErrorMessage}");
                }

                return await SavePaymentPlanSubscription(model);

            }
            catch (Exception ex)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, $"{ex.Message}");
            }
        }
        public async Task<OperationResult<Subscription>> SavePaymentPlanSubscription(RequestPalnModel model)
        {
            try
            {
                var refCode = await Task.FromResult(__purchaseOrderService.GetReferenceCodesById(model.ReferenceCode));
                var plan = Request.GetCatalogsService().GetLicenceTypes().Where(p => p.Code == model.ProductCode)?.FirstOrDefault();
                if (refCode.IsSuccess)
                {
                    // Validar si existe el usuario, si no se crea.
                    var ruc = model.Identification.Length == 10 ? $"{model.Identification}001" : model.Identification;
                    var issuer = Request.GetIssuersService().GetIssuer(ruc);
                    if (issuer == null)
                    {
                        var sri = new SRIContrib
                        {
                            RUC = ruc,
                            BusinessName = model.Name,
                            TradeName = model.Name,
                            CreatedOn = DateTime.Today,
                            Status = SRIContribStatus.Pasive,
                            Establishments = new List<Establishment>{
                                new Establishment{
                                    EstablishmentNumber = "1",
                                    CommercialName = "",
                                    Street = model.Address
                                }
                            }
                        };
                        issuer = sri.ToIssuer();
                        issuer.Phone = model.Phone;
                        issuer.Email = model.Email;
                        var result = Request.GetIssuersService().AddIssuer(issuer, issuer.RUC);
                    }
                    if (issuer?.Id > 0)
                    {

                        #region suscripción
                        var _subscriptionLog = new SubscriptionLog();
                        var subscription = Request.GetSubscriptionService().GetSubscription(issuer.RUC);
                        if (subscription == null)
                        {
                            subscription = new Subscription
                            {
                                IssuerId = issuer.Id,
                                LicenceTypeId = plan.Id,
                                RUC = issuer.RUC,
                                Status = SubscriptionStatusEnum.Activa,
                                StatusMsg = "OK",
                                AmountDocument = plan.AmountDocument,
                                SubscriptionStartDate = DateTime.Now,
                                SubscriptionExpirationDate = DateTime.Now.AddYears(1),
                                IssuedDocument = null,
                                BalanceDocument = null
                            };
                        }
                        else
                        {
                            _subscriptionLog = new SubscriptionLog
                            {
                                RUC = subscription.RUC,
                                IssuerId = subscription.IssuerId,
                                SubscriptionId = subscription.Id,
                                LicenceTypeId = subscription.LicenceTypeId,
                                SubscriptionStartDate = subscription.SubscriptionStartDate,
                                SubscriptionExpirationDate = subscription.SubscriptionExpirationDate,
                                IssuedDocument = subscription.IssuedDocument ?? 0,
                                BalanceDocument = subscription.BalanceDocument ?? 0,
                                Observation = subscription.StatusMsg,
                                Status = subscription.Status,
                            };

                            subscription.Status = SubscriptionStatusEnum.Activa;
                            subscription.StatusMsg = "OK";
                            subscription.LicenceTypeId = plan.Id;
                            subscription.LastModifiedOn = DateTime.Now;
                            subscription.SubscriptionStartDate = DateTime.Now;
                            subscription.SubscriptionExpirationDate = DateTime.Now.AddYears(1);
                            subscription.AmountDocument = plan.AmountDocument;
                            subscription.IssuedDocument = null;
                            subscription.BalanceDocument = null;
                        }

                        subscription.LicenceType = plan;

                        #endregion

                        #region Agrega los datos del cliente información adicional de la factura
                        var additionField = new List<AdditionalFieldModel>();
                        var agreementInvoice = refCode.Entity.AgreementInvoice ?? false;
                        if (agreementInvoice)
                        {
                            additionField.Add(new AdditionalFieldModel { Name = "Cliente", Value = issuer.BussinesName, LineNumber = 1 });
                            additionField.Add(new AdditionalFieldModel { Name = "RUC", Value = issuer.RUC, LineNumber = 2 });
                        }
                        else
                        {
                            additionField.Add(new AdditionalFieldModel { Name = "Codigo", Value = refCode.Entity.Code, LineNumber = 1 });
                            additionField.Add(new AdditionalFieldModel { Name = "Convenio", Value = refCode.Entity.Description, LineNumber = 2 });
                        }
                        
                        #endregion

                        #region generamos la orden de compra 

                        var discountCommission = refCode?.Entity?.DiscountCommission ?? false;
                        decimal planPrice = plan.Price;
                        decimal ivaProduct = 0;
                        decimal total = 0;
                        if (discountCommission)
                        {
                            if (plan.Code == Constants.PlanPro)
                            {
                                decimal planL03Discount = refCode?.Entity?.PlanL03 ?? 0;
                                planPrice -= planL03Discount;
                            }
                            else if (plan.Code != Constants.PlanBasic)
                            {
                                decimal planL03Discount = refCode?.Entity?.PlanL02 ?? 0;
                                planPrice -= planL03Discount;
                            }
                        }
                        ivaProduct = decimal.Round(planPrice * (plan.TaxBase / 100), 2);
                        total = planPrice + ivaProduct;

                        var order = new PurchaseOrder
                        {
                            BusinessName = agreementInvoice ? refCode?.Entity?.BusinessName : issuer.BussinesName,
                            Identification = agreementInvoice ?  refCode?.Entity?.RUC : issuer.RUC,
                            Address = agreementInvoice ?  refCode?.Entity?.Address : issuer.MainAddress,
                            Email = agreementInvoice ? refCode?.Entity?.Email:issuer.Email,
                            Phone = agreementInvoice ?  refCode?.Entity?.Phone : issuer.Phone,
                            IssuedOn = DateTime.Today,
                            Products = $"Plan {plan.Name}({(plan.Code == "L03" ? "Emisión ilimitada" : plan.AmountDocument)} documentos), {plan.Description?.Split('|')[2]}",
                            Subtotal0 = 0,
                            Subtotal12 = planPrice,
                            IVA = ivaProduct,
                            ICE = 0,
                            Interests = 0,
                            Additional = 0,
                            Total = total,
                            ZIP = "000",
                            Status = PurchaseOrderStatusEnum.Payed,
                            IssuerId = issuer.Id,
                            FirstName = ToFirstName(Issuer.BussinesName),
                            LastName = ToLastName(Issuer.BussinesName),
                            City = issuer.City ?? "GUAYAQUIL",
                            Province = issuer.Province ?? "GUAYAS",
                            Country = "EC",
                            TotalDiscount = 0,
                            Discount = 0,
                            BeniReferCodeId = null,
                            CreatedOn = DateTime.Now,
                            DiscountCode = null
                        };

                        //Se genera la orden de compra para la suscripción :
                        var electronicSignSendProcess = refCode.Entity.ElectronicSignSendProcess ?? true;
                        var _purchaseSubscription = new PurchaseSubscription()
                        {
                            Status = PurchaseOrderSubscriptionStatusEnum.Payed,
                            IssuerId = issuer.Id,
                            CreatedOn = DateTime.Now,
                            LicenceTypeId = plan.Id,
                            RequestElectronicSign = plan.IncludeCertificate ? RequestElectronicSignEnum.Pending : RequestElectronicSignEnum.NotInclude, //electronicSignSendProcess ? RequestElectronicSignEnum.Pending:RequestElectronicSignEnum.Sent,
                            RequestElectronicSignMsg = plan.IncludeCertificate ? RequestElectronicSignEnum.Pending.GetDisplayValue() : RequestElectronicSignEnum.NotInclude.GetDisplayValue(), //electronicSignSendProcess ?  RequestElectronicSignEnum.Pending.GetDisplayValue() : RequestElectronicSignEnum.Sent.GetDisplayValue(),
                            PurchaseOrder = order,
                            Subscription = subscription,
                            SubscriptionLog = _subscriptionLog
                        };

                        var benyRefeCode = new BeneficiaryReferenceCode()
                        {
                            DiscountCode = refCode.Entity.Code,
                            Identification = issuer.RUC,
                            ReferenceCodId = refCode.Entity.Id,
                            IssuerId = issuer.Id,
                            Status = ReferenceCodeStatusEnum.Applied,
                            StatusMsg = refCode.Entity.DiscountRate > 0 ? $"Aplicado:Código de descuento por {refCode.Entity.Description}" : $"Compra del plan {plan.Name} por {refCode.Entity.Description}",
                            LastModifiedOn = DateTime.Now,
                            BeneficiaryId = 0
                        };

                        var resulturchaseSubscription = await Task.FromResult(Request.GetSubscriptionService().BuySubscription(_purchaseSubscription));
                        if (resulturchaseSubscription.IsSuccess)
                        {
                            await Task.FromResult(__purchaseOrderService.AddBeneficiaryReferenceCode(benyRefeCode));
                            await _subscriptionService.GenerateInvoice(resulturchaseSubscription.Entity, "M", additionField);

                            #region registrar Usuario 
                            var registerModel = new RegisterClientModel
                            {
                                Username = issuer.RUC,
                                BusinessName = issuer.BussinesName,
                                Name = issuer.BussinesName,
                                Email = issuer.Email,
                                Phone = issuer.Phone,
                                Password = Constants.AppToken,
                                StreetAddress = issuer.EstablishmentAddress,
                                AppName = "WEB-APP"
                            };
                            await Request.RegisterUsers(registerModel);                            
                            #endregion

                            return new OperationResult<Subscription>(true, HttpStatusCode.OK, subscription);
                        }
                        #endregion
                    }
                }

                return new OperationResult<Subscription>(false, HttpStatusCode.InternalServerError, "No se encontro convenio");
            }
            catch (Exception ex)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, ex.ToString(), $"Se produjo un error al procesar la solciitud: {ex.ToString()}");
            }
        }
        private async Task<OperationResult<PurchaseSubscription>> SavePurchaseSubscription(SubscriptionRequest request)
        {
            try
            {
                #region Variables

                decimal discount = 0;
                decimal totalDiscount = 0;
                long benReferenCodId = 0;
                bool omitirPago = false;
                bool generarFactura = false;
                bool commission = false;
                string refCode = "";
                decimal discountValue = 0;
                BeneficiaryReferenceCode descuentoCod = null;
                ReferenceCodes codPromocional = null;

                #endregion

                #region Validar modelo
                if (request == null)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, $"La solicitud especificada no es válida: { JsonConvert.SerializeObject(request)}");
                }
                #endregion

                #region Validar si hay una orden pendiente
                var _purchaseOrderSubscription = _subscriptionService.GetPurchaseBySubscriptionValidatePayment(request.RUC);
                if (_purchaseOrderSubscription.IsSuccess)
                {
                    return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.Created, _purchaseOrderSubscription.Entity); 
                }
                #endregion     

                #region Codigo de prpmoción o descuento
                //Verifica si el usuario en vía código promocional
                var plan = _catalogsService.GetLicenceTypes().Where(p => p.Id == request.LicenceTypeId)?.FirstOrDefault();
                if (!string.IsNullOrEmpty(request?.InvoiceInfo?.DiscountCode))
                {
                    var referenceCode = await Task.FromResult(__purchaseOrderService.GetReferenceCodes(request.InvoiceInfo.DiscountCode, request.RUC));
                    if (referenceCode.IsSuccess)
                    {
                        codPromocional = referenceCode.Entity;
                        var agreementInvoice = codPromocional.AgreementInvoice ?? false;
                        if (agreementInvoice && !string.IsNullOrEmpty(codPromocional.RUC))
                        {
                            request.InvoiceInfo.Name = codPromocional.BusinessName;
                            request.InvoiceInfo.Identification = codPromocional.RUC;
                            request.InvoiceInfo.Address = codPromocional.Address;
                            request.InvoiceInfo.Email = codPromocional.Email;
                            request.InvoiceInfo.Phone = codPromocional.Phone;
                        }
                        commission = codPromocional.DiscountCommission ?? false;                       
                        
                        omitirPago = codPromocional.SkipPaymentOrder;
                        generarFactura = omitirPago;
                        refCode = codPromocional.Code;
                        if(referenceCode.Entity.SpecialPromotion)
                        {
                            var _planCode = _subscriptionService.GetSubscription(request.RUC).LicenceType.Code;
                            if (referenceCode.Entity.ApplyDiscount.Equals(plan.Code) && referenceCode.Entity.PromotionByPlan.Equals(_planCode))
                            {
                                discount = codPromocional.DiscountRate;
                                discountValue = codPromocional.DiscountValue;
                            }
                            else if(referenceCode.Entity.ApplyDiscount.Equals(plan.Code) && referenceCode.Entity.PromotionByPlan.Equals("T01"))
                            {
                                discount = codPromocional.DiscountRate;
                                discountValue = codPromocional.DiscountValue;
                            }
                        }
                        else
                        {
                            discount = codPromocional.DiscountRate;
                            discountValue = codPromocional.DiscountValue;
                        }
                    }
                }
                else
                {
                    //Verifica si el Ruc tiene codigo de promoción o descuento 
                    var resultDescuentoCod = await Task.FromResult(__purchaseOrderService.GetBeneficiaryReferenceCode(request.RUC));
                    if (resultDescuentoCod.IsSuccess)
                    {
                        descuentoCod = resultDescuentoCod.Entity;
                        var agreementInvoice = descuentoCod.ReferenceCodes.AgreementInvoice ?? false;
                        if (agreementInvoice && !string.IsNullOrEmpty(descuentoCod.ReferenceCodes.RUC))
                        {
                            request.InvoiceInfo.Name = descuentoCod.ReferenceCodes.BusinessName;
                            request.InvoiceInfo.Identification = descuentoCod.ReferenceCodes.RUC;
                            request.InvoiceInfo.Address = descuentoCod.ReferenceCodes.Address;
                            request.InvoiceInfo.Email = descuentoCod.ReferenceCodes.Email;
                            request.InvoiceInfo.Phone = descuentoCod.ReferenceCodes.Phone;
                        }
                        discount = descuentoCod.ReferenceCodes.DiscountRate;
                        omitirPago = descuentoCod.ReferenceCodes.SkipPaymentOrder;
                        generarFactura = omitirPago;
                        refCode = descuentoCod.ReferenceCodes.Code;
                        benReferenCodId = descuentoCod.Id;
                    }
                }

                //se realiza los respectivos calculos del valor de la solicitud de firma electronica                
                decimal price = plan.Price;
                if (discount > 0)
                {
                    totalDiscount = decimal.Round(price * (discount / 100), 3, MidpointRounding.AwayFromZero);
                    price -= decimal.Round(price * (discount / 100), 3, MidpointRounding.AwayFromZero);
                }
                else if(discountValue > 0)
                {
                    discount = decimal.Round((discountValue * 100) / price, 3, MidpointRounding.AwayFromZero); 
                    totalDiscount = discountValue;
                    price -= discountValue;
                }
                else if (commission)
                {
                    if (plan.Code == Constants.PlanPro)
                    {
                        var commissionPlanPro = codPromocional.PlanL03 ?? 0;
                        price -= commissionPlanPro;
                    }
                    else if (plan.Code != Constants.PlanBasic)
                    {
                        var commissionPlanPime = codPromocional.PlanL02 ?? 0;
                        price -= commissionPlanPime;
                    }
                }

                var ivaProduct = decimal.Round(price * 0.15M, 3, MidpointRounding.AwayFromZero);
                var total = price + ivaProduct;

                #endregion

                #region Verificar suscripción
                var _subscriptionLog = new SubscriptionLog();
                request.Issuer = Issuer;
                var subscription = _subscriptionService.GetSubscription(request.RUC);
                if (subscription == null)
                {
                    subscription = new Subscription
                    {
                        IssuerId = Issuer.Id,
                        LicenceTypeId = request.LicenceTypeId,
                        RUC = Issuer.RUC,
                        Status = SubscriptionStatusEnum.Saved,
                        StatusMsg = "Registrada",
                        AmountDocument = plan.AmountDocument,
                        IssuedDocument = null,
                        BalanceDocument = null
                    };
                }               
                #endregion

                #region generamos la orden de compra
                var order = new PurchaseOrder
                {
                    BusinessName = request.InvoiceInfo.Name,
                    Identification = request.InvoiceInfo.Identification,
                    Address = request.InvoiceInfo?.Address,
                    Email = request.InvoiceInfo?.Email,
                    Phone = !string.IsNullOrWhiteSpace(request.InvoiceInfo.Phone) ? request.InvoiceInfo.Phone: Issuer.Phone,
                    IssuedOn = DateTime.Today,
                    Products = $"{request.InvoiceInfo.Product}",
                    Subtotal0 = 0,
                    Subtotal12 = price,
                    IVA = ivaProduct,
                    ICE = 0,
                    Interests = 0,
                    Additional = 0,
                    Total = total,
                    ZIP = "000",
                    Status = PurchaseOrderStatusEnum.Saved,
                    IssuerId = Issuer.Id,
                    FirstName = ToFirstName(Issuer.BussinesName),
                    LastName = ToLastName(Issuer.BussinesName),
                    City = Issuer.City ?? "GUAYAQUIL",
                    Province = Issuer.Province ?? "GUAYAS",
                    Country = "EC",
                    TotalDiscount = totalDiscount,
                    Discount = discount,
                    BeniReferCodeId = benReferenCodId,
                    CreatedOn = DateTime.Now,
                    DiscountCode = refCode
                };
                #endregion

                #region Agrega los datos del cliente información adicional de la factura
                var additionField = new List<AdditionalFieldModel>();
                if (request?.InvoiceInfo?.Identification != Issuer.RUC)
                {
                    additionField.Add(new AdditionalFieldModel { Name = "Cliente", Value = Issuer.BussinesName, LineNumber = 1 });
                    additionField.Add(new AdditionalFieldModel { Name = "RUC", Value = Issuer.RUC, LineNumber = 2 });
                }
                #endregion

                #region orden de compra y subcripcion 
                //Se guarda la orden de compra para la suscripción

                var model = new PurchaseSubscription()
                {
                    Status = PurchaseOrderSubscriptionStatusEnum.Saved,
                    IssuerId = request.Issuer.Id,
                    CreatedOn = DateTime.Now,
                    LicenceTypeId = plan.Id,
                    PurchaseOrder = order,     
                    Subscription = subscription
                };

                
                var result = await Task.FromResult(_subscriptionService.BuySubscription(model));
                if (result.IsSuccess)
                {
                    //Regsitra el usuario benificiiario del codigo promocional
                    if (codPromocional != null)
                    {
                        var _model = new BeneficiaryReferenceCode()
                        {
                            DiscountCode = codPromocional.Code,
                            Identification = request.RUC,
                            ReferenceCodId = codPromocional.Id,
                            IssuerId = Issuer.Id,
                            Status = ReferenceCodeStatusEnum.Applied,
                            StatusMsg = $"Aplicado:Código de descuento por {codPromocional.Description}",
                            CreatedOn = DateTime.Now
                        };
                        await Task.FromResult(__purchaseOrderService.AddBeneficiaryReferenceCode(_model));
                    }
                    else if (descuentoCod != null)
                    {
                        descuentoCod.IssuerId = request.Issuer.Id;
                        await Task.FromResult(__purchaseOrderService.UpdateBeneficiaryReferenceCode(descuentoCod));
                    }

                    // omitir la pasarela de pago y generación de la factura
                    if (omitirPago)
                    {
                        if (descuentoCod != null)
                        {
                            result.Entity.PurchaseOrder.ReferenceCodes = descuentoCod.ReferenceCodes;
                        }
                        else if (codPromocional != null)
                        {
                            result.Entity.PurchaseOrder.ReferenceCodes = codPromocional;
                        }

                        //Se genera la factura
                        await _subscriptionService.GenerateInvoice(result.Entity, "", additionField);

                    }

                    return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.Created, result.Entity);
                }
                #endregion

                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, $"Error al crear la orden de compra de la suscripción del RUC: {request.InvoiceInfo.Identification}");
            }
            catch (Exception ex)
            {

                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, ex.ToString(), $"Se produjo un error al guardar la orden: {ex.ToString()}");
            }

        }
        private async Task<OperationResult<SubscriptionDto>> ProcessBasicSubscription(int id)
        {
            try
            {
                var plans = _catalogsService.GetLicenceTypes().Where(s => s.Id == id).FirstOrDefault();
                if (plans?.Code == Constants.PlanBasic)
                {
                    var _subscription = _subscriptionService.GetSubscription(Issuer.RUC);
                    if (_subscription != null)
                    {                        
                        _subscription.Status = SubscriptionStatusEnum.Activa;
                        _subscription.LicenceTypeId = plans.Id;
                        _subscription.StatusMsg = "OK";
                        _subscription.LastModifiedOn = DateTime.Now;
                        _subscription.SubscriptionStartDate = DateTime.Now;
                        _subscription.SubscriptionExpirationDate = DateTime.Now.AddYears(1);
                        _subscription.AmountDocument = plans.AmountDocument;
                        _subscription.IssuedDocument = null;
                        _subscription.BalanceDocument = null;

                        var resultput = _subscriptionService.UpdateSubscription(_subscription);
                        if (resultput.IsSuccess)
                        {
                            return await Task.FromResult(new OperationResult<SubscriptionDto>(true, HttpStatusCode.Created, resultput.Entity.ToSubscriptionDto()));
                        }
                    }
                    else
                    {
                        var model = new Subscription
                        {
                            Status = SubscriptionStatusEnum.Activa,
                            LicenceTypeId = plans.Id,
                            RUC = Issuer.RUC,
                            IssuerId = Issuer.Id,
                            SubscriptionStartDate = DateTime.Now,
                            SubscriptionExpirationDate = DateTime.Now.AddYears(1),
                            StatusMsg = "OK",
                            AmountDocument = plans.AmountDocument,
                        };
                        var result = _subscriptionService.AddSubscription(model);
                        if (result.IsSuccess)
                        {
                            return await Task.FromResult(new OperationResult<SubscriptionDto>(true, HttpStatusCode.Created, result.Entity.ToSubscriptionDto()));
                        }
                    }
                }


                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, $"Tipo de plans:{plans.Code}-{plans.Name}");
            }
            catch (Exception ex)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, ex.ToString(), $"Se produjo un error al listar el plan: {ex.ToString()}");
            }

        }
        private static string ToFirstName(string bussinesName)
        {
            var names = bussinesName.Split(' ');
            if (names.Length > 0)
            {
                if (names.Length > 1)
                {
                    return $"{names[0]} {names[1]}";
                }
                else if (names.Length == 1)
                {
                    return names[0];
                }
            }

            return bussinesName;
        }
        private static string ToLastName(string bussinesName)
        {
            var lastName = bussinesName.Split(' ');
            if (lastName.Length > 0)
            {
                if (lastName.Length > 3)
                {
                    return $"{lastName[2]} {lastName[3]}";
                }
                else if (lastName.Length == 3)
                {
                    return lastName[2];
                }
            }

            return bussinesName;
        }        
    }
}
