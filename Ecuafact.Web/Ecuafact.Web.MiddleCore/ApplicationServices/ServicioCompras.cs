using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecuafact.Web.Domain.Entities;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Ecuafact.Web.Domain.Services;
using System.Net;
using System.Configuration;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public class ServicioCompras
    {        
        public async static Task<PaymentRequestModel> GenerarPagoAsync(string token, int purchaseOrderNumber, string typeCommerce)
        {
            PaymentRequestModel paymentRequest = default;

            try
            {
                var httpClient = ClientHelper.GetClient(token);

                var response = await httpClient.PostAsync($"{Constants.WebApiUrl}/PurchaseOrder/{purchaseOrderNumber}/PayBill?eCommerceCode={typeCommerce}", default);

                if (response.IsSuccessStatusCode)
                {
                    paymentRequest = await response.GetContentAsync<PaymentRequestModel>();
                }
            }
            catch (Exception ex)
            {
                // Do Nothing
                ex.ToString();
            }

            return paymentRequest;
        }

        public async static Task<PaymentRequestModel> GenerarPagoTransferAsync(string token, int purchaseOrderNumber)
        {
            PaymentRequestModel paymentRequest = default;

            try
            {
                var httpClient = ClientHelper.GetClient(token);

                var response = await httpClient.PostAsync($"{Constants.WebApiUrl}/PurchaseOrder/{purchaseOrderNumber}/transfer", default);

                if (response.IsSuccessStatusCode)
                {
                    paymentRequest = await response.GetContentAsync<PaymentRequestModel>();
                }
            }
            catch (Exception ex)
            {
                // Do Nothing
                ex.ToString();
            }

            return paymentRequest;
        }

        /// <summary>
        /// Guarda el registro del log de los pagos realizados
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<PaymentRequestModel>> GuardarResultadoAsync(PaymentResultModel model)
        {
            try
            {
                // Usamos un token para acceder al API y enviar datos anonimos.
                var httpClient = ClientHelper.GetClient(Constants.ServiceToken);

                // Aumentamos el tiempo de espera, debido a que ejecuta un proceso que puede demorar mucho tiempo esperando la respuesta de GSV.
                httpClient.Timeout = new TimeSpan(0, 30, 0);

                var response = await httpClient.PutAsync($"{Constants.WebApiUrl}/Payment/{model.OperationNumber}/Update", model.ToContent());

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<OperationResult<PaymentRequestModel>>();
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<PaymentRequestModel>(ex);
            }

            return new OperationResult<PaymentRequestModel>(false, HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Guarda el comprobante de pago con tranferencia bancaria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<PaymentVoucherModel>> GuardarComprobanteAsync(string token, PaymentVoucherRequest model)
        {
            try
            {
                // Usamos un token para acceder al API y enviar datos anonimos.
                var httpClient = ClientHelper.GetClient(Constants.ServiceToken);
                
                var response = await httpClient.PostAsync($"{Constants.WebApiUrl}/Payment/Voucher", model.ToContent());

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<OperationResult<PaymentVoucherModel>> ();
                }

                return new OperationResult<PaymentVoucherModel>(false, HttpStatusCode.InternalServerError)
                {
                    UserMessage = "Hubo un error al procesar el registro",
                    DevMessage = "No se pudo eliminar el registro!"
                };

            }
            catch (Exception ex)
            {
                return new OperationResult<PaymentVoucherModel>(ex);
            }   
        }

        /// <summary>
        /// Verifica si hay una orden pendiente de suscripción
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<PurchaseSubscription>> BuscarPendienteAsync(string token, string ruc = null)
        {
            var solicitud = new OperationResult<PurchaseSubscription>(false);

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/Subscription/pending?ruc={ruc}");

                    if (response.IsSuccessStatusCode)
                    {
                        solicitud = await response.GetContentAsync<OperationResult<PurchaseSubscription>>();
                    }
                }

            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return solicitud;
        }

        /// <summary>
        /// Verifica si el plan es basic
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static async Task<OperationResult<SubscriptionModel>> ValidarTipoLicenciaAsync(string token, long id)
        {
            var solicitud = new OperationResult<SubscriptionModel>(false);

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/Subscription/{id}/LicenceType");

                    if (response.IsSuccessStatusCode)
                    {
                        var model = await response.GetContentAsync<SubscriptionModel>();
                        return await Task.FromResult(new OperationResult<SubscriptionModel>(true, HttpStatusCode.OK, model));
                    }
                }

            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return solicitud;
        }

        /// <summary>
        /// Guarda el registro del log de los pagos realizados
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<PaymentRequestModel>> ObtenerOrdenPagoAsync(string token, long paymnentId)
        {
            try
            {
                // Usamos un token para acceder al API y enviar datos anonimos.
                var httpClient = ClientHelper.GetClient(token);

                // Aumentamos el tiempo de espera, debido a que ejecuta un proceso que puede demorar mucho tiempo esperando la respuesta de GSV.
                httpClient.Timeout = new TimeSpan(0, 30, 0);

                var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/Payment/{paymnentId}/Purchase"); ;

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<OperationResult<PaymentRequestModel>>();
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<PaymentRequestModel>(ex);
            }

            return new OperationResult<PaymentRequestModel>(false, HttpStatusCode.InternalServerError);
        }


        /// <summary>
        /// Verifica si hay una orden pendiente de suscripción
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<SubscriptionRequest>> VerificarSubscripcionAsync(string token, long id )
        {
            var solicitud = new OperationResult<SubscriptionRequest>(false);

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/Subscription/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        solicitud = await response.GetContentAsync<OperationResult<SubscriptionRequest>>();
                    }
                }

            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return solicitud;
        }

        public static async Task<OperationResult<PurchaseSubscription>> GuardarOrdenSuscripcionAsync(string issuerToken, SubscriptionRequest model)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(issuerToken);
                {
                    var content = new StringContent(JsonConvert.SerializeObject(model));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await httpClient.PostAsync($"{Constants.WebApiUrl}/Subscription/purchaseOrder", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.GetContentAsync<OperationResult<PurchaseSubscription>>();
                        return new OperationResult<PurchaseSubscription>(true, HttpStatusCode.OK, result.Entity);
                    }
                    else
                    {
                        var result = await response.GetContentAsync<OperationResult>();
                        return new OperationResult<PurchaseSubscription>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = ex.ToString(),
                    UserMessage = ex.Message
                };
            }
        }

        public static async Task<OperationResult<ReferenceCodes>> ValidarCodigoPromocional(string token, string code)
        {
            var solicitud = new OperationResult<ReferenceCodes>(false) {UserMessage = "El código ingresado no es valido!" };

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/Payment/coupon/{code}");

                    if (response.IsSuccessStatusCode)
                    {
                        solicitud = await response.GetContentAsync<OperationResult<ReferenceCodes>>();
                    }
                }

            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return solicitud;
        }

        public static IssuerDto ObtenerEmisorPorRuc(string token, string issuerRuc)
        {
            var httpClient = ClientHelper.GetClient(token);
            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/payment/{issuerRuc}/issuers").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.GetContent<IssuerDto>();
            }

            return default;
        }

        /// <summary>
        /// Guarda el registro del log de los pagos realizados
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<PaymentRequestModel>> GuardarPaymentResultadoAsync(PaymentTransaction model)
        {
            try
            {
                // Usamos un token para acceder al API y enviar datos anonimos.
                var httpClient = ClientHelper.GetClient(Constants.ServiceToken);

                // Aumentamos el tiempo de espera, debido a que ejecuta un proceso que puede demorar mucho tiempo esperando la respuesta de GSV.
                httpClient.Timeout = new TimeSpan(0, 30, 0);

                var response = await httpClient.PutAsync($"{Constants.WebApiUrl}/Payment/Transaction", model.ToContent());

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<OperationResult<PaymentRequestModel>>();
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<PaymentRequestModel>(ex);
            }

            return new OperationResult<PaymentRequestModel>(false, HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Verifica si hay una orden pendiente de suscripción
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<OperationResult<PurchaseSubscription>> ValidarPagoSubscripcionAsync(string token, string ruc = null)
        {
            var solicitud = new OperationResult<PurchaseSubscription>(false);

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/Subscription/orderpayment?ruc={ruc}");

                    if (response.IsSuccessStatusCode)
                    {
                        solicitud = await response.GetContentAsync<OperationResult<PurchaseSubscription>>();
                    }
                }

            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return solicitud;
        }

        public async static Task<OperationResult<PaymentRequestModel>> ValidarPagoOrdenAsync(string token, long purchaseOrderId)
        {
            var  paymentRequest = new OperationResult<PaymentRequestModel>(false);

            try
            {
                var httpClient = ClientHelper.GetClient(token);

                var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/payment/{purchaseOrderId}/validate-payment");

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<OperationResult<PaymentRequestModel>>();
                }
            }
            catch (Exception ex)
            {               
                ex.ToString();
            }

            return paymentRequest;
        }

    }
}
