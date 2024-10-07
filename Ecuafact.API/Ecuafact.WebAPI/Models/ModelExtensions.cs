using AutoMapper;
using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Dal.Services;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Entities.VPOS2;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.Models.Authentication;
using Ecuafact.WebAPI.Models.Dtos;
using Ecuafact.WebAPI.PayMe;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Linq;

namespace Ecuafact.WebAPI
{
    /// <summary>
    /// ModelExtensions
    /// </summary>
    public static class ModelExtensions
    {
        internal static async Task<OperationResult<ElectronicSign>> Process(this IPurchaseOrderService service, ElectronicSign esign)
        {
            try
            {
                // Antes de enviar a procesar esta firma electronica valido si ya fue procesada.
                if (esign?.RequestResult?.Contains("recibida correctamente") ?? false)
                {
                    var message = "La firma electronica ya fue procesada";
                    Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.ERROR", message, esign);
                    return await Task.FromResult(esign.ToResult(false, HttpStatusCode.Conflict, message));
                }

                return await service.UanatacaProcess(esign);
            }
            catch (Exception ex)
            {               
                Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.ERROR", ex.Message, esign);
                return await Task.FromResult(esign.ToResult(false, HttpStatusCode.Conflict, ex.Message));
            } 
        }

        internal static async Task<OperationResult<ElectronicSign>> GSVProcess(this IPurchaseOrderService service, ElectronicSign esign)
        {
            try
            {
                var request = esign.ToRequestModel();
                using (var client = new HttpClient())
                {
                    // Esperamos 15 minutos como maximo:
                    client.Timeout = new TimeSpan(0, 15, 0);

                    var jsonReq = JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonReq);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.PostAsync(Constants.ElectronicSign.ProcessorURL, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.GetContentAsync<ElectronicSignServiceResult>();
                        if (data?.result == true)
                        {
                            esign.Status = ElectronicSignStatusEnum.Approved;
                            esign.RequestDate = DateTime.Now;
                            esign.RequestResult = data.message;

                            Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.OK", "Procesamiento de Firma Electronica:", request, "RESULTADO:", data);
                            return await Task.FromResult(service.UpdateElectronicSign(esign));
                        }
                    }

                    throw new Exception($"Error al Procesar la Firma Electronica: {await response.Content?.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.ERROR", ex, "REQUEST: ", esign);
                esign.Status = ElectronicSignStatusEnum.Error;
                esign.RequestResult = $"{DateTime.Now} - {ex.Message} {Environment.NewLine}{esign.RequestResult}";
                return await Task.FromResult(service.UpdateElectronicSign(esign));
            }
        }

        internal static async Task<OperationResult<ElectronicSign>> UanatacaProcess(this IPurchaseOrderService service, ElectronicSign esign)
        {
            try
            {
                var request = esign.ToRequestModelUanataca();
                using (var client = new HttpClient())
                {
                    // Esperamos 15 minutos como maximo:
                    client.Timeout = new TimeSpan(0, 15, 0);

                    var jsonReq = JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonReq);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await client.PostAsync(Constants.ElectronicSign.ProcessorURL, content);
                    var data = await response.GetContentAsync<ElectronicSignServiceResult>();

                    if (data?.result == true)
                    {
                        esign.Status = ElectronicSignStatusEnum.Approved;
                        esign.RequestDate = DateTime.Now;
                        esign.RequestResult = data.message;   

                        Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.OK", "Procesamiento de Firma Electronica:", request, "RESULTADO:", data);
                        return await Task.FromResult(service.UpdateElectronicSign(esign));
                    }

                    throw new Exception($"Error al Procesar la Firma Electronica: {await response.Content?.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.ERROR", ex, "REQUEST: ", esign);
                esign.Status = ElectronicSignStatusEnum.Error; 
                esign.RequestResult = $"Error al Procesar la Firma Electronica: {DateTime.Now}-{ex.Message}"; //, {ex.InnerException.Message}
                return await Task.FromResult(service.UpdateElectronicSign(esign));
            }
        }

        internal static OperationResult<ElectronicSign> ElecsignProcess(this IPurchaseOrderService service, ElectronicSign esign)
        {
            try
            {
                // Antes de enviar a procesar esta firma electronica valido si ya fue procesada.
                if (esign?.RequestResult?.Contains("recibido correctamente") ?? false)
                {
                    var message = "La firma electronica ya fue procesada";
                    Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.ERROR", message, esign);
                    return esign.ToResult(false, HttpStatusCode.Conflict, message);
                }
                return service.ElecsignUanatacaProcess(esign);
            }
            catch (Exception ex)
            {
                Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.ERROR", ex, "REQUEST: ", esign);
                return esign.ToResult(false, HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        internal static OperationResult<ElectronicSign> ElecsignUanatacaProcess(this IPurchaseOrderService service, ElectronicSign esign)
        {
            var msj = string.Empty;
            try
            {
                var request = esign.ToRequestModelUanataca();

                using (var client = new HttpClient())
                {
                    // Esperamos 15 minutos como maximo:
                    client.Timeout = new TimeSpan(0, 15, 0);
                    var jsonReq = JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonReq);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = client.PostAsync(Constants.ElectronicSign.ProcessorURL, content).Result;
                    var data =  response.GetContentAsync<ElectronicSignServiceResult>().Result;

                    if (data.result)
                    {
                        esign.Status = ElectronicSignStatusEnum.Approved;
                        esign.RequestDate = DateTime.Now;
                        esign.RequestResult = data.message;
                        esign.RequestNumber = data.token;
                        Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.OK", "Procesamiento de Firma Electronica:", request, "RESULTADO:", data);
                        return service.UpdateElectronicSign(esign);
                    }
                    else 
                    {
                        msj = $"{DateTime.Now}-{data.message}";
                        esign.RequestResult = $"Error al Procesar la Firma Electronica: {DateTime.Now}-{data.message}";
                        throw new Exception(esign.RequestResult);
                    }                   
                    throw new Exception($"Error al Procesar la Firma Electronica: {response.Content?.ReadAsStringAsync().Result}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.ERROR", ex, "REQUEST: ", esign);
                esign.Status = ElectronicSignStatusEnum.Error;
                esign.RequestResult = $"{esign.RequestResult ?? $"Error al Procesar la Firma Electronica: {DateTime.Now}"}, {ex?.Message}, {ex?.InnerException?.Message}";
                esign.StatusMsg = esign.RequestResult;
                return service.UpdateElectronicSign(esign);
            }
        }

        internal static OperationResult<ElectronicSign> ElecsignGVSProcess(this IPurchaseOrderService service, ElectronicSign esign)
        {
            try
            {               
                var request = esign.ToRequestModel();

                using (var client = new HttpClient())
                {
                    // Esperamos 15 minutos como maximo:
                    client.Timeout = new TimeSpan(0, 15, 0);
                    var jsonReq = JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonReq);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = client.PostAsync(Constants.ElectronicSign.ProcessorURL, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var data = response.GetContentAsync<ElectronicSignServiceResult>().Result;
                        if (data?.result == true)
                        {
                            esign.Status = ElectronicSignStatusEnum.Approved;
                            esign.RequestDate = DateTime.Now;
                            esign.RequestResult = data.message;
                            Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.OK", "Procesamiento de Firma Electronica:", request, "RESULTADO:", data);
                            return service.UpdateElectronicSign(esign);
                        }
                    }

                    throw new Exception($"Error al Procesar la Firma Electronica: {response.Content?.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.PROCESS.ERROR", ex, "REQUEST: ", esign);

                esign.Status = ElectronicSignStatusEnum.Error;
                esign.RequestResult = $"{DateTime.Now} - {ex.Message} {Environment.NewLine}{esign.RequestResult}";

                return service.UpdateElectronicSign(esign);
            }
        }

        internal static async Task<OperationResult<ElectronicSign>> GenerateInvoice(this IPurchaseOrderService service, ElectronicSign esign)
        {
            if (esign.InvoiceId > 0)
            {
                var message = "La Firma electronica especificada ya fue facturada";
                Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.INVOICE.ERROR", message, esign);
                return await Task.FromResult(esign.ToResult(false, HttpStatusCode.Conflict, message));
            }

            if (esign.PurchaseOrder == null)
            {
                esign.PurchaseOrder = service.GetPurchaseOrderById(esign.PurchaseOrderId)?.Entity;
            }

            var invoice = await esign?.PurchaseOrder?.GenerateInvoice();

            if (invoice.IsSuccess)
            {
                esign.InvoiceResult = invoice.Entity.Reason ?? invoice.UserMessage ?? "Documento Generado Exitosamente";
                esign.InvoiceDate = invoice?.Entity?.IssuedOn;
                esign.InvoiceId = invoice?.Entity?.Id;
                esign.InvoiceNumber = invoice?.Entity?.DocumentNumber;                
                esign.PurchaseOrder.InvoiceDate = invoice?.Entity?.IssuedOn;
                esign.PurchaseOrder.InvoiceNumber = invoice?.Entity?.DocumentNumber;
                esign.ActivateSubscription = true;
                esign.PurchaseOrder.Status = PurchaseOrderStatusEnum.Payed;
            }
            else
            {
                esign.InvoiceResult = $"{DateTime.Now} - No se generó la factura para esta solicitud \r\n{invoice.UserMessage ?? esign.InvoiceResult}.";
            }

            var result = service.UpdateElectronicSign(esign);

            return await Task.FromResult(result);

        }

        internal static async Task<OperationResult<PurchaseSubscription>> GenerateInvoice(this ISubscriptionService service, PurchaseSubscription purchaseSubs, string typeEmail = "", List<AdditionalFieldModel>  additionalField = null)
        {
            var enviarEmail = false;
            var _purchaseSubs = await Task.FromResult(service.GetIPurchaseSubscription(purchaseSubs.Id));
            if(_purchaseSubs.IsSuccess)
            {
                if(_purchaseSubs.Entity.InvoiceId > 0)
                {
                    var message = "La suscripción anual especificada ya fue facturada";
                    Logger.Log($"SUBSCRIPTION.{purchaseSubs?.Subscription?.RUC}.{DateTime.Now.ToFileTime()}.INVOICE.ERROR", message, purchaseSubs);
                    return await Task.FromResult(purchaseSubs.ToResult(false, HttpStatusCode.Conflict, message));
                }
            }
            else
            {
                if (purchaseSubs.InvoiceId > 0)
                {
                    var message = "La suscripción anual especificada ya fue facturada";
                    Logger.Log($"SUBSCRIPTION.{purchaseSubs?.Subscription?.RUC}.{DateTime.Now.ToFileTime()}.INVOICE.ERROR", message, purchaseSubs);
                    return await Task.FromResult(purchaseSubs.ToResult(false, HttpStatusCode.Conflict, message));
                }
            }

            if (_purchaseSubs.Entity.InvoicePrrocessed)
            {
                var message = "La suscripción anual especificada ya fue facturada";
                Logger.Log($"SUBSCRIPTION.{purchaseSubs?.Subscription?.RUC}.{DateTime.Now.ToFileTime()}.INVOICE.ERROR", message, purchaseSubs);
                return await Task.FromResult(purchaseSubs.ToResult(false, HttpStatusCode.Conflict, message));
            }
            else
            {
               var _result = await Task.FromResult(service.PurchaseSubscriptionInvoiceProcessed(_purchaseSubs.Entity.Id));
                if(_result.IsSuccess)
                {
                    _purchaseSubs.Entity.InvoicePrrocessed = _result.Entity.InvoicePrrocessed;
                }
            }
            if (purchaseSubs.PurchaseOrder == null)
            {
               var _purchaseOrder = await Task.FromResult(service.GetPurchaseOrderById(purchaseSubs.PurchaseOrderId));
                purchaseSubs.PurchaseOrder = _purchaseOrder.Entity;
            }
            var invoice = await purchaseSubs?.PurchaseOrder?.GenerateInvoice(additionalField);
            if (invoice.IsSuccess)
            {
                var plan = await Task.FromResult(service.GetLicenceTypeById(purchaseSubs.LicenceTypeId.Value));
                purchaseSubs.InvoiceResult = invoice.Entity?.Reason ;
                purchaseSubs.InvoiceDate = invoice?.Entity?.IssuedOn;
                purchaseSubs.InvoiceId = invoice?.Entity?.Id;
                purchaseSubs.InvoiceNumber = invoice?.Entity?.DocumentNumber;
                purchaseSubs.LastModifiedOn = DateTime.Now;              
                purchaseSubs.Status = PurchaseOrderSubscriptionStatusEnum.Payed;
                purchaseSubs.PaymentType = purchaseSubs.PaymentType ?? PaymentTypeEnum.BankTransfer;               
                purchaseSubs.RequestElectronicSign = plan.IncludeCertificate ? RequestElectronicSignEnum.Pending: RequestElectronicSignEnum.NotInclude;
                purchaseSubs.RequestElectronicSignMsg = plan.IncludeCertificate ?  RequestElectronicSignEnum.Pending.GetDisplayValue(): RequestElectronicSignEnum.NotInclude.GetDisplayValue();

                //actualizar la orden
                purchaseSubs.PurchaseOrder.InvoiceDate = invoice?.Entity?.IssuedOn;
                purchaseSubs.PurchaseOrder.InvoiceNumber = invoice?.Entity?.DocumentNumber;
                purchaseSubs.PurchaseOrder.LastModifiedOn = DateTime.Now;
                purchaseSubs.PurchaseOrder.Status =PurchaseOrderStatusEnum.Payed;

                //actualizar la suscripción
                var expirationDate = DateTime.Now.AddYears(1);
                var _subscription = await Task.FromResult(service.GetSubscription(purchaseSubs.SubscriptionId));
                if (_subscription.Status == SubscriptionStatusEnum.Activa && _subscription.SubscriptionExpirationDate > DateTime.Now)
                {
                    TimeSpan totalDias =  _subscription.SubscriptionExpirationDate.Value - DateTime.Now;
                    var dias = (int)totalDias.TotalDays;
                    if (dias > 0)
                    {
                        expirationDate = expirationDate.AddDays(dias + 1);
                    }
                }
                purchaseSubs.SubscriptionLog = new SubscriptionLog{
                    RUC = _subscription.RUC,
                    IssuerId = _subscription.IssuerId,
                    SubscriptionId = _subscription.Id,
                    LicenceTypeId = _subscription.LicenceTypeId,
                    SubscriptionStartDate = _subscription.SubscriptionStartDate ?? DateTime.Now,
                    SubscriptionExpirationDate = _subscription.SubscriptionExpirationDate ?? expirationDate,
                    IssuedDocument = _subscription.IssuedDocument,
                    BalanceDocument = _subscription.BalanceDocument,
                    Observation = _subscription.StatusMsg,
                    Status = _subscription.Status,
                };               
                 
                _subscription.Status = SubscriptionStatusEnum.Activa;
                _subscription.SubscriptionStartDate = DateTime.Now;
                _subscription.SubscriptionExpirationDate = expirationDate;
                _subscription.LastModifiedOn = DateTime.Now;
                _subscription.StatusMsg ="Ok";
                _subscription.LicenceTypeId = plan.Id;
                _subscription.AmountDocument = plan.AmountDocument;
                _subscription.IssuedDocument = null;
                _subscription.BalanceDocument = null;
                purchaseSubs.Subscription = _subscription;
                enviarEmail = true;                
            }
            else
            {
                purchaseSubs.InvoiceResult = $"{DateTime.Now} - No se generó la factura para esta solicitud \r\n{invoice.UserMessage ?? purchaseSubs.InvoiceResult}.";
            }

            var result = await Task.FromResult(service.UpdatePurchaseSubscription(purchaseSubs));
            if (enviarEmail) {
                var valor = purchaseSubs?.UserPaymentId > 0? "A":"";
                if (!string.IsNullOrWhiteSpace(typeEmail))
                { valor = "M"; }
                purchaseSubs.Subscription.LicenceType = new LicenceType { Name = purchaseSubs.PurchaseOrder.Products };
                purchaseSubs.Subscription.Issuer.EmailActivationAccount(purchaseSubs.Subscription, 0, valor);
            }

            return await Task.FromResult(result);

        }

        internal static  OperationResult<ElectronicSign> GenerateInvoices(this IPurchaseOrderService service, ElectronicSign esign)
        {
            if (esign.InvoiceId > 0)
            {
                var message = "La Firma electronica especificada ya fue facturada";
                Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.INVOICE.ERROR", message, esign);
                return esign.ToResult(false, HttpStatusCode.Conflict, message);
            }

            if (esign.PurchaseOrder == null)
            {
                esign.PurchaseOrder = service.GetPurchaseOrderById(esign.PurchaseOrderId)?.Entity;
            }

            var invoice =  esign?.PurchaseOrder?.GenerateInvoices();

            if (invoice.IsSuccess)
            {
                esign.InvoiceResult = invoice.Entity.Reason ?? invoice.UserMessage ?? "Documento Generado Exitosamente";
                esign.InvoiceDate = invoice?.Entity?.IssuedOn;
                esign.InvoiceId = invoice?.Entity?.Id;
                esign.InvoiceNumber = invoice?.Entity?.DocumentNumber;
                esign.PurchaseOrder.InvoiceDate = invoice?.Entity?.IssuedOn;
                esign.PurchaseOrder.InvoiceNumber = invoice?.Entity?.DocumentNumber;
                esign.PurchaseOrder.Status = invoice?.Entity?.Id > 0 ? PurchaseOrderStatusEnum.Payed: PurchaseOrderStatusEnum.Saved;
                esign.ActivateSubscription = true;
            }
            else
            {
                esign.InvoiceResult = $"{DateTime.Now} - No se generó la factura para esta solicitud \r\n{invoice.UserMessage ?? esign.InvoiceResult}.";
            }

            var result = service.UpdateElectronicSign(esign);

            return result;

        }

        private static async Task<OperationResult<InvoiceDto>> GenerateInvoice(this PurchaseOrder purchaseOrder, List<AdditionalFieldModel> additionalField = null)
        { 
            var request = purchaseOrder?.ToInvoiceRequest(additionalField);
            var product = purchaseOrder.Products.Contains("Firma Electrónica") ? "ELECTRONICSIGN" : "SUBSCRIPTION";
            try
            {
                if (request == null)
                {
                    throw new Exception("Hubo un error al facturar la orden de compra especificada.");
                }
                using (var client = new HttpClient())
                {
                    client.SetAuthentication(Constants.NexusToken);
                    var response = await client.PostAsync($"{Constants.NexusApiUrl}/Invoice", request, new JsonMediaTypeFormatter());
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.GetContentAsync<InvoiceDto>();
                        Logger.Log($"{product}.{purchaseOrder?.Identification}.{DateTime.Now.ToFileTime()}.INVOICE.OK", "FACTURA GENERADA", result);
                        return new OperationResult<InvoiceDto>(true, System.Net.HttpStatusCode.OK, result);
                    }
                    
                    throw new HttpResponseException(response);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"{product}.{purchaseOrder?.Identification}.INVOICE.ERROR", $"Hubo un error al generar la factura", ex, "FACTURA:", request, "Orden de Compra:", purchaseOrder);
                return new OperationResult<InvoiceDto>(false, HttpStatusCode.InternalServerError, $"Hubo un error al generar la factura") { DevMessage = ex.ToString() };
            }

        }

        private static OperationResult<InvoiceDto> GenerateInvoices(this PurchaseOrder purchaseOrder)
        {
            var request = purchaseOrder?.ToInvoiceRequest();

            try
            {
                if (request == null)
                {
                    throw new Exception("Hubo un error al facturar la orden de compra especificada.");
                }

                using (var client = new HttpClient())
                {
                    client.SetAuthentication(Constants.NexusToken);

                    var response = client.PostAsync($"{Constants.NexusApiUrl}/Invoice", request, new JsonMediaTypeFormatter()).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.GetContentAsync<InvoiceDto>().Result;

                        Logger.Log($"ELECTRONICSIGN.{purchaseOrder?.Identification}.INVOICE.OK", "FACTURA GENERADA", result);
                        return new OperationResult<InvoiceDto>(true, System.Net.HttpStatusCode.OK, result);
                    }

                    throw new HttpResponseException(response);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"ELECTRONICSIGN.{purchaseOrder?.Identification}.INVOICE.ERROR", $"Hubo un error al generar la factura", ex, "FACTURA:", request, "Orden de Compra:", purchaseOrder);
                return new OperationResult<InvoiceDto>(false, HttpStatusCode.InternalServerError, $"Hubo un error al generar la factura") { DevMessage = ex.ToString() };
            }

        }

        internal static async Task<OperationResult<ElectronicSign>> StatusProcess(this IPurchaseOrderService service, ElectronicSign esign)
        {
            var  statusProcces = new Solicitudes();
            try
            {
                var request = esign.ToRequestModelStatusProcess();
                
                using (var client = new HttpClient())
                {
                    // Esperamos 15 minutos como maximo:
                    client.Timeout = new TimeSpan(0, 15, 0);
                    var jsonReq = JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonReq);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.PostAsync(Constants.ElectronicSign.ProcessStatusUrl, content);
                    var data = await response.GetContentAsync<ElectSignStatusPorcessServiceResult>();

                    if (data.result)
                    {
                        var solicitud = data.data.solicitudes.Count > 1 ? data.data.solicitudes[data.data.solicitudes.Count - 1] : data.data.solicitudes.First();
                        if (solicitud.estado.Contains("ACTUALIZACION SOLICITADA"))
                        {
                            esign.Status = ElectronicSignStatusEnum.Error;
                            esign.StatusMsg = $"{solicitud.estado}: {solicitud.observacion ?? ""}";
                        }
                        else if (solicitud.estado.Contains("RECHAZADO") || solicitud.estado.Contains("ELIMINADO"))
                        {
                            esign.Status = ElectronicSignStatusEnum.Error;
                            esign.StatusMsg = $"{solicitud.estado}: {solicitud.observacion ?? ""}";
                            esign.RequestResult = $"Error al Procesar la Firma Electronica:{solicitud.estado}-{solicitud.observacion ?? ""}";
                        }
                        else if (solicitud.estado.Contains("NUEVO") || solicitud.estado.Contains("ASIGNADO") || solicitud.estado.Contains("EN VALIDACION"))
                        {
                            esign.Status = ElectronicSignStatusEnum.Approved;
                            esign.StatusMsg = $"{solicitud.estado}";
                            esign.ReceivedDate = DateTime.Now;
                        }
                        else if (solicitud.estado.Contains("APROBADO") || solicitud.estado.Contains("ENTREGADO") || solicitud.estado.Contains("EN ESPERA") || solicitud.estado.Contains("EMITIDO (VALIDO)"))
                        {
                            esign.Status = ElectronicSignStatusEnum.Processed;
                            esign.StatusMsg = $"{solicitud.estado}";
                            esign.ReceivedDate = DateTime.Now;
                        }


                        esign.LastModifiedOn = DateTime.Now;
                        service.UpdateElectronicSign(esign);
                    }

                }

                return await Task.FromResult(esign.ToResult(true, HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.STATUSPROCESS.ERROR", ex.Message, esign);
                return await Task.FromResult(esign.ToResult(false, HttpStatusCode.Conflict, ex.Message));
            }
        }

        internal static async Task<OperationResult<List<ElectronicSign>>> StatusProcessSolicituds(this IPurchaseOrderService service)
        {            
            try
            {
                var esign = service.GetElectronicSignByApproved();
                if (!esign.IsSuccess)
                {
                    return await Task.FromResult(esign.Entity.ToResult(false, HttpStatusCode.NoContent));
                }

                foreach (var e in esign.Entity)
                {
                    await service.StatusProcess(e);
                }
                return await Task.FromResult(esign.Entity.ToResult(true, HttpStatusCode.OK));

            }
            catch (Exception ex)
            {                
                return await Task.FromResult(new OperationResult<List<ElectronicSign>>(false, HttpStatusCode.Conflict, ex.Message));
            }
        }

        internal static async Task<OperationResult<ElectronicSign>> StatusSolicitud(this IPurchaseOrderService service, ElectronicSign esign)
        {            
            try
            {
                var request = esign.ToRequestModelStatusProcess();
                if (esign?.RequestResult?.Contains("Error al Procesar la Firma Electronica") ?? false)
                {                    
                    return await Task.FromResult(esign.ToResult(true, HttpStatusCode.OK));
                }

                using (var client = new HttpClient())
                {
                    // Esperamos 15 minutos como maximo:
                    client.Timeout = new TimeSpan(0, 15, 0);
                    var jsonReq = JsonConvert.SerializeObject(request);
                    var content = new StringContent(jsonReq);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.PostAsync(Constants.ElectronicSign.ProcessStatusUrl, content);
                    var data = await response.GetContentAsync<ElectSignStatusPorcessServiceResult>();

                    if (data.result)
                    {
                        var solicitud = data.data.solicitudes.Count > 1 ? data.data.solicitudes[data.data.solicitudes.Count -1] : data.data.solicitudes.First();
                        if (!string.IsNullOrEmpty(solicitud.observacion) && solicitud.estado.Contains("ACTUALIZACION SOLICITADA"))
                        {
                            esign.Status = ElectronicSignStatusEnum.Error;
                            esign.StatusMsg = $"{solicitud.estado}: {solicitud.observacion}";
                        }                        
                        else if(solicitud.estado.Contains("APROBADO") || solicitud.estado.Contains("ENTREGADO") || solicitud.estado.Contains("EN ESPERA") || solicitud.estado.Contains("EMITIDO (VALIDO)"))
                        {
                            esign.Status = ElectronicSignStatusEnum.Processed;
                            esign.StatusMsg = $"{solicitud.estado}";
                            esign.ReceivedDate = DateTime.Now;
                        }
                        else {
                            esign.StatusMsg = $"{solicitud.estado}";
                        }
                        esign.LastModifiedOn = DateTime.Now;
                        service.UpdateElectronicSign(esign);                        
                    }

                    return await Task.FromResult(esign.ToResult(true, HttpStatusCode.OK));
                  
                }

            }
            catch (Exception ex)
            {
                Logger.Log($"ELECTRONICSIGN.{esign?.Identification}.STATUSPROCESS.ERROR", ex.Message, esign);
                return await Task.FromResult(esign.ToResult(false, HttpStatusCode.Conflict, ex.Message));
            }
        }

        internal static ElectronicSignServiceRequest ToRequestModel(this ElectronicSign esign)
        {
            return new ElectronicSignServiceRequest
            {
                apikey = Constants.ElectronicSign.ApiKey,
                uid = Constants.ElectronicSign.UID,
                tipo_firma = esign.SignType,
                tipodocumento = esign.DocumentType.GetDisplayValue()?.ToLower() ?? "cedula",
                numerodocumento = esign.Identification,
                nombre = esign.FirstName,
                apellido = esign.LastName,
                telefono = esign.Phone,
                correo = esign.Email,
                direccion = esign.Address,
                ciudad = esign.City,
                provincia = esign.Province,

                ruc_empresa = esign.RUC, 
                razon_social = esign.BusinessName,
                direccion_ruc = esign.Address,
                cobro_directo = 1, // El cobro lo realiza directamente ecuanexus.

                id_vigenciafirma = SignatureValidyEnum.TwoYears,
                id_tipoverificacion = esign.VerificationType,
                id_formato = esign.FileFormat,
                id_direccion_skype = esign.Skype,
                
                id_copiacedula = GetFile(esign.IdentificationCardFile, esign.Identification),
                id_copiapapvotacion = GetFile(esign.ElectionsTicketFile, esign.Identification),
                id_copiaruc = GetFile(esign.RucFile, esign.Identification),
                id_autreprelegal = GetFile(esign.AuthorizationLegalRepFile, esign.Identification),
                id_nombramiento = GetFile(esign.DesignationFile, esign.Identification),
                id_const = GetFile(esign.ConstitutionFile, esign.Identification)
            };
        }

        internal static ElectronicSignUanatacaServiceRequest ToRequestModelUanataca(this ElectronicSign esign)
        {
            var apellidos = esign.LastName.Split(' ');            
            
            return new ElectronicSignUanatacaServiceRequest
            {
                apikey = Constants.ElectronicSign.ApiKey,
                uid = Constants.ElectronicSign.UID,
                tipo_solicitud = esign.SignType,
                contenedor = esign.FileFormat,
                nombres = esign.FirstName,
                apellido1 = (apellidos.Length == 1 ? esign.LastName : apellidos[0]),
                apellido2 = (apellidos.Length > 1 ? apellidos[1] : ""),
                tipodocumento = (esign.DocumentType == IdentificationTypeEnum.Passport ? "PASAPORTE" : "CEDULA"),
                numerodocumento = (esign.DocumentType == IdentificationTypeEnum.Passport ? esign.Identification : int.TryParse(esign.Identification, out int num) ? Convert.ToInt64(esign.Identification).ToString("0000000000") : esign.Identification.Substring(0, 10)),
                coddactilar = esign.FingerPrintCode ?? "",
                ruc_personal = (esign.SignType == RucTypeEnum.Natural ? esign.RUC :""),
                sexo = (esign.Sexo == SexoEnum.Masculino ? SexoEnum.Masculino.GetDisplayValue(): SexoEnum.Femenino.GetDisplayValue()),
                fecha_nacimiento = MySystemExtensions.ToDateTimeString2(esign.BirthDate),
                nacionalidad = "ECUATORIANA",
                telfCelular = esign.Phone,
                telfCelular2 = esign.Phone2,
                telfFijo = "",
                eMail = esign.Email,
                eMail2 = esign.Email2,
                provincia = esign.Province,
                ciudad = esign.City,
                direccion = esign.Address.Length > 100 ? esign.Address.Substring(0,90).Trim(): esign.Address.Trim(),
                empresa = (esign.SignType == RucTypeEnum.Juridical ? esign.BusinessName : ""),
                ruc_empresa = (esign.SignType == RucTypeEnum.Juridical  ? esign.RUC :""),                
                direccion_ruc = (esign.SignType == RucTypeEnum.Juridical ? esign.Address.Length > 100 ? esign.Address.Substring(0, 90).Trim() : esign.Address.Trim() : ""),
                vigenciafirma = $"{(int)esign.SignatureValidy} año{(esign.SignatureValidy == SignatureValidyEnum.OneYear ? "":"s")}",
                cargo = esign.WorkPosition ?? "",
                f_cedulaFront = GetFiles(esign.CedulaFrontFile, esign.Identification),
                f_cedulaBack = GetFiles(esign.CedulaBackFile, esign.Identification),
                f_selfie = GetFiles(esign.SelfieFile, esign.Identification),
                f_copiaruc = GetFiles(esign.RucFile, esign.Identification),
                f_nombramiento = GetFiles(esign.DesignationFile, esign.Identification),
                f_constitucion = GetFiles(esign.ConstitutionFile, esign.Identification),
                f_adicional1 = esign.AuthorizationAgeFile != null ? GetFiles(esign.AuthorizationAgeFile, esign.Identification):string.Empty,
                f_adicional2 = esign.AuthorizationFile != null ? GetFiles(esign.AuthorizationFile, esign.Identification) : string.Empty
            };
        }

        internal static ElectSignStatusProcessServiceRequest ToRequestModelStatusProcess(this ElectronicSign esign)
        {

            return new ElectSignStatusProcessServiceRequest
            {
                apikey = Constants.ElectronicSign.ApiKey,
                uid = Constants.ElectronicSign.UID,
                documento = esign.SignType == RucTypeEnum.Natural ? (esign.DocumentType == IdentificationTypeEnum.Passport ? esign.Identification: esign.Identification.Substring(0, 10)) : esign.RUC,
                tipo_solicitud = esign.SignType == RucTypeEnum.Juridical ? "REPRESENTANTE LEGAL" : "PERSONA NATURAL"               
            };
        }

        internal static InvoiceRequestModel ToInvoiceRequest(this PurchaseOrder purchaseOrder, List<AdditionalFieldModel> additionalField = null)
        {
            decimal discount = 0;
            decimal totalDiscount = 0;
            decimal unitPrice = purchaseOrder.Subtotal;
            if (purchaseOrder.Discount > 0)
            {
                discount = purchaseOrder.Discount;
                totalDiscount = purchaseOrder.TotalDiscount;
                unitPrice = purchaseOrder.Subtotal + totalDiscount;
            }

            if (additionalField?.Count > 0)
            {
                additionalField.Add(new AdditionalFieldModel { LineNumber = 3, Name = "Orden de Compra #", Value = purchaseOrder.PurchaseOrderId.ToString("D10") });
                additionalField.Add(new AdditionalFieldModel { LineNumber = 4, Name = "Direccion", Value = purchaseOrder.Address });
                additionalField.Add(new AdditionalFieldModel { LineNumber = 5, Name = "Email", Value = purchaseOrder.Email });
                additionalField.Add(new AdditionalFieldModel { LineNumber = 6, Name = "Plan", Value = purchaseOrder.Products });
                additionalField.Add(new AdditionalFieldModel { LineNumber = 7, Name = "Observaciones", Value = "Comprobante correspondiente al Servicio de Facturación Electrónica." });
                if (discount > 0)
                {
                    additionalField.Add(new AdditionalFieldModel { LineNumber = 8, Name = "Descuento", Value = $"Se aplica el {(purchaseOrder.Discount / 100):0.#%} de descuento, por {purchaseOrder?.ReferenceCodes?.Description.Trim() ?? $"Código {purchaseOrder.DiscountCode.Trim()}"}" });
                }
            }
            else {

                additionalField = new List<AdditionalFieldModel>{
                    new AdditionalFieldModel{ LineNumber=1, Name="Orden de Compra #", Value=purchaseOrder.PurchaseOrderId.ToString("D10") },
                    new AdditionalFieldModel{ LineNumber=2, Name="Direccion", Value=purchaseOrder.Address },
                    new AdditionalFieldModel{ LineNumber=3, Name="Email", Value=purchaseOrder.Email },
                    new AdditionalFieldModel{ LineNumber=4, Name="Plan", Value=purchaseOrder.Products },
                    new AdditionalFieldModel{ LineNumber=5, Name="Observaciones", Value="Comprobante correspondiente al Servicio de Facturación Electrónica." },
                };
                if (discount > 0) {
                    additionalField.Add(new AdditionalFieldModel { LineNumber = 6, Name = "Descuento", Value = $"Se aplica el {(purchaseOrder.Discount / 100):0.#%} de descuento, por {purchaseOrder?.ReferenceCodes?.Description.Trim() ?? $"Código {purchaseOrder.DiscountCode.Trim()}"}" });
                }
            }

            return new InvoiceRequestModel
            {
                Identification = purchaseOrder.Identification,
                IdentificationType = purchaseOrder.Identification.Length == 13 ? "04" :
                                        purchaseOrder.Identification.Length == 10 ? "05" : "06",
                ContributorName = purchaseOrder.BusinessName ?? $"{purchaseOrder.FirstName} {purchaseOrder.LastName}",
                Currency = "DOLAR",
                Address = purchaseOrder.Address,
                EmailAddresses = $"{purchaseOrder.Email}; contabilidad@ecuanexus.com; finanzas@ecuanexus.com",
                IssuedOn = DateTime.Today.ToString("dd/MM/yyyy"),
                Phone = purchaseOrder.Phone,
                Reason = $"Comprobante correspondiente al Servicio de Facturacion Electronica. ORDEN DE COMPRA # {purchaseOrder.PurchaseOrderId:D10}, Incluye {purchaseOrder.Products}",
                Status = NewDocumentStatusEnum.Issued,
                ValueAddedTax = purchaseOrder.IVA,
                Subtotal = purchaseOrder.Subtotal,
                SubtotalVat = purchaseOrder.Subtotal12,
                SubtotalVatZero = purchaseOrder.Subtotal0,
                Tip = purchaseOrder.Additional,
                Total = purchaseOrder.Total,
                TotalDiscount = totalDiscount,
                Details = new List<InvoiceDetailModel>
                    {
                        new InvoiceDetailModel
                        {
                            MainCode="26",
                            Description = "SERVICIOS ECUAFACT APP",
                            Name1 = "Servicio:",
                            Value1 = "Fact. Electronica",
                            Name2 = "Incluye:",
                            Value2 = purchaseOrder.Products,
                            Amount = 1,
                            UnitPrice = unitPrice,
                            SubTotal = purchaseOrder.Subtotal,
                            Discount = discount,
                            Taxes = new List<TaxModel>
                            {
                                new TaxModel {
                                    Code="2", 
                                    PercentageCode = "4", 
                                    Rate=15,
                                    TaxableBase = purchaseOrder.Subtotal12,
                                    TaxValue =  purchaseOrder.IVA 
                                }
                            }
                        }
                    },
                    AdditionalFields = additionalField,
                    Payments = new List<PaymentModel>{
                        new PaymentModel
                        {
                            Name = "TARJETA DE CREDITO",
                            PaymentMethodCode = "19",
                            Term = 0,
                            TimeUnit = "DIAS",
                            Total = purchaseOrder.Total
                        }
                    },
            };

        }             

        internal static async Task<OperationResult<PurchasePayment>> ValidatePayment(this IPurchaseOrderService service, PurchasePayment payment)
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

            return await Task.FromResult(service.UpdatePurchasePayment(payment));
        }

        /// <summary>
        /// Validamos el pago contra el web service de alignet para comprobar que no es fraudulento.
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static async Task<OperationResult<PurchasePayment>> ValidatePayment(this IPurchaseOrderService service, PurchasePayment payment, PaymentResultModel model)
        {
            var request = new PaymentRequestModel(payment);

            if (!Constants.DevelopmentMode && string.IsNullOrEmpty(model?.Result))
            {
                var response = await WalletCustomerService.GetPaymentStatus(request);

                if (response.IsSuccess)
                {
                    if (response?.Entity?.errorCode != "8005")
                    {
                        // Es un pago procesado.
                        payment.UpdatePayment(response.Entity);
                    }
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
            }
            else
            {
                payment.UpdatePayment(model);
            }

            // Guardo el codigo de verificacion de compra.
            payment.PurchaseVerification = request.PurchaseVerification;

            return await Task.FromResult(service.UpdatePurchasePayment(payment));
        }

        internal static byte[] GetFile(string fname, string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Constants.ResourcesLocation;
            }
            else if(!path.Contains("\\") && !path.Contains("/"))
            {
                path = Path.Combine(Constants.ResourcesLocation, path);
            }

            byte[] file = new byte[0];

            try
            {
                if (!string.IsNullOrEmpty(fname))
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    path = Path.Combine(path, fname);

                    if (File.Exists(path))
                    {
                        // Si se logro obtener el archivo lo devuelvo completo
                        file = File.ReadAllBytes(path);
                    }

                }
            }
            catch (Exception) { }

            return file;

        }
        internal static string GetFiles(string fname, string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Constants.ResourcesLocation;
            }
            else if (!path.Contains("\\") && !path.Contains("/"))
            {
                path = Path.Combine(Constants.ResourcesLocation, path);
            }

            var file = "";

            try
            {
                if (!string.IsNullOrEmpty(fname))
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    path = Path.Combine(path, fname);

                    if (File.Exists(path))
                    {
                        // Si se logro obtener el archivo lo devuelvo completo
                        file = Convert.ToBase64String(File.ReadAllBytes(path));
                    }

                }
            }
            catch (Exception) { }

            return file;

        }
        internal static UserSessionDto ToSessionDto(this RequestSession session, LoginResponseModel model)
        {
            var mapper = new MapperConfiguration(config => config.CreateMap<RequestSession, UserSessionDto>()).CreateMapper();
            var result = mapper.Map<UserSessionDto>(session);
            if (result.Issuer.Establishments?.Count == 0)
            {
                result.Issuer.Establishments = new List<Establishments>() {
                    new Establishments(){
                        Address = result.Issuer.EstablishmentAddress,
                        Name = result.Issuer.BussinesName,
                        Code = result.Issuer.EstablishmentCode,
                        IssuerId = result.Issuer.Id,
                        Id = 1,
                        IssuePoint = new List<IssuePoint>(){
                            new IssuePoint {
                                Code = result.Issuer.IssuePointCode,
                                Name = $"Caja {result.Issuer.IssuePointCode}",
                                EstablishmentsId = 1,
                                IssuerId = result.Issuer.Id
                            }
                        }
                    }
                };
            }

            result.SRIConnected = model.Client?.SRIConnected ?? false;
            result.Messages = model.Messages ?? new List<string>();
            if (model.Client.Username != result.Issuer.RUC)
            {
                // se valida si el usuario pertenece a una transportista
                var _issuePoint = result.Issuer?.Establishments?.Where(c => c.Id == c.IssuePoint?.Where(i => i?.CarrierRUC == model.Client.Username)?.FirstOrDefault()?.EstablishmentsId)
                  .FirstOrDefault()?.IssuePoint?.Where(d => d?.CarrierRUC == model.Client.Username)?.FirstOrDefault();

                if (_issuePoint != null)
                {
                    result.Issuer.Establishments = new List<Establishments>() {
                    new Establishments(){
                        Address =_issuePoint.Establishments.Address,
                        Name = _issuePoint.Establishments.Name,
                        Code = _issuePoint.Establishments.Code,
                        IssuerId = result.Issuer.Id,
                        Id = _issuePoint.Establishments.Id,
                        IssuePoint = new List<IssuePoint>(){
                            new IssuePoint
                            {
                                Code = _issuePoint.Code,
                                Name = _issuePoint.Name,
                                EstablishmentsId = _issuePoint.EstablishmentsId,
                                IssuerId = result.Issuer.Id,
                                CarPlate = _issuePoint.CarPlate,
                                CarrierRUC= _issuePoint.CarrierRUC,
                                CarrierEmail = _issuePoint.CarrierEmail,
                                CarrierPhone = _issuePoint.CarrierPhone,
                                CarrierContribuyente = _issuePoint.CarrierContribuyente,
                                CarrierResolutionNumber = _issuePoint.CarrierResolutionNumber,
                            }
                        }
                    }
                };
                }

            }

            return result;
        }

        /// <summary>
        /// Devuelve el primer error en el modelo de datos
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ModelError GetError(this ModelStateDictionary model)
        {
            return model?.Values?
                .FirstOrDefault(x => x.Errors.Any())?
                .Errors?.FirstOrDefault();
        }
        internal static async Task<OperationResult<Subscription>> SubscriptionProcess(this ISubscriptionService service, Subscription sub, PurchaseSubscription pursub)
        {
            try
            {                
                if(sub != null)
                {
                    sub.LastModifiedOn = DateTime.Now;
                    sub.Status = SubscriptionStatusEnum.Activa;
                    sub.StatusMsg = "OK";
                    sub.SubscriptionStartDate = DateTime.Now;
                    sub.SubscriptionExpirationDate = sub.SubscriptionStartDate?.AddYears(1);
                    pursub.LastModifiedOn = DateTime.Now;                    
                    pursub.Status = PurchaseOrderSubscriptionStatusEnum.Payed;                   
                    pursub.RequestElectronicSign = RequestElectronicSignEnum.Pending;
                    pursub.RequestElectronicSignMsg = "Pendiente de enviar solicitud";
                    pursub.LicenceTypeId = sub.LicenceTypeId;
                    pursub.PurchaseOrder.Status = PurchaseOrderStatusEnum.Payed;
                    var result = await Task.FromResult(service.UpdateSubscription(sub));
                    await Task.FromResult(service.UpdatePurchaseSubscription(pursub));
                    Logger.Log($"Subscription.{sub?.RUC}.PROCESS.OK", "Procesamiento de Suscripción de cuenta anual");

                    return result;
                }
                else
                {
                    var message = "Error al procesar la suscripción anual.";
                    return await Task.FromResult(sub.ToResult(false, HttpStatusCode.Conflict, message));
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Subscription.{sub?.RUC}.PROCESS.ERROR", ex, "REQUEST: ", sub);
                sub.Status = SubscriptionStatusEnum.Inactiva;
                sub.StatusMsg = $"{DateTime.Now} - {ex.Message}";               
                return await Task.FromResult(service.UpdateSubscription(sub));
            }
        }

        internal static OperationResult<Subscription> UpdateSubscription(this ISubscriptionService service, ElectronicSign electronic)
        {
            var sub = new Subscription();
            try
            {
                if (electronic != null)
                {
                    var _sub = service.GetSubscription(electronic.RUC);
                    if (_sub == null)
                    {
                        sub.RUC = electronic.RUC;
                        sub.CreatedOn = DateTime.Now;
                        sub.IssuerId = electronic.Id;
                        sub.LicenceTypeId = (long)LicenceTypeEnum.Free;//ojo modificar
                        sub.SubscriptionStartDate = DateTime.Now;
                        sub.SubscriptionExpirationDate = DateTime.Now.AddYears(1);
                        sub.LastModifiedOn = DateTime.Now;
                        sub.StatusMsg = "OK";
                        sub.Status = SubscriptionStatusEnum.Activa;
                        var result = service.AddSubscription(sub);
                        return result;
                    }
                    else
                    {
                        _sub.LastModifiedOn = DateTime.Now;
                        _sub.Status = SubscriptionStatusEnum.Activa;
                        _sub.StatusMsg = "OK";
                        _sub.SubscriptionStartDate = DateTime.Now;
                        _sub.SubscriptionExpirationDate = DateTime.Now.AddYears(1);
                        var result = service.UpdateSubscription(_sub);
                        return result;
                    }
                }

                var message = "Error al procesar la suscripción anual.";
                return sub.ToResult(false, HttpStatusCode.Conflict, message);

            }
            catch (Exception ex)
            {
                return sub.ToResult(false, HttpStatusCode.Conflict, ex.Message);
            }
        }
        internal static async Task<OperationResult<Subscription>> UpdateSubscriptionProcess(this ISubscriptionService service, ElectronicSign electronic)
        {
            var sub = new Subscription();
            try
            {
                if (electronic != null)
                {
                    var _sub = service.GetSubscription(electronic.RUC);
                    if (_sub == null)
                    {
                        sub = new Subscription()
                        {
                            RUC = electronic.RUC,
                            CreatedOn = DateTime.Now,
                            IssuerId = electronic.IssuerId,
                            LicenceTypeId = (long)LicenceTypeEnum.Free,//ojo modificar
                            SubscriptionStartDate = DateTime.Now,
                            SubscriptionExpirationDate = DateTime.Now.AddYears(1),
                            LastModifiedOn = DateTime.Now,
                            StatusMsg = "OK",
                            Status = SubscriptionStatusEnum.Activa
                        };

                        var result = await Task.FromResult(service.AddSubscription(sub));
                        return result;
                    }
                    else {                       

                        _sub.LastModifiedOn = DateTime.Now;
                        _sub.Status = SubscriptionStatusEnum.Activa;
                        _sub.StatusMsg = "OK";
                        _sub.SubscriptionStartDate = DateTime.Now;
                        _sub.SubscriptionExpirationDate = DateTime.Now.AddYears(1);

                        var result = await Task.FromResult(service.UpdateSubscription(_sub));
                        return result;
                    }
                }

                var message = "Error al procesar la suscripción anual.";
                return await Task.FromResult(sub.ToResult(false, HttpStatusCode.Conflict, message));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(sub.ToResult(false, HttpStatusCode.Conflict, ex.Message));              
            }
        }
        internal static async Task<OperationResult<PurchaseSubscription>> SubscriptionGenerateInvoice(this ISubscriptionService service, Subscription sub, PurchaseSubscription pursub)
        {
            var PurchaseOrder = new PurchaseOrder();
            if (pursub.InvoiceId > 0)
            {
                var message = "La suscripcion especificada ya fue facturada";
                Logger.Log($"Subscription.{sub?.RUC}.INVOICE.ERROR", message, sub);
                return await Task.FromResult(pursub.ToResult(false, HttpStatusCode.Conflict, message));
            }

            if (pursub != null)
            {
                PurchaseOrder = service.GetPurchaseOrderById(pursub.PurchaseOrderId)?.Entity;
            }

            var invoice = await PurchaseOrder?.GenerateInvoice();

            if (invoice.IsSuccess)
            {
                pursub.InvoiceResult = invoice?.Entity?.Reason; 
                pursub.InvoiceId = (long)invoice?.Entity?.Id;
                pursub.InvoiceNumber = invoice?.Entity?.DocumentNumber;
                pursub.InvoiceDate = invoice?.Entity?.IssuedOn;
                pursub.PaymentType = PaymentTypeEnum.CreditCard;
                pursub.Status = PurchaseOrderSubscriptionStatusEnum.Payed;
                pursub.PurchaseOrder.InvoiceDate = invoice?.Entity?.IssuedOn;
                pursub.PurchaseOrder.InvoiceNumber = invoice?.Entity?.DocumentNumber;
                pursub.PurchaseOrder.Status = PurchaseOrderStatusEnum.Payed;                             
                pursub.RequestElectronicSign = RequestElectronicSignEnum.Pending;
                pursub.RequestElectronicSignMsg = "Pendiente de enviar solicitud";
                pursub.LastModifiedOn = DateTime.Now;

                //actualizar la suscripción
                var plan = service.GetLicenceTypeById(pursub.LicenceTypeId.Value);
                var expirationDate = DateTime.Now.AddYears(1);
                if (sub.Status == SubscriptionStatusEnum.Activa && sub.SubscriptionExpirationDate > DateTime.Now && sub.LicenceType.Code != Constants.PlanBasic)
                {
                    TimeSpan totalDias = sub.SubscriptionExpirationDate.Value - DateTime.Now;
                    var dias = (int)totalDias.TotalDays;
                    if (dias > 0)
                    {
                        expirationDate = expirationDate.AddDays(dias +1);
                    }
                }
                pursub.SubscriptionLog = new SubscriptionLog
                {
                    RUC = sub.RUC,
                    IssuerId = sub.IssuerId,
                    SubscriptionId = sub.Id,
                    LicenceTypeId = sub.LicenceTypeId,
                    SubscriptionStartDate = sub.SubscriptionStartDate ?? DateTime.Now,
                    SubscriptionExpirationDate = sub.SubscriptionExpirationDate ?? expirationDate,
                    IssuedDocument = sub.IssuedDocument ?? 0,
                    BalanceDocument = sub.BalanceDocument ?? 0,
                    Observation = sub.StatusMsg,
                    Status = sub.Status,
                };                
                sub.LicenceTypeId = plan.Id;
                sub.Status = SubscriptionStatusEnum.Activa;
                sub.SubscriptionStartDate = DateTime.Now;
                sub.SubscriptionExpirationDate = expirationDate;
                sub.LastModifiedOn = DateTime.Now;
                sub.StatusMsg = "Ok";
                sub.AmountDocument = plan.AmountDocument;
                sub.IssuedDocument = null;
                sub.BalanceDocument = null;
                pursub.Subscription = sub;
            }
            else
            {
                pursub.InvoiceResult = $"{DateTime.Now} - No se generó la factura para esta solicitud \r\n{invoice.UserMessage ?? pursub.InvoiceResult}.";
            }

            var result = service.UpdatePurchaseSubscription(pursub);
            return await Task.FromResult(result);

        }


    }

    public class ElectronicSignServiceRequest
    {
        public string apikey { get; set; }
        public string uid { get; set; }
        public short cobro_directo { get; set; } = 1;
        public RucTypeEnum tipo_firma { get; set; }
        public string tipodocumento { get; set; }
        public string numerodocumento { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string telefono { get; set; }
        public string correo { get; set; }
        public string direccion { get; set; }
        public string ruc_empresa { get; set; }
        public string razon_social { get; set; }
        public string direccion_ruc { get; set; }
        public string ciudad { get; set; }
        public string provincia { get; set; }        
        public SignatureValidyEnum id_vigenciafirma { get; set; }
        public VerificationTypeEnum id_tipoverificacion { get; set; }
        public FileFormatEnum id_formato { get; set; }
        public string id_direccion_skype { get; set; }
        public byte[] id_copiacedula { get; set; }
        public byte[] id_copiapapvotacion { get; set; }
        public byte[] id_copiaruc { get; set; }
        public byte[] id_autreprelegal { get; set; }
        public byte[] id_nombramiento { get; set; }
        public byte[] id_const { get; set; }

    }

    public class ElectronicSignUanatacaServiceRequest
    {
        public string apikey { get; set; }
        public string uid { get; set; }
        public RucTypeEnum tipo_solicitud { get; set; }
        public FileFormatEnum contenedor { get; set; }        
        public string nombres { get; set; }
        public string apellido1 { get; set; }
        public string apellido2 { get; set; }        
        public string tipodocumento { get; set; }
        public string numerodocumento { get; set; }
        public string coddactilar { get; set; }
        public string ruc_personal { get; set; }
        public string sexo { get; set; }
        public string fecha_nacimiento { get; set; }
        public string nacionalidad { get; set; }        
        public string telfCelular { get; set; }
        public string telfCelular2 { get; set; }        
        public string telfFijo { get; set; }
        public string eMail { get; set; }
        public string eMail2 { get; set; }        
        public string provincia { get; set; }
        public string ciudad { get; set; }        
        public string direccion { get; set; }
        public string empresa { get; set; }
        public string ruc_empresa { get; set; }
        public string direccion_ruc { get; set; }       
        public string vigenciafirma { get; set; }        
        public string cargo { get; set; }
        public string f_cedulaFront { get; set; }
        public string f_cedulaBack { get; set; }
        public string f_selfie { get; set; }
        public string f_copiaruc { get; set; }
        public string f_nombramiento { get; set; }
        public string f_constitucion { get; set; }
        public string f_adicional1 { get; set; }
        public string f_adicional2 { get; set; }   
    }

    public class ElectSignStatusProcessServiceRequest
    {
        public string apikey { get; set; }
        public string uid { get; set; }
        public string documento { get; set; }
        public string tipo_solicitud { get; set; }        

    }

    public class ElectronicSignServiceResult
    {
        public ElectronicSignServiceResult()
        {

        }

        public ElectronicSignServiceResult(bool isOK, string msg, string token)
        {
            this.result = isOK;
            this.message = msg;
            this.token = token;
        }        
        public string message { get; set; }
        public string token { get; set; }
        public bool result { get; set; }
    }

    public class ElectSignStatusPorcessServiceResult
    {
        public ElectSignStatusPorcessServiceResult()
        {

        }

        public ElectSignStatusPorcessServiceResult(bool isOK, Data sol)
        {
            this.result = isOK;
            this.data = sol;
        }

        public Data data { get; set; }
        public bool result { get; set; }
    }

    public class Data
    {
        public IList<Solicitudes> solicitudes { get; set; }

    }

    public class Solicitudes
    {
        public string tipo_solicitud { get; set; }
        public string estado { get; set; }
        public string observacion { get; set; }
        public string documento_tipo { get; set; }
        public string documento { get; set; }
        public string nombre_completo { get; set; }
        public string validez { get; set; }
        public string uid { get; set; }
        public string creado_por { get; set; }
        public DateTime fecha_registro { get; set; }
    }
   
}