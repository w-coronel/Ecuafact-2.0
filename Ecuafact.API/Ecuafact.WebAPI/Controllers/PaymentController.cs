using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Entities.VPOS2;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.PayMe;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// Procesamiento de Ordenes de Compra:
    /// </summary>

    public partial class PaymentController : ApiController
    {
        IPurchaseOrderService __purchaseOrderService;
        ISubscriptionService _subscriptionService;
        IIssuersService _issuersService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="poService"></param>
        /// <param name="subscripService"></param>
        /// <param name="issuersService"></param>
        public PaymentController(IPurchaseOrderService poService, ISubscriptionService subscripService, IIssuersService issuersService)
        {
            __purchaseOrderService = poService;
            _subscriptionService = subscripService;
            _issuersService = issuersService;
        }

        /// <summary>
        /// 
        /// </summary>
        public IssuerDto Issuer => this.GetAuthenticatedIssuer();

        /// <summary>
        /// Actualiza el estado de la operación
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Payment/{id}/Update")]
        [AllowAnonymous]
        public async Task<OperationResult<PaymentRequestModel>> UpdateOperationStatus(long id, PaymentResultModel model)
        {
            try
            {
                var authToken = Request.Headers?.Authorization?.Parameter;

                if (!Constants.ServiceToken.Equals(authToken, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "No Autorizado");
                }

                // Actualizamos el log de pagos
                var log = new PurchaseLog
                {
                    Date = model.Date ?? DateTime.Now,
                    ErrorCode = model.ErrorCode,
                    Message = model.Message,
                    OperationNumber = model.OperationNumber,
                    Result = JsonConvert.SerializeObject(model)
                };

                __purchaseOrderService.SavePurchaseLog(log);

                Logger.Log("PAYMENT.UPDATESTATUS", "Se ha recibido el siguiente pago electrónico:", JsonConvert.SerializeObject(model));

                // Ahora actualizamos el pago con la informacion mas actualizada del servicio.
                var payment = __purchaseOrderService.GetPaymentById(id);

                if (payment.IsSuccess)
                {
                    var result = await __purchaseOrderService.ValidatePayment(payment.Entity, model);

                    if (result.IsSuccess)
                    {
                        // Actualizamos los estados de los documentos y enviamos la informacion.
                        var esign = __purchaseOrderService.GetElectronicSignByPurchase(payment.Entity.PurchaseOrderId);

                        if (esign.IsSuccess) // Successfull
                        {
                            if (result.Entity?.ErrorCode?.Trim() == "00")
                            {
                                // Procesamos la solicitud de firma electronica:
                                await __purchaseOrderService.Process(esign.Entity);

                                // Generamos la factura:
                                await __purchaseOrderService.GenerateInvoice(esign.Entity);

                                // Procesamos la solicitud de suscripcion:                                
                                //await _subscriptionService.UpdateSubscriptionProcess(esign.Entity);
                            }

                            return new OperationResult<PaymentRequestModel>(true, HttpStatusCode.OK, new PaymentRequestModel(result.Entity));
                        }
                        else
                        {
                            var susbc = _subscriptionService.GetSubscriptionByPurchase(payment.Entity.PurchaseOrderId);
                            if (susbc.IsSuccess) // Successfull
                            {
                                if (result.Entity?.ErrorCode?.Trim() == "00")
                                {
                                    var _sub = _subscriptionService.GetSubscription(susbc.Entity.SubscriptionId);
                                    // Procesamos la solicitud de suscripcion:
                                    var res = await _subscriptionService.SubscriptionProcess(_sub, susbc.Entity);
                                    var issuer = _issuersService.GetIssuer(_sub.RUC);
                                    issuer.EmailActivationAccount(res.Entity, 0, "R");
                                    // Generamos la factura:
                                    await _subscriptionService.SubscriptionGenerateInvoice(_sub, susbc.Entity);

                                }
                                var resul = new PaymentRequestModel(result.Entity);
                                resul.SubscriptionActiva = true;
                                resul.TipoProceso = 1;

                                return new OperationResult<PaymentRequestModel>(true, HttpStatusCode.OK, resul);
                            }
                        }
                    }

                }

                return await Task.FromResult(new OperationResult<PaymentRequestModel>(false, HttpStatusCode.NotFound, "El numero de Orden de Pago no existe"));
            }
            catch (Exception ex)
            {
                return new OperationResult<PaymentRequestModel>(ex);
            }
        }

        /// <summary>
        /// Actualiza el estado de la operación
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Payment/Voucher")]
        [AllowAnonymous]
        public async Task<OperationResult<PaymentVouchersModel>> SavePaymentVoucher(PaymentVoucherRequest model)
        {

            try
            {
                var authToken = Request.Headers?.Authorization?.Parameter;
                if (!Constants.ServiceToken.Equals(authToken, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "No Autorizado");
                }

                var payment = __purchaseOrderService.GetPurchaseOrderById(model.PurchaseOrderId);

                if (payment.IsSuccess)
                {
                    // Primero Guardamos los documentos usando la identidad del usuario
                    var basename = $"{payment.Entity.Identification}";
                    var paymentVoucher = GetFilename(basename, $"Voucher_{payment.Entity.PurchaseOrderId}", model.Ext); // comprobante de pago type png
                    var _paymentVoucher = new PaymentVouchersModel();
                    payment.Entity.PaymentVoucher = paymentVoucher;
                    payment.Entity.BankTransfer = true;
                    payment.Entity.Status = PurchaseOrderStatusEnum.Transfer;
                    var result = await Task.FromResult(__purchaseOrderService.Update(payment.Entity));
                    if (result.IsSuccess)
                    {
                        var electronicSign = __purchaseOrderService.GetElectronicSignByPurchase(model.PurchaseOrderId);
                        if (electronicSign.IsSuccess)
                        {
                            electronicSign.Entity.Status = ElectronicSignStatusEnum.ValidatingPayment;
                            electronicSign.Entity.StatusMsg = "Procesando comprobante de pago...";
                            electronicSign.Entity.LastModifiedOn = DateTime.Now;
                            electronicSign.Entity.PaymentDate = DateTime.Now;
                            electronicSign.Entity.PaymentResult = "Transferencia bancaria";
                            __purchaseOrderService.UpdateElectronicSign(electronicSign.Entity);
                            _paymentVoucher.ElectronicSign = electronicSign.Entity;
                            var path = $@"{electronicSign.Entity.Identification}\PAYMENTVOUCHER";
                            SaveFile(paymentVoucher, model.PaymentVoucherRaw, path);
                            SendMail(_paymentVoucher.ElectronicSign.PurchaseOrder, "FE");
                        }
                        else
                        {
                            var subscription = _subscriptionService.GetSubscriptionByPurchase(model.PurchaseOrderId);
                            if (subscription.IsSuccess)
                            {
                                subscription.Entity.Status = PurchaseOrderSubscriptionStatusEnum.ValidatingPayment;
                                subscription.Entity.Subscription.Status = SubscriptionStatusEnum.ValidatingPayment;
                                subscription.Entity.Subscription.StatusMsg = SubscriptionStatusEnum.ValidatingPayment.GetDisplayValue();
                                subscription.Entity.Subscription.LastModifiedOn = DateTime.Now;
                                _paymentVoucher.PurchaseSubscription = _subscriptionService.UpdatePurchaseSubscription(subscription.Entity).Entity;
                                var path = $@"{subscription.Entity.Subscription.RUC}\PAYMENTVOUCHER";
                                SaveFile(paymentVoucher, model.PaymentVoucherRaw, path);
                                SendMail(_paymentVoucher.PurchaseSubscription.PurchaseOrder, "SA");
                            }
                        }


                    }

                    Logger.Log("PAYMENT.SAVEVOUCHER", "Se ha recibido el comprobante de pago por transferencia bancaria:", JsonConvert.SerializeObject(payment.Entity));

                    return await Task.FromResult(new OperationResult<PaymentVouchersModel>(true, HttpStatusCode.OK, "Se  guardo el comprobante de pago con exíto")
                    {
                        Entity = _paymentVoucher
                    });
                }

                return await Task.FromResult(new OperationResult<PaymentVouchersModel>(false, HttpStatusCode.NotFound, "El numero de orden de compra no existe"));
            }
            catch (Exception ex)
            {
                return new OperationResult<PaymentVouchersModel>(ex);
            }
        }

        /// <summary>
        /// Codigos promocionales
        /// </summary>
        /// <remarks>
        /// verifica si el codigo enviado es promocional.
        /// </remarks>
        /// <param name="code"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpGet, Route("Payment/coupon/{code}")]
        public async Task<OperationResult<ReferenceCodes>> ValidateCoupon(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "Código promocional invalido"); ;
            }
            var result = await Task.FromResult(__purchaseOrderService.GetReferenceCodes(code, Issuer.RUC));
            if (result.IsSuccess)
            {
                return result;
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "Código promocional invalido");
        }


        /// <summary>
        /// boton de pagos
        /// </summary>
        /// <remarks>
        ///Devuelve la lista de boton de pagos disponibles
        /// </remarks>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpGet, Route("Payment/{id}/Purchase")]
        public async Task<OperationResult<PaymentRequestModel>> GetPurchasePayment(long id)
        {
            var result = __purchaseOrderService.GetPaymentById(id, true);
            if (result.IsSuccess)
            {
                return await Task.FromResult(new OperationResult<PaymentRequestModel>(true, HttpStatusCode.OK, new PaymentRequestModel(result.Entity)));
            }

            return await Task.FromResult(new OperationResult<PaymentRequestModel>(false, HttpStatusCode.NotFound, "No se encontro la orden de pago"));
        }

        /// <summary>
        /// Registrar el estado de la transacíon del pago
        /// </summary>       
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Payment/Transaction")]
        public async Task<OperationResult<PaymentRequestModel>> PaymentTransactionStatus(PaymentTransaction model)
        {

            try
            {
                var authToken = Request.Headers?.Authorization?.Parameter;
                if (!Constants.ServiceToken.Equals(authToken, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "No Autorizado");
                }

                var purchaseOrder = __purchaseOrderService.GetPurchaseOrderById(model.PurchaseOrder.PurchaseOrderId);
                if (purchaseOrder.IsSuccess)
                {
                    model.PurchaseOrder = purchaseOrder.Entity;
                    var result = __purchaseOrderService.SavePurchasePayment(model.ToPurchasePayment());
                    if (result.IsSuccess)
                    {
                        // Actualizamos el log de pagos
                        var log = new PurchaseLog
                        {
                            Date = DateTime.Now,
                            ErrorCode = model.transaction.carrier_code,
                            Message = $"{model.transaction.status}-{model.transaction.current_status}",
                            OperationNumber = $"{result.Entity.Id:000000000}",
                            Result = JsonConvert.SerializeObject(model)
                        };

                        __purchaseOrderService.SavePurchaseLog(log);

                        Logger.Log("PAYMENT.UPDATESTATUS", "Se ha recibido el siguiente pago electrónico:", JsonConvert.SerializeObject(model));

                        if (model.transaction.carrier_code?.Trim() == "00")
                        {
                            // Actualizamos los estados de los documentos y enviamos la informacion.
                            var esign = __purchaseOrderService.GetElectronicSignByPurchase(model.PurchaseOrder.PurchaseOrderId);
                            if (esign.IsSuccess) // Successfull
                            {
                                // Procesamos la solicitud de firma electronica:
                                await __purchaseOrderService.Process(esign.Entity);

                                // Generamos la factura:
                                await __purchaseOrderService.GenerateInvoice(esign.Entity);

                                var obj = new PaymentRequestModel(result.Entity);

                                //enviar email por la transacion del pago
                                var _Issuer = _issuersService.GetIssuer(esign.Entity.IssuerId)?.ToIssuerDto();
                                SendMailPayment(obj, _Issuer.ToIssuer());

                                return new OperationResult<PaymentRequestModel>(true, HttpStatusCode.OK, obj);
                            }
                            else
                            {
                                var susbc = _subscriptionService.GetSubscriptionByPurchase(model.PurchaseOrder.PurchaseOrderId);
                                if (susbc.IsSuccess) // Successfull
                                {
                                    var _sub = _subscriptionService.GetSubscription(susbc.Entity.SubscriptionId);

                                    // Procesamos la solicitud de suscripcion:
                                    //var res = await _subscriptionService.SubscriptionProcess(_sub, susbc.Entity);

                                    // Generamos la factura:
                                    var res = await _subscriptionService.SubscriptionGenerateInvoice(_sub, susbc.Entity);

                                    var resul = new PaymentRequestModel(result.Entity);

                                    //enviar email por compra del plan
                                    var __issuer = _sub.Issuer.ToIssuerDto();
                                    __issuer.ToIssuer().EmailActivationAccount(res.Entity.Subscription, 0, "R");

                                    //enviar email por la transacion
                                    SendMailPayment(resul, __issuer.ToIssuer());

                                    resul.SubscriptionActiva = true;
                                    resul.TipoProceso = 1;

                                    return new OperationResult<PaymentRequestModel>(true, HttpStatusCode.OK, resul);
                                }
                            }
                        }

                        //enviar email por la transacion rechazada
                        var _issuer = _issuersService.GetIssuer(model.PurchaseOrder.IssuerId)?.ToIssuerDto();
                        SendMailPayment(new PaymentRequestModel(result.Entity), Issuer.ToIssuer());

                        return new OperationResult<PaymentRequestModel>(true, HttpStatusCode.OK, new PaymentRequestModel(result.Entity));
                    }

                    return await Task.FromResult(new OperationResult<PaymentRequestModel>(false, HttpStatusCode.NotFound, result.DevMessage));
                }

                return await Task.FromResult(new OperationResult<PaymentRequestModel>(false, HttpStatusCode.NotFound, "El número de Orden de Pago no existe"));
            }
            catch (Exception ex)
            {
                return new OperationResult<PaymentRequestModel>(ex);
            }
        }


        /// <summary>
        /// boton de pagos
        /// </summary>
        /// <remarks>
        ///Devuelve la lista de boton de pagos disponibles
        /// </remarks>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpGet, Route("Payment/checkout-payment")]
        public IEnumerable<ECommerce> GetTypeCheckoutPayment()
        {
            var result = __purchaseOrderService.GeteCommerces();
            if (result.IsSuccess)
            {
                return result.Entity.Where(o => o.IsEnabled).ToList();
            }

            return null;
        }

        /// <summary>
        /// Registrar el estado de la transacíon del pago
        /// </summary>       
        /// <param name="id"></param>
        /// <returns></returns>
        [EcuafactExpressAuthorize]
        [HttpPost, Route("Payment/transaction/{id}/refund")]
        public async Task<OperationResult<PurchasePayment>> Refund(long id = 0)
        {

            try
            {
                if (id == 0)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "Debe suministrar el id de la transacción");
                }

                var transaction = __purchaseOrderService.GetPaymentById(id);
                if (!transaction.IsSuccess)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, $"{transaction.UserMessage}, {transaction.DevMessage}");
                }

                return await Request.Refund(transaction.Entity);
            }
            catch (Exception ex)
            {
                return new OperationResult<PurchasePayment>(ex);
            }
        }


        /// <summary>
        /// Validar si la orden ya esta pagada
        /// </summary>
        /// <param name="purchaseOrderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("payment/{purchaseOrderId}/validate-payment")]
        public async Task<OperationResult<PaymentRequestModel>> ValidatePurchasePayment(long purchaseOrderId)
        {
            var purchaseOrder = __purchaseOrderService.GetPurchaseOrderById(purchaseOrderId);

            if (purchaseOrder.IsSuccess)
            {
                // 1. Validamos los pagos concretados a esta orden:
                var validation = __purchaseOrderService.GetValidateOrderPayment(purchaseOrderId);
                if (validation.IsSuccess)
                {
                    return new OperationResult<PaymentRequestModel>(true, HttpStatusCode.OK, new PaymentRequestModel(validation.Entity.FirstOrDefault()));
                }
            }

            return new OperationResult<PaymentRequestModel>(false, HttpStatusCode.NotFound, "La orden esta pendiente por pagar");
        }

        /// <summary>
        /// Obtener informacion de un emisor por el Numero de RUC
        /// </summary>
        /// <param name="ruc"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("payment/{ruc}/issuers")]
        public IssuerDto GetIssuerByRuc(string ruc = null)
        {
            
            if (!string.IsNullOrEmpty(ruc))
            {
                var issuer = _issuersService.GetIssuer(ruc);
                return issuer.ToIssuerDto();
            }

            return default;
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
                else if (!path.Contains("\\") && !path.Contains("/"))
                {
                    path = Path.Combine(Constants.ResourcesLocation, path);
                }
                else
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
      
        private static void SendMail(PurchaseOrder orden, string type)
        {
            if (orden != null)
            {
                var msj = "";
                if (type == "FE")
                {
                    msj = "Solicitud de firma elctronica";
                }
                else if (type == "SA")
                {
                    msj = "Solicitud suscripcion anual";
                }
                var email = Emailer.Create("notification", $"ECUAFACT: Recibo de pago orden: {orden.PurchaseOrderId}", Constants.emailNotification);
                email.Parameters.Add("Cliente", orden.BusinessName);
                email.Parameters.Add("purchaseOrder", orden.PurchaseOrderId.ToString());
                email.Parameters.Add("ruc", orden.Identification);
                email.Parameters.Add("producto", msj);
                email.Send();
            }
        }

        private static void SendMailPayment(PaymentRequestModel orden, Issuer issuer)
        {
            if (orden != null)
            {
                var email = Emailer.Create("transaction", $"ECUAFACT: Confirmación Transacción: {orden.TransactionId}", issuer.Email);
                var _valor = "";
                var _numAutorizacion = "";
                var _fecha = $"{orden.PaymentDate}";
                var _numTranzacion = $"{orden.TransactionId}";
                var _mensaje = "Continúa tu proceso, te invitamos a ingresar a nuestra plataforma de Facturación Electrónica.";
                if (orden.ErrorCode == "00")
                {
                    _valor = $"{(Convert.ToDecimal(orden.PurchaseAmount) / 100)}";
                    _numAutorizacion = $"{orden.PurchaseOperationNumber}";

                }
                else
                {
                    _mensaje = $"{orden.ErrorMessage}";
                }
                email.Parameters.Add("Cliente", issuer.BussinesName);
                email.Parameters.Add("NumOrden", orden.PurchaseOperationNumber);
                email.Parameters.Add("Estado", (orden.ErrorCode == "00" ? "APROBADA" : "RECHAZADA"));
                email.Parameters.Add("NumTransacion", _numTranzacion);
                email.Parameters.Add("Valor", _valor);
                email.Parameters.Add("NumAutorizacion", _numAutorizacion);
                email.Parameters.Add("FechaHora", _fecha);
                email.Parameters.Add("Mensaje", _mensaje);
                email.Send();
            }
        }
    }
}
