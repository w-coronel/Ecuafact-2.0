using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private IEntityRepository<PurchaseOrder> _purchaseOrderRepository;
        private IEntityRepository<PurchasePayment> _purchasePaymentRepository;
        private IEntityRepository<ElectronicSign> _eSignRepository;
        private IEntityRepository<PurchaseLog> _purchaseLogRepository;
        private IEntityRepository<BeneficiaryReferenceCode> _benefRefereCodeRepository;
        private IEntityRepository<ReferenceCodes> _referenceCodeRepository;
        private IEntityRepository<ECommerce> _eCommerceRepository;
        private IEntityRepository<Subscription> _subscriptionRepository;

        public PurchaseOrderService(IEntityRepository<PurchaseOrder> poRepository, 
            IEntityRepository<PurchasePayment> payRepository, 
            IEntityRepository<ElectronicSign> eSignRepository, 
            IEntityRepository<PurchaseLog> purchaseLog,
            IEntityRepository<BeneficiaryReferenceCode> benefRefereCodeRepository, 
            IEntityRepository<ReferenceCodes> referenceCodeRepository, 
            IEntityRepository<ECommerce> eCommerceRepository,
            IEntityRepository<Subscription> subscriptionRepository)
        {
            _eSignRepository = eSignRepository;
            _purchaseOrderRepository = poRepository;
            _purchasePaymentRepository = payRepository;
            _purchaseLogRepository = purchaseLog;
            _benefRefereCodeRepository = benefRefereCodeRepository;
            _referenceCodeRepository = referenceCodeRepository;
            _eCommerceRepository = eCommerceRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public OperationResult<PurchaseOrder> Add(PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.CreatedOn = DateTime.Now;

                _purchaseOrderRepository.Add(purchaseOrder);
                _purchaseOrderRepository.Save();

                return new OperationResult<PurchaseOrder>(true, HttpStatusCode.OK)
                {
                    Entity = purchaseOrder
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                string msg;
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<PurchaseOrder>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la Orden de Compra" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<PurchaseOrder>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la Orden de Compra" };
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseOrder>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar la Orden de Compra" };
            }
        }
        public OperationResult<PurchaseOrder> Update(PurchaseOrder purchaseOrder)
        {
            try
            {
                purchaseOrder.LastModifiedOn = DateTime.Now;

                _purchaseOrderRepository.Edit(purchaseOrder);
                _purchaseOrderRepository.Save();

                return new OperationResult<PurchaseOrder>(true, HttpStatusCode.OK)
                {
                    Entity = purchaseOrder
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                string msg;
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<PurchaseOrder>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la Orden de Compra" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<PurchaseOrder>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la Orden de Compra" };
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseOrder>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar la Orden de Compra" };
            }
        }
        public OperationResult<ElectronicSign> BuyElectronicSign(ElectronicSign eSign)
        {
            try
            {
                // Primero verificamos si existe otra solicitud pendiente de procesamiento o de pago:
                var existeSolicitudPendiente = _eSignRepository.FindBy(m => m.RUC == eSign.RUC && m.Status < ElectronicSignStatusEnum.Processed).Any();
                if (existeSolicitudPendiente)
                {
                    return new OperationResult<ElectronicSign>(false, HttpStatusCode.NotFound)
                    {
                        DevMessage = $"Ya existe una solicitud para el RUC especificado: {eSign.RUC}",
                        UserMessage = "Ya existe una solicitud para este usuario"
                    };
                }

                // Primero agregamos la orden de compra:
                if (eSign.PurchaseOrder.PurchaseOrderId == 0)
                {
                    _purchaseOrderRepository.Add(eSign.PurchaseOrder);
                }

                // Luego guardamos la firma electronica
                _eSignRepository.Add(eSign);

                // Un solo metodo guarda todas las tablas relacionadas:
                _eSignRepository.Save();

                return new OperationResult<ElectronicSign>(true, HttpStatusCode.OK)
                {
                    Entity = eSign
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

                return new OperationResult<ElectronicSign>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar la Orden de Compra" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<ElectronicSign>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.Message} {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar la Orden de Compra" };
            }
            catch (Exception e)
            {
                return new OperationResult<ElectronicSign>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar la Orden de Compra" };
            }
        }
        public OperationResult<PurchasePayment> SavePurchasePayment(PurchasePayment purchasePayment)
        {
            try
            {
                purchasePayment.CreatedOn = DateTime.Now;

                _purchasePaymentRepository.Add(purchasePayment);
                _purchasePaymentRepository.Save();

                return new OperationResult<PurchasePayment>(true, HttpStatusCode.OK)
                {
                    Entity = purchasePayment
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                string msg;
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<PurchasePayment>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el pago de la Orden de Compra" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<PurchasePayment>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el pago de la Orden de Compra" };
            }
            catch (Exception e)
            {
                return new OperationResult<PurchasePayment>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el pago de la Orden de Compra" };
            }
        }
        public OperationResult<PurchasePayment> UpdatePurchasePayment(PurchasePayment purchasePayment)
        {
            try
            {
                var purchaseOrder = _purchaseOrderRepository
                    .FindBy(p => p.PurchaseOrderId == purchasePayment.PurchaseOrderId)
                    .FirstOrDefault();

                if (purchaseOrder == null)
                {
                    return new OperationResult<PurchasePayment>(false, HttpStatusCode.NotFound)
                    {
                        DevMessage = "No se pudo procesar el pago porque la orden de compra no existe",
                        UserMessage = "Ocurrio un error guardar el pago de la Orden de Compra"
                    };
                }

                purchaseOrder.LastModifiedOn = DateTime.Now;
                purchaseOrder.Status = (!string.IsNullOrEmpty(purchasePayment.ErrorCode) && purchasePayment.ErrorCode.Trim() == "00") ?
                    PurchaseOrderStatusEnum.Payed : PurchaseOrderStatusEnum.Rejected;

                // Si la orden de compra tiene una firma electronica relacionada la actualiza a aprobada o con error:
                var electronicSign = _eSignRepository.FindBy(p => p.PurchaseOrderId == purchasePayment.PurchaseOrderId).FirstOrDefault();

                if (electronicSign != null)
                {
                    electronicSign.PaymentResult = purchasePayment.ErrorMessage ?? purchasePayment.Result;
                    electronicSign.PaymentDate = DateTime.Now;
                }

                purchasePayment.LastModifiedOn = DateTime.Now;

                _purchasePaymentRepository.Save();

                return new OperationResult<PurchasePayment>(true, HttpStatusCode.OK)
                {
                    Entity = purchasePayment
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                string msg;
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<PurchasePayment>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el pago de la Orden de Compra" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<PurchasePayment>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el pago de la Orden de Compra" };
            }
            catch (Exception e)
            {
                return new OperationResult<PurchasePayment>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el pago de la Orden de Compra" };
            }
        }
        public OperationResult<ElectronicSign> GetElectronicSignByPurchase(long purchaseOrderId)
        {
            try
            {
                var firmaElectronica = _eSignRepository.All.FirstOrDefault(x => x.PurchaseOrderId == purchaseOrderId);

                if (firmaElectronica == null)
                {
                    return new OperationResult<ElectronicSign>(false, HttpStatusCode.NotFound, "No se encontro la orden de firma electronica");
                }

                return new OperationResult<ElectronicSign>(true, HttpStatusCode.OK) { Entity = firmaElectronica };
            }
            catch (Exception e)
            {
                return new OperationResult<ElectronicSign>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No se encuentra la orden de firma electronica!" };
            }
        }
        public OperationResult<ElectronicSign> GetElectronicSignById(long id)
        {
            try
            {
                var firmaElectronica = _eSignRepository.All.FirstOrDefault(x => x.Id == id);

                firmaElectronica.PurchaseOrder = GetPurchaseOrderById(firmaElectronica.PurchaseOrderId)?.Entity;

                if (firmaElectronica == null)
                {
                    var msg = "Ocurrio un error en la busqueda de la orden de firma electronica";
                    return new OperationResult<ElectronicSign>(false, HttpStatusCode.NotFound) { DevMessage = msg, UserMessage = msg };
                }

                return new OperationResult<ElectronicSign>(true, HttpStatusCode.OK) { Entity = firmaElectronica };
            }
            catch (Exception e)
            {
                return new OperationResult<ElectronicSign>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No se encuentra la orden de firma electronica!" };
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
        public OperationResult<ElectronicSign> GetElectronicSignByRUC(string ruc, bool pending = false)
        {
            try
            {
                var list = _eSignRepository.All.Where(x => x.RUC == ruc || x.Identification == ruc);

                if (pending)
                {
                    // Procesado es el unico estado que testifica que un documento ha sido recibido.
                    list = list.Where(p => p.Status < ElectronicSignStatusEnum.Processed);
                }

                if (list.Any())
                {
                    var firmaElectronica = list.OrderByDescending(p => p.Id).FirstOrDefault();

                    if (firmaElectronica != null)
                    {
                        return new OperationResult<ElectronicSign>(true, HttpStatusCode.OK) { Entity = firmaElectronica };
                    }
                }

                return new OperationResult<ElectronicSign>(false, HttpStatusCode.NotFound, "No se encuentra la solicitud de firma electronica!");
            }
            catch (Exception e)
            {
                return new OperationResult<ElectronicSign>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error en la busqueda de la orden de firma electronica" };
            }
        }
        public OperationResult<List<PurchasePayment>> GetPayments(long purchaseOrderId)
        {
            try
            {
                var payments = _purchasePaymentRepository.All.Where(p => p.PurchaseOrderId == purchaseOrderId).ToList();

                if (payments.Count > 0)
                {
                    return new OperationResult<List<PurchasePayment>>(true, HttpStatusCode.OK) { Entity = payments };
                }

                return new OperationResult<List<PurchasePayment>>(false, HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return new OperationResult<List<PurchasePayment>>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No se encuentra la orden pago!" };
            }
        }
        public OperationResult<List<PurchasePayment>> GetValidateOrderPayment(long purchaseOrderId)
        {
            try
            {
                var payments = _purchasePaymentRepository.All.Where(p => p.PurchaseOrderId == purchaseOrderId && p.Status.ToLower() == "success" && p.ErrorCode == "00").ToList();

                if (payments.Count > 0)
                {
                    return new OperationResult<List<PurchasePayment>>(true, HttpStatusCode.OK) { Entity = payments };
                }

                return new OperationResult<List<PurchasePayment>>(false, HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return new OperationResult<List<PurchasePayment>>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No se encuentra la orden de pago!" };
            }
        }
        public OperationResult<PurchasePayment> GetPaymentById(long id, bool purchaseOrder = false)
        {
            try
            {
                //var payment = _purchasePaymentRepository.All.FirstOrDefault(x => x.Id == id);
                var payment = purchaseOrder ? _purchasePaymentRepository.All.Include(p => p.PurchaseOrder).FirstOrDefault(x => x.Id == id) : _purchasePaymentRepository.All.FirstOrDefault(x => x.Id == id);
                if (payment != null)
                {
                    return new OperationResult<PurchasePayment>(true, HttpStatusCode.OK, payment);
                }

                return new OperationResult<PurchasePayment>(false, HttpStatusCode.BadRequest, "No se encuentra la orden de pago especificada!");
            }
            catch (Exception e)
            {
                return new OperationResult<PurchasePayment>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error en la busqueda de la orden de firma electronica" };
            }
        }
        public long GetElectronicSignByEmail(string ruc, string email)
        {
            try
            {
                var list = _eSignRepository.All.Where(x => x.RUC != ruc && x.Email == email);
                if (list.Any())
                {
                    var firmaElectronica = list.OrderByDescending(p => p.Id).ToList();
                    if (firmaElectronica != null)
                    {
                        return firmaElectronica.GroupBy(s=> s.RUC).Count();
                    }
                }

                return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public OperationResult<ElectronicSign> UpdateElectronicSign(ElectronicSign eSign)
        {
            try
            {

                eSign.LastModifiedOn = DateTime.Now;

                _eSignRepository.Edit(eSign);
                _eSignRepository.Save();

                return new OperationResult<ElectronicSign>(true, HttpStatusCode.OK)
                {
                    Entity = eSign
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                string msg;
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<ElectronicSign>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al actualizar su Solicitud" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<ElectronicSign>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al actualizar su Solicitud" };
            }
            catch (Exception e)
            {
                return new OperationResult<ElectronicSign>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al actualizar su Solicitud" };
            }
        }
        public OperationResult<PurchaseLog> SavePurchaseLog(PurchaseLog log)
        {
            try
            {
                _purchaseLogRepository.Add(log);
                _purchaseLogRepository.Save();

                return new OperationResult<PurchaseLog>(true, HttpStatusCode.OK)
                {
                    Entity = log
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                string msg;
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<PurchaseLog>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error guardar el Historico de Pagos" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<PurchaseLog>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error guardar el Historico de Pagos" };
            }
            catch (Exception e)
            {
                return new OperationResult<PurchaseLog>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error guardar el Historico de Pagos" };
            }
        }
        public OperationResult<ElectronicSign> GetHasElectronicSignByRUC(string ruc)
        {
            try
            {
                var list = _eSignRepository.All.Where(x => x.RUC == ruc && x.Status == ElectronicSignStatusEnum.Processed);
                if (list.Any())
                {
                    var firmaElectronica = list.OrderByDescending(p => p.Id).FirstOrDefault();

                    if (firmaElectronica != null)
                    {
                        return new OperationResult<ElectronicSign>(true, HttpStatusCode.OK) { Entity = firmaElectronica };
                    }
                }

                return new OperationResult<ElectronicSign>(false, HttpStatusCode.NotFound, "No se encuentra la solicitud de firma electronica!");
            }
            catch (Exception e)
            {
                return new OperationResult<ElectronicSign>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error en la busqueda de la orden de firma electronica" };
            }
        }
        public OperationResult<ElectronicSign> GetElectronicSignByRUC(string ruc)
        {
            try
            {
                var list = _eSignRepository.All.Where(x => x.RUC == ruc && x.Status >= ElectronicSignStatusEnum.Approved);
                if (list.Any())
                {
                    var firmaElectronica = list.OrderByDescending(p => p.Id).FirstOrDefault();

                    if (firmaElectronica != null)
                    {
                        return new OperationResult<ElectronicSign>(true, HttpStatusCode.OK) { Entity = firmaElectronica };
                    }
                }

                return new OperationResult<ElectronicSign>(false, HttpStatusCode.NotFound, "No se encuentra la solicitud de firma electronica!");
            }
            catch (Exception e)
            {
                return new OperationResult<ElectronicSign>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error en la busqueda de la orden de firma electronica" };
            }
        }
        public OperationResult<List<ElectronicSign>> GetElectronicSignByApproved()
        {
            try
            {
                var date = DateTime.Now.AddMonths(-6);
                var list = _eSignRepository.All.Where(x => x.CreatedOn >= date &&   x.Status == ElectronicSignStatusEnum.Approved).ToList();
                if (list.Any())
                {
                    return new OperationResult<List<ElectronicSign>>(true, HttpStatusCode.OK) { Entity = list };
                }

                return new OperationResult<List<ElectronicSign>>(false, HttpStatusCode.NotFound, "No hay solicitudes en estado aprobadas!");
            }
            catch (Exception e)
            {
                return new OperationResult<List<ElectronicSign>>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error en la busqueda la firma electronica" };
            }
        }
        public OperationResult<ElectronicSign> GetElectronicSignByToken(string token)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    var _eSign = _eSignRepository.All.Where(x => x.RequestNumber == token).FirstOrDefault();
                    if (_eSign != null)
                    {
                        return new OperationResult<ElectronicSign>(true, HttpStatusCode.OK) { Entity = _eSign };
                    }
                }
                return new OperationResult<ElectronicSign>(false, HttpStatusCode.NotFound, "No se encuentra la solicitud de firma electronica!");
            }
            catch (Exception e)
            {
                return new OperationResult<ElectronicSign>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error en la busqueda de la orden de firma electronica" };
            }
        }

        public OperationResult<List<ECommerce>> GeteCommerces()
        {
            try
            {
                var ecommerce = _eCommerceRepository.All.Where(p => p.IsEnabled == true).ToList();

                if (ecommerce.Count > 0)
                {
                    return new OperationResult<List<ECommerce>>(true, HttpStatusCode.OK) { Entity = ecommerce };
                }

                return new OperationResult<List<ECommerce>>(false, HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return new OperationResult<List<ECommerce>>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No hay punto de pagos disponibles...!" };
            }
        }
        public OperationResult<ECommerce> GeteCommerceByCode(string code)
        {
            try
            {
                var ecommerce = _eCommerceRepository.All.Where(p => p.Code == code && p.IsEnabled == true).FirstOrDefault();

                if (ecommerce != null)
                {
                    return new OperationResult<ECommerce>(true, HttpStatusCode.OK) { Entity = ecommerce };
                }

                return new OperationResult<ECommerce>(false, HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                return new OperationResult<ECommerce>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "No esta disponible el punto de pago...!" };
            }
        }

        #region Benifiaciarios con descuentos 

        public OperationResult<BeneficiaryReferenceCode> GetBeneficiaryReferenceCode(string RUC)
        {
            try
            {
                //verificamos si existe codigo de descuento para el ruc:
                var referenceCode = _benefRefereCodeRepository.FindBy(m => (m.Identification == RUC || m.Identification == RUC.Substring(0, 10)) && m.Status == ReferenceCodeStatusEnum.WithoutApply)
                    .Include(m => m.ReferenceCodes).FirstOrDefault();

                if (referenceCode != null)
                {
                    return new OperationResult<BeneficiaryReferenceCode>(true, HttpStatusCode.OK)
                    {
                        Entity = referenceCode
                    };
                }

                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.NotFound, "No se encuentra código de descuento!");
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

                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error verificar un código descuento" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.Message} {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error verificar un código descuento" };
            }
            catch (Exception e)
            {
                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error verificar un código descuento" };
            }
        }
        public OperationResult<ReferenceCodes> GetReferenceCodes(string code, string ruc)
        {
            try
            {
                //verificamos si existe el codigo promosional
                var referenceCode = _referenceCodeRepository.FindBy(m => m.Code == code && m.IsEnabled == true)
                                                            .Include(m => m.Agreement)
                                                            .FirstOrDefault();

                if (referenceCode != null)
                {
                    if (referenceCode.SpecialPromotion && referenceCode.ValidateData)
                    {
                        if (DateTime.Now <= referenceCode.EndDate)
                        {

                            if(referenceCode.PromotionByPlan.Equals("T01"))
                            {
                                return new OperationResult<ReferenceCodes>(true, HttpStatusCode.OK)
                                {
                                    Entity = referenceCode
                                };
                            }
                            else
                            {
                                var codPlan = _subscriptionRepository.FindBy(sub => sub.RUC == ruc)
                                                                .Include(m => m.LicenceType)
                                                                .Select(lic => lic.LicenceType.Code)
                                                                .First();
                                if (!string.IsNullOrWhiteSpace(codPlan) && referenceCode.PromotionByPlan.Equals(codPlan))
                                {
                                    return new OperationResult<ReferenceCodes>(true, HttpStatusCode.OK)
                                    {
                                        Entity = referenceCode
                                    };
                                }
                            }                           
                            return new OperationResult<ReferenceCodes>(false, HttpStatusCode.NotFound, "El código de descuento no esta activo.!");
                        }
                        else {
                            return new OperationResult<ReferenceCodes>(false, HttpStatusCode.NotFound, "El código de descuento no esta activo.!");
                        }
                    }
                    else if (referenceCode.ValidateData)
                    {
                        if (DateTime.Now >= referenceCode.StartDate  && DateTime.Now <= referenceCode.EndDate)
                        {
                            return new OperationResult<ReferenceCodes>(true, HttpStatusCode.OK)
                            {
                                Entity = referenceCode
                            };
                        }
                        else
                        {
                            return new OperationResult<ReferenceCodes>(false, HttpStatusCode.NotFound, "El código de descuento no esta activo.!");
                        }
                    }
                    else
                    {
                        return new OperationResult<ReferenceCodes>(true, HttpStatusCode.OK)
                        {
                            Entity = referenceCode
                        };
                    }                   
                }

                return new OperationResult<ReferenceCodes>(false, HttpStatusCode.NotFound, "No se encuentra código promosional!");
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

                return new OperationResult<ReferenceCodes>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error verificar un código promocional" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<ReferenceCodes>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.Message} {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error verificar un código descuento" };
            }
            catch (Exception e)
            {
                return new OperationResult<ReferenceCodes>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error verificar un código promocional" };
            }
        }
        public OperationResult<BeneficiaryReferenceCode> AddBeneficiaryReferenceCode(BeneficiaryReferenceCode model)
        {
            try
            {
                _benefRefereCodeRepository.Add(model);
                _benefRefereCodeRepository.Save();

                return new OperationResult<BeneficiaryReferenceCode>(true, HttpStatusCode.OK)
                {
                    Entity = model
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                string msg;
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al actualizar su Solicitud" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al actualizar su Solicitud" };
            }
            catch (Exception e)
            {
                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al actualizar su Solicitud" };
            }
        }
        public OperationResult<BeneficiaryReferenceCode> UpdateBeneficiaryReferenceCode(BeneficiaryReferenceCode beniReferCode)
        {
            try
            {
                var referenceCode = _benefRefereCodeRepository.FindBy(m => m.Id == beniReferCode.Id).FirstOrDefault();
                referenceCode.LastModifiedOn = DateTime.Now;
                referenceCode.Status = ReferenceCodeStatusEnum.Applied;
                referenceCode.StatusMsg = $"Aplicado:Código de descuento por {referenceCode.ReferenceCodes.Description}";
                referenceCode.IssuerId = beniReferCode.IssuerId;

                _benefRefereCodeRepository.Save();

                return new OperationResult<BeneficiaryReferenceCode>(true, HttpStatusCode.OK)
                {
                    Entity = referenceCode
                };
            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                string msg;
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }

                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al actualizar su Solicitud" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al actualizar su Solicitud" };
            }
            catch (Exception e)
            {
                return new OperationResult<BeneficiaryReferenceCode>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al actualizar su Solicitud" };
            }
        }
        public OperationResult<ReferenceCodes> GetReferenceCodesById(long id)
        {
            try
            {
                //verificamos si existe el codigo promosional
                var referenceCode = _referenceCodeRepository.FindBy(m => m.Id == id && m.IsEnabled == true)
                    .Include(m => m.Agreement).FirstOrDefault();

                if (referenceCode != null)
                {
                    return new OperationResult<ReferenceCodes>(true, HttpStatusCode.OK)
                    {
                        Entity = referenceCode
                    };
                }

                return new OperationResult<ReferenceCodes>(false, HttpStatusCode.NotFound, "No se encuentra código promosional!");
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

                return new OperationResult<ReferenceCodes>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error verificar un código promocional" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<ReferenceCodes>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.Message} {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error verificar un código descuento" };
            }
            catch (Exception e)
            {
                return new OperationResult<ReferenceCodes>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error verificar un código promocional" };
            }
        }

        #endregion


    }
}
