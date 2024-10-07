using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Dal.Repository.Extensions;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IEntityRepository<Subscription> _subscriptionService;
        private readonly IEntityRepository<SubscriptionLog> _subscriptionLogService;
        private readonly IEntityRepository<PurchaseSubscription> _purchaseSubscriptionService;
        private IEntityRepository<PurchaseOrder> _purchaseOrderRepository;
        private IEntityRepository<UserPayment> _userPaymentRepository;
        private ICatalogsService _catalogsService;

        public SubscriptionService(
            IEntityRepository<Subscription> subscriptionRepository,
            IEntityRepository<UserPayment> userPaymentRepository,
            IEntityRepository<PurchaseSubscription> purchaseSubscriptionService, 
            IEntityRepository<PurchaseOrder> poRepository,
            IEntityRepository<SubscriptionLog> subscriptionLogService,
             ICatalogsService catalogsService
            )
        {
            _subscriptionService = subscriptionRepository;
            _purchaseSubscriptionService = purchaseSubscriptionService;
            _purchaseOrderRepository = poRepository;
            _userPaymentRepository = userPaymentRepository;
            _subscriptionLogService = subscriptionLogService;
            _catalogsService = catalogsService;
        }

        public Subscription GetSubscription(long id)
        {
            return _subscriptionService.FindBy(m => m.Id == id).Include(s => s.Issuer).FirstOrDefault();
        }
        public Subscription GetSubscription(string issuerRuc)
        {
            return _subscriptionService.FindBy(m => m.RUC.Equals((string)issuerRuc))
                                        .Include(s => s.Issuer)                                        
                                        .Include(l => l.LicenceType)
                                        .FirstOrDefault();
        }
        public IQueryable<Subscription> GetISubscriptions(object issuerRuc)
        {
            if (issuerRuc is long)
            {
                return _subscriptionService.FindBy(pr => pr.IssuerId == (long)issuerRuc).Include(s => s.Issuer);
            }
            return _subscriptionService.FindBy(m => m.RUC.Equals((string)issuerRuc)).Include(s => s.Issuer);
        }
        public IQueryable<Subscription> GetISubscriptions()
        {
            return _subscriptionService.All.Include(s => s.Issuer);
        }
        public bool Exists(object id)
        {
            if (id is long)
            {
                return _subscriptionService.Exists(model => model.Id == (long)id);
            }

            return _subscriptionService.Exists(model => model.RUC == (string)id);
        }
        public OperationResult<Subscription> AddSubscription(Subscription model)
        {
            try
            {
                var subscription = _subscriptionService.FindBy(pr => pr.RUC.Equals(model.RUC)).FirstOrDefault();

                if (subscription != null)
                {
                    return new OperationResult<Subscription>(true, HttpStatusCode.OK)
                    {
                        Entity = subscription
                    };
                }
                //newIssuerLicence.CreatedOn = DateTime.Now;
                _subscriptionService.Add(model);
                _subscriptionService.Save();

                //Se registra en el log de suscripcion
                AddSubscriptionLog(model);

                return new OperationResult<Subscription>(true, HttpStatusCode.OK)
                {
                    Entity = model
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<Subscription>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la suscripción del emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Subscription>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la suscripción del emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<Subscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar la suscripción del emisor" };
            }
        }
        public OperationResult<Subscription> UpdateSubscription(Subscription model)
        {
            try
            {
                _subscriptionService.Save();
                return new OperationResult<Subscription>(true, HttpStatusCode.OK)
                {
                    Entity = model
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<Subscription>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la suscripción del emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Subscription>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la suscripción del emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<Subscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.Message, UserMessage = "Ocurrio un error guardar la suscripción del emisor" };
            }
        }
        public OperationResult<Subscription> ActiveSubscription(string ruc, long issuerId)
        {
            try
            {
                var subscription = _subscriptionService.FindBy(pr => pr.RUC.Equals(ruc)).FirstOrDefault();

                if (subscription != null)
                {
                    subscription.SubscriptionStartDate = DateTime.Now;
                    subscription.SubscriptionExpirationDate = DateTime.Now.AddYears(1);
                    subscription.Status = SubscriptionStatusEnum.Activa;
                    subscription.StatusMsg = "OK";
                    subscription.LastModifiedOn = DateTime.Now;
                }
                else
                {
                    var model = new Subscription()
                    {
                        SubscriptionStartDate = DateTime.Now,
                        SubscriptionExpirationDate = DateTime.Now.AddYears(1),
                        Status = SubscriptionStatusEnum.Activa,
                        StatusMsg = "OK",
                        CreatedOn = DateTime.Now,
                        RUC = ruc,
                        IssuerId = issuerId,
                        LicenceTypeId = (long)LicenceTypeEnum.Basic ///ojo modificar
                    };
                    _subscriptionService.Add(model);
                }

                _subscriptionService.Save();
                return new OperationResult<Subscription>(true, HttpStatusCode.OK)
                {
                    Entity = subscription
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<Subscription>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la suscripción del emisor" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<Subscription>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la suscripción del emisor" };
            }
            catch (Exception e)
            {
                return new OperationResult<Subscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar la suscripción del emisor" };
            }
        }

        public OperationResult<PurchaseSubscription> BuySubscription(PurchaseSubscription model)
        {
            try
            {
                // Primero verificamos si existe otra solicitud pendiente de procesamiento o de pago:
                var existeSolicitudPendiente = _purchaseSubscriptionService.FindBy(m => m.SubscriptionId == model.SubscriptionId && m.Status != PurchaseOrderSubscriptionStatusEnum.Payed).Any();
                if (existeSolicitudPendiente)
                {
                    return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.NotFound)
                    {
                        DevMessage = $"Ya existe una solicitud para el RUC especificado: {model.Subscription.RUC}",
                        UserMessage = "Ya existe una solicitud para este usuario"
                    };
                }

                // Primero agregamos la suscripcion:
                if (model.Subscription !=null && model.Subscription.Id == 0)
                {
                    _subscriptionService.Add(model.Subscription);
                }                

                // Luego agregamos la orden de compra                
                _purchaseOrderRepository.Add(model.PurchaseOrder);

                // Luego guardamos orden suscripcion
                _purchaseSubscriptionService.Add(model);

                // Un solo metodo guarda todas las tablas relacionadas:
                _purchaseSubscriptionService.Save();

                return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK)
                {
                    Entity = model
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                string msg = ex.Message + "\n";

                if (ex.EntityValidationErrors.Count() > 0)
                {
                    foreach (var item in ex.EntityValidationErrors)
                    {
                        var er = item.ValidationErrors?.FirstOrDefault();
                        msg += $"{er.PropertyName}: {er.ErrorMessage}\n";
                    }
                }
                else
                {
                    msg += "  " + ex.ToString();
                }

                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la Orden de Compra" };
            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.Message} {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la Orden de Compra" };
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar la Orden de Compra" };
            }
        }
        public OperationResult<PurchaseSubscription> AddPurchaseSubscription(PurchaseSubscription model)
        {

            try
            {
                //newIssuerLicence.CreatedOn = DateTime.Now;
                _purchaseSubscriptionService.Add(model);
                _purchaseSubscriptionService.Save();
                return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK)
                {
                    Entity = model
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la orden de la suscripción" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la orden de la suscripción" };
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar la orden de la suscripción" };
            }
        }
        public OperationResult<PurchaseSubscription> UpdatePurchaseSubscription(PurchaseSubscription model)
        {
            try
            {
                if(model.SubscriptionLog != null)
                {
                    _subscriptionLogService.Add(model.SubscriptionLog);
                }
                _purchaseSubscriptionService.Save();
                if (model?.UserPaymentId > 0)
                {
                    var userPayment = _userPaymentRepository.FindBy(up => up.Id == model.UserPaymentId && up.Status == UserPaymentStatusEnum.Validate).FirstOrDefault();
                    if (userPayment != null)
                    {
                        userPayment.Status = UserPaymentStatusEnum.Applied;
                        userPayment.StatusMsg = userPayment.StatusMsg.Replace("Pago del", "Activado");
                        userPayment.LastModifiedOn = DateTime.Now;
                        _userPaymentRepository.Save();
                    }
                }
                return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK)
                {
                    Entity = model
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la orden de la suscripción" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la orden de la suscripción" };
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.Message, UserMessage = "Ocurrio un error guardar la orden de la suscripción" };
            }
        }
        public OperationResult<PurchaseSubscription> PurchaseSubscriptionInvoiceProcessed(long purchaseSubscriptionId)
        {
            try
            {
                var _purchaseSubscription =  _purchaseSubscriptionService.FindBy(id => id.Id == purchaseSubscriptionId).FirstOrDefault();
                _purchaseSubscription.InvoicePrrocessed = true;
                _purchaseSubscriptionService.Edit(_purchaseSubscription);
                _purchaseSubscriptionService.Save();               
                return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK)
                {
                    Entity = _purchaseSubscription
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la orden de la suscripción" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la orden de la suscripción" };
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.Message, UserMessage = "Ocurrio un error guardar la orden de la suscripción" };
            }
        }
        public OperationResult<PurchaseSubscription> GetIPurchaseSubscription(long id)
        {
            try
            {
                var purchaseSubscription = _purchaseSubscriptionService.All.Where(x => x.Id == id)
                    .Include(s => s.Subscription).Include(p => p.PurchaseOrder).OrderByDescending(o => o.Id).FirstOrDefault();

                if (purchaseSubscription != null)
                {
                    return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK) { Entity = purchaseSubscription };
                }

                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.NotFound, "No se encuentra la solicitud de suscripción!");
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error en la busqueda de la orden de suscripción" };
            }

        }
        public OperationResult<PurchaseSubscription> GetPurchaseBySubscriptionbyRuc(string ruc)
        {
            try
            {
                // validar que la subscripcion este activa
                var subscription = _subscriptionService.FindBy(s => s.RUC == ruc).Include(p=> p.LicenceType).FirstOrDefault();
                if (subscription != null)
                {                   
                    if (subscription.Status == SubscriptionStatusEnum.Activa && subscription.LicenceType.Code != Constants.PlanBasic)
                    {
                        var balanceDocument = subscription.BalanceDocument != null ? subscription.BalanceDocument : 0;
                        var dias = Convert.ToInt32((subscription.SubscriptionExpirationDate.Value - DateTime.Now).TotalDays);
                        if (dias > 30 && balanceDocument >= 10)
                        {                           
                            var purchaseSubscriptionPayed = _purchaseSubscriptionService.All.Where(x => x.Subscription.RUC == ruc && x.Status == PurchaseOrderSubscriptionStatusEnum.Payed)
                              .Include(s => s.Subscription).Include(s => s.Subscription.Issuer).Include(l => l.Subscription.LicenceType).Include(p => p.PurchaseOrder)
                              .OrderByDescending(o => o.Id).FirstOrDefault();
                            if (purchaseSubscriptionPayed != null)
                            {                               
                                return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK) { Entity = purchaseSubscriptionPayed };
                            }
                        }
                       
                    }
                }

                // validar que haya una orden sin pagar
                var purchaseSubscription = _purchaseSubscriptionService.All.Where(x => x.Subscription.RUC == ruc && x.Status != PurchaseOrderSubscriptionStatusEnum.Payed)
                    .Include(s => s.Subscription).Include(s => s.Subscription.Issuer).Include(l => l.Subscription.LicenceType).Include(p => p.PurchaseOrder)
                    .OrderByDescending(o => o.Id).FirstOrDefault();

                if (purchaseSubscription != null)
                {
                    return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK) { Entity = purchaseSubscription };
                }

                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.NotFound, "No se encuentra la solicitud de suscripción!");
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error en la busqueda de la orden de suscripción" };
            }

        }
        public OperationResult<PurchaseSubscription> GetSubscriptionByPurchase(long purchaseOrderId)
        {
            try
            {
                var PurchaseSubscription = _purchaseSubscriptionService.FindBy(x => x.PurchaseOrderId == purchaseOrderId).Include(s => s.Subscription.LicenceType).Include(p => p.PurchaseOrder).FirstOrDefault();

                if (PurchaseSubscription == null)
                {
                    return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.NotFound, "No se encontro la orden de suscripción");
                }

                return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK) { Entity = PurchaseSubscription };
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No se encuentra la orden de suscripción!" };
            }

        }
        public OperationResult<PurchaseOrder> GetPurchaseOrderById(long id)
        {
            try
            {
                var purchaseOrder = _purchaseOrderRepository.All.FirstOrDefault(x => x.PurchaseOrderId == id);

                if (purchaseOrder == null)
                {
                    var msg = "No existe la orden de compra especificada.";
                    return new OperationResult<PurchaseOrder>(false, HttpStatusCode.NotFound) { DevMessage = msg, UserMessage = msg };
                }

                return new OperationResult<PurchaseOrder>(true, HttpStatusCode.OK) { Entity = purchaseOrder };
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseOrder>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No se encuentra la orden de compra!" };
            }
        }
        public OperationResult<PurchaseSubscription> GetPurchaseSubscriptionActive(string ruc)
        {
            try
            {
                var startDate = DateTime.Now.AddYears(-1);
                var purchaseSubscription = _purchaseSubscriptionService.FindBy(x => x.Subscription.RUC == ruc && x.Status == PurchaseOrderSubscriptionStatusEnum.Payed
                && x.InvoiceId > 0 && x.CreatedOn >= startDate && x.CreatedOn <= DateTime.Now && (x.RequestElectronicSign == 0 || x.RequestElectronicSign == null))
                .Include(s => s.Subscription).Include(p => p.PurchaseOrder).OrderByDescending(sp => sp.Id).FirstOrDefault();

                if (purchaseSubscription != null)
                {
                    return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK) { Entity = purchaseSubscription };
                }

                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.NotFound, "No se encuentra la solicitud de suscripción!");
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error en la busqueda de la orden de suscripción" };
            }

        }
        public OperationResult<PurchaseSubscription> GetPurchaseBySubscriptionValidatePayment(string ruc)
        {
            try
            {
                // validar que la subscripcion este activa
                var subscription = _subscriptionService.FindBy(s => s.RUC == ruc).Include(s=> s.LicenceType).FirstOrDefault();
                if (subscription != null)
                {                    
                    if (subscription.Status == SubscriptionStatusEnum.Activa && subscription.LicenceType.Code != Constants.PlanBasic)
                    {
                        var dias = Convert.ToInt32((subscription.SubscriptionExpirationDate.Value - DateTime.Now).TotalDays);
                        if (dias > 30 )
                        {
                            var purchaseSubscriptionPayed = _purchaseSubscriptionService.All.Where(x => x.Subscription.RUC == ruc && x.Status == PurchaseOrderSubscriptionStatusEnum.Payed)
                                   .Include(s => s.Subscription).Include(s => s.Subscription.Issuer).Include(l => l.Subscription.LicenceType).Include(p => p.PurchaseOrder)
                                   .OrderByDescending(o => o.Id).FirstOrDefault();

                            if (purchaseSubscriptionPayed != null)
                            {
                                return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK) { Entity = purchaseSubscriptionPayed };
                            }
                        }
                        
                    }
                }
                // validar que haya una orden sin pagar
                var _purchaseSubscription = _purchaseSubscriptionService.All.Where(x => x.Subscription.RUC == ruc && x.Status != PurchaseOrderSubscriptionStatusEnum.Payed)
                           .Include(s => s.Subscription).Include(s => s.Subscription.Issuer).Include(l => l.Subscription.LicenceType).Include(p => p.PurchaseOrder)
                           .OrderByDescending(o => o.Id).FirstOrDefault();

                if (_purchaseSubscription != null)
                {
                    return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK) { Entity = _purchaseSubscription };
                }

                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.NotFound, "No se encuentra la solicitud de suscripción!");
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error en la busqueda de la orden de suscripción" };
            }

        }
        public OperationResult<PurchaseSubscription> GetIPurchaseSubscriptionBySubscriptionId(long id)
        {
            try
            {
                var date = DateTime.Now.AddYears(-1);
                var purchaseSubscription = _purchaseSubscriptionService.All.Where(x => x.SubscriptionId == id && x.CreatedOn > date)
                                                                           .Include(s => s.Subscription)                                                                          
                                                                           .OrderByDescending(o => o.CreatedOn)
                                                                           .FirstOrDefault();

                if (purchaseSubscription != null)
                {
                    return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK) { Entity = purchaseSubscription };
                }

                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.NotFound, "No se encuentra la solicitud de suscripción!");
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error en la busqueda de la orden de suscripción" };
            }

        }
        public OperationResult<SubscriptionLog> AddSubscriptionLog(Subscription model)
        {
            try
            {
                var _model = new SubscriptionLog
                {
                    RUC = model.RUC,
                    IssuerId = model.IssuerId,
                    SubscriptionId = model.Id,
                    LicenceTypeId = model.LicenceTypeId,
                    SubscriptionStartDate = model.SubscriptionStartDate,
                    SubscriptionExpirationDate = model.SubscriptionExpirationDate,
                    IssuedDocument = model.IssuedDocument ?? 0,
                    BalanceDocument = model.BalanceDocument ?? 0
                };


                _subscriptionLogService.Add(_model);
                _subscriptionLogService.Save();
                return new OperationResult<SubscriptionLog>(true, HttpStatusCode.OK)
                {
                    Entity = _model
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<SubscriptionLog>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el log de suscripción" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<SubscriptionLog>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el log de suscripción" };
            }
            catch (Exception e)
            {
                return new OperationResult<SubscriptionLog>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el log de suscripción" };
            }
        }
        public SubscriptionLog GetSubscriptionLog(long id)
        {
            return _subscriptionLogService.FindBy(m => m.SubscriptionId == id)
                                          .OrderByDescending(s=> s.Id)
                                          .FirstOrDefault();
        }

        public LicenceType GetLicenceTypeById(long id)
        {
            return _catalogsService.GetLicenceTypes().Where(m => m.Id == id).FirstOrDefault();
        }

    }
}

