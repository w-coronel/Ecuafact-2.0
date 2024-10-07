using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Entities.VPOS2;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.Models.Dtos;
using Ecuafact.WebAPI.PayMe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// Procesamiento de Ordenes de Compra:
    /// </summary>
    [EcuafactExpressAuthorize]

    public class PurchaseOrderController : ApiController
    {
        IPurchaseOrderService __purchaseOrderService;
        public PurchaseOrderController(IPurchaseOrderService poService)
        {
            __purchaseOrderService = poService;
        }

        public IssuerDto Issuer
        {
            get
            {

                return HttpContext.Current.Session.GetAuthenticatedIssuerSession() as IssuerDto;
            }
        }


        [HttpPost]
        [Route("purchaseOrder/{purchaseOrderId}/invoice")]
        public async Task<OperationResult<ElectronicSign>> GenerateInvoice(long purchaseOrderId)
        {
            var esign = __purchaseOrderService.GetElectronicSignByPurchase(purchaseOrderId);

            if (esign.IsSuccess)
            {
                if (esign.Entity.IssuerId != Issuer.Id)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "Usted no esta autorizado para generar este documento");
                }

                return await __purchaseOrderService.GenerateInvoice(esign.Entity);
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se encuentra la orden de compra");
        }

        /// <summary>
        /// Devuelve el objeto de pago para la orden de compra.
        /// </summary>
        /// <param name="purchaseOrderId"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("purchaseOrder/{purchaseOrderId}/paybill")]
        //public async Task<PaymentRequestModel> GeneratePayOrder(long purchaseOrderId)
        //{
        //    var purchaseOrder = __purchaseOrderService.GetPurchaseOrderById(purchaseOrderId);

        //    if (purchaseOrder.IsSuccess)
        //    {
        //        // 1. Validamos los pagos concretados a esta orden:
        //        var validation = await ValidateOrderPayment(purchaseOrderId, true);

        //        if (validation.IsSuccess)
        //        {
        //            throw Request.BuildHttpErrorException(HttpStatusCode.Conflict, "La Orden seleccionada ya ha sido pagada!");
        //        }
        //        else
        //        {
        //            // Verifico si hay algun pago no procesado para reutirizarlo
        //            var purchasePayment = validation.Entity.FirstOrDefault(p => !p.Processed);

        //            if (purchasePayment == null)
        //            {
        //                // Si no existe la orden de pago la genero para su procesamiento
        //                var walletUser = GetWalletUser(purchaseOrder.Entity);

        //                purchasePayment = new PurchasePayment
        //                {
        //                    PurchaseOrderId = purchaseOrder.Entity.PurchaseOrderId,
        //                    PurchaseOrder = purchaseOrder.Entity,
        //                    AcquirerId = Constants.VPOS2.IDAcquirer,
        //                    IdCommerce = Constants.VPOS2.IDCommerceCode,
        //                    PurchaseAmount = purchaseOrder.Entity.Total,
        //                    PurchaseCurrencyCode = Constants.VPOS2.CURRENCY_CODE,
        //                    UserCodePayme = walletUser.CodAsoCardHolderWallet
        //                };

        //                var result = __purchaseOrderService.SavePurchasePayment(purchasePayment);

        //                if (result.IsSuccess)
        //                {
        //                    purchasePayment = result.Entity;
        //                }
        //                else
        //                {
        //                    throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "No se pudo generar el pago para esta solicitud");
        //                }
        //            }

        //            return await Task.FromResult(new PaymentRequestModel(purchasePayment));
        //        } 
        //    }

        //    throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se encuentra la orden de compra");
        //}

        [HttpPost]
        [Route("purchaseOrder/{purchaseOrderId}/paybill")]
        public async Task<PaymentRequestModel> GeneratePayOrder(long purchaseOrderId, string eCommerceCode = "")
        {
            var purchaseOrder = __purchaseOrderService.GetPurchaseOrderById(purchaseOrderId);

            if (purchaseOrder.IsSuccess)
            {
                var ecommerce = __purchaseOrderService.GeteCommerceByCode(eCommerceCode);

                if (eCommerceCode == Constants.alignet)
                {
                    // 1. Validamos los pagos concretados a esta orden:
                    var validation = await ValidateOrderPayment(purchaseOrderId, true);

                    if (validation.IsSuccess)
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.Conflict, "La Orden seleccionada ya ha sido pagada!");
                    }
                    else
                    {
                        // Verifico si hay algun pago no procesado para reutirizarlo
                        var purchasePayment = validation.Entity.FirstOrDefault(p => !p.Processed);

                        if (purchasePayment == null)
                        {
                            // Si no existe la orden de pago la genero para su procesamiento
                            var walletUser = GetWalletUser(purchaseOrder.Entity);

                            purchasePayment = new PurchasePayment
                            {
                                PurchaseOrderId = purchaseOrder.Entity.PurchaseOrderId,
                                PurchaseOrder = purchaseOrder.Entity,
                                AcquirerId = Constants.VPOS2.IDAcquirer,
                                IdCommerce = Constants.VPOS2.IDCommerceCode,
                                PurchaseAmount = purchaseOrder.Entity.Total,
                                PurchaseCurrencyCode = Constants.VPOS2.CURRENCY_CODE,
                            };

                            var result = __purchaseOrderService.SavePurchasePayment(purchasePayment);

                            if (result.IsSuccess)
                            {
                                purchasePayment = result.Entity;
                            }
                            else
                            {
                                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, "No se pudo generar el pago para esta solicitud");
                            }
                        }

                        return await Task.FromResult(new PaymentRequestModel(purchasePayment));
                    }

                }
                else
                {
                    // 1. Validamos los pagos concretados a esta orden:
                    var validation = __purchaseOrderService.GetValidateOrderPayment(purchaseOrderId);
                    if (validation.IsSuccess)
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"La orden de compra {purchaseOrderId} ya esta pagada");
                    }
                    var purchasePayment = new PurchasePayment
                    {
                        PurchaseOrderId = purchaseOrder.Entity.PurchaseOrderId,
                        PurchaseOrder = purchaseOrder.Entity,
                        IdCommerce = ecommerce?.Entity?.Id.ToString(),
                        PurchaseAmount = purchaseOrder.Entity.Total,
                        PurchaseCurrencyCode = ecommerce?.Entity?.Code
                    };

                    return await Task.FromResult(new PaymentRequestModel(purchasePayment));
                }

            }

            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se encuentra la orden de compra");
        }

        /// <summary>
        /// Devuelve el objeto de pago para la orden de compra.
        /// </summary>
        /// <param name="purchaseOrderId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("purchaseOrder/{purchaseOrderId}/transfer")]
        public async Task<PaymentRequestModel> GeneratePayTransferOrder(long purchaseOrderId)
        {
            var validation = __purchaseOrderService.GetValidateOrderPayment(purchaseOrderId);
            if (validation.IsSuccess)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"La orden de compra {purchaseOrderId} ya esta pagada");
            }

            var purchaseOrder = __purchaseOrderService.GetPurchaseOrderById(purchaseOrderId);
            if (purchaseOrder.IsSuccess)
            {
                if (purchaseOrder.Entity.IssuerId != Issuer.Id)
                {
                    throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "Usted no esta autorizado para generar este documento");
                }

                var purchasePayment = new PurchasePayment
                {
                    PurchaseOrderId = purchaseOrder.Entity.PurchaseOrderId,
                    PurchaseOrder = purchaseOrder.Entity
                };

                return await Task.FromResult(new PaymentRequestModel(purchasePayment));

            }

            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se encuentra la orden de compra");
        }

        /// <summary>
        /// Registrar Pago de orden de compra
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("purchaseOrder/{id}")]
        public PurchaseOrder GetPurchaseOrder(long id)
        {
            var purchaseOrder = __purchaseOrderService.GetPurchaseOrderById(id);

            if (purchaseOrder.IsSuccess)
            {
                ValidateOrderPayment(id);
                return purchaseOrder.Entity;
            }

            throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se encuentra la orden de compra");
        }

        private async Task<OperationResult<List<PurchasePayment>>> ValidateOrderPayment(long purchaseOrderId, bool throwOnError = false)
        {
            var payments = __purchaseOrderService.GetPayments(purchaseOrderId);

            if (payments.IsSuccess)
            {
                foreach (var payment in payments.Entity)
                {
                    OperationResult<PurchasePayment> result;
                    // Si no se ha procesado aun: Verificamos la información
                    if (string.IsNullOrEmpty(payment.PurchaseVerification))
                    {
                        result = await ValidatePayment(payment);
                    }
                    else
                    {
                        result = new OperationResult<PurchasePayment>(true, HttpStatusCode.OK, payment);
                    }

                    if (result.IsSuccess)
                    {
                        var purchasePayment = result.Entity;
                        if ((!string.IsNullOrEmpty(purchasePayment.ErrorCode) && purchasePayment.ErrorCode.Trim() == "00"))
                        {
                            // PAGADO
                            return await Task.FromResult(payments);
                        }
                    }
                }
            }

            return await Task.FromResult(new OperationResult<List<PurchasePayment>>(false, HttpStatusCode.PaymentRequired, payments?.Entity ?? new List<PurchasePayment>()));
        }

        private async Task<OperationResult<PurchasePayment>> ValidatePayment(PurchasePayment payment)
        {
            var request = new PaymentRequestModel(payment);
            var response = await WalletCustomerService.GetPaymentStatus(request);

            if (response.IsSuccess)
            {
                // Es un pago procesado.
                payment.UpdatePayment(response.Entity);
            }
            else
            {
                // Es un pago no procesado. 
                // Lo anulamos para casos de log:
                payment.AuthorizationResult = OperationResultEnum.Invalido; // Inválido
                payment.AuthorizationCode = null; // No Autorizado
                payment.ErrorMessage = "El pago no fue procesado";
                payment.ErrorCode = "6002"; // 6002 - Order Number does not exists 
                payment.Processed = false;
            }

            payment.PurchaseVerification = request.PurchaseVerification;

            return await Task.FromResult(__purchaseOrderService.UpdatePurchasePayment(payment));
        }                

        private WalletCustomerResult GetWalletUser(PurchaseOrder request)
        {
            return WalletCustomerService.RegisterCustomer(new WalletCustomerRequest
            {
                CustomerCode = request.Identification.Substring(0, 10),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            });
        }
    }
}
