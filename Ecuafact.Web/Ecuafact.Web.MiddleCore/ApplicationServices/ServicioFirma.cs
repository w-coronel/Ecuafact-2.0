using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecuafact.Web.Domain.Entities;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Ecuafact.Web.Domain.Services;
using System.Net;
using System.Linq;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public class ServicioFirma
    {
         
        public static async Task<OperationResult<ElectronicSignModel>> GuardarFirmaAsync(string issuerToken, ElectronicSignRequest model)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(issuerToken);
                {
                    var content = new StringContent(JsonConvert.SerializeObject(model));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await httpClient.PostAsync($"{Constants.WebApiUrl}/ElectronicSign", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.GetContentAsync<ElectronicSignModel>();
                        return new OperationResult<ElectronicSignModel>(true, HttpStatusCode.OK, result);
                    }
                    else
                    {
                        var result = await response.GetContentAsync<OperationResult>();
                        return new OperationResult<ElectronicSignModel>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<ElectronicSignModel>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = ex.ToString(),
                    UserMessage = ex.Message
                };
            }
        }

        public static async Task<OperationResult<ElectronicSignModel>> EditarFirmaAsync(string issuerToken, ElectronicSignRequest model)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(issuerToken);
                {
                    var content = new StringContent(JsonConvert.SerializeObject(model));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await httpClient.PutAsync($"{Constants.WebApiUrl}/ElectronicSign/{model.Id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.GetContentAsync<ElectronicSignModel>();
                        return new OperationResult<ElectronicSignModel>(true, HttpStatusCode.OK, result);
                    }
                    else
                    {
                        var result = await response.GetContentAsync<OperationResult>();
                        return new OperationResult<ElectronicSignModel>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<ElectronicSignModel>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = ex.ToString(),
                    UserMessage = ex.Message
                };
            }
        }

        public static async Task<OperationResult<ElectronicSignModel>> GetSolicitudAsync(string token, string id = null)
        {
            var solicitudes = new OperationResult<ElectronicSignModel>(false);

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/ElectronicSign/{id}");

                    if (response.IsSuccessStatusCode)
                    {
                        if (string.IsNullOrEmpty(id))
                        {
                            var lista = await response.GetContentAsync<List<ElectronicSignModel>>();

                            if (lista.Count > 0)
                            {
                                solicitudes = new OperationResult<ElectronicSignModel>(true, HttpStatusCode.OK, lista.LastOrDefault());
                            }
                        }
                        else
                        {
                            solicitudes = await response.GetContentAsync<OperationResult<ElectronicSignModel>>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<ElectronicSignModel>(ex);
            }

            return solicitudes;
        }

        public static async Task<OperationResult<ElectronicSignModel>> BuscarPendienteAsync(string token, string ruc = null)
        {
            var solicitud = new OperationResult<ElectronicSignModel>(false);

            try
            { 
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/ElectronicSign/Pending?ruc={ruc}");

                    if (response.IsSuccessStatusCode)
                    {
                        solicitud = await response.GetContentAsync<OperationResult<ElectronicSignModel>>();
                    }
                }

            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return solicitud;
        }

        public static async Task<List<ElectronicSignModel>> BuscarAsync(string token, string filtro = null, string ruc= null,
             DateTime? startDate = null, DateTime? endDate = null, ElectronicSignStatusEnum? status = null, int? pagina = null, int? cantidad = null)
        {
            var solicitudes = new List<ElectronicSignModel>();
            
            try
            {
                string url = $"{Constants.WebApiUrl}/ElectronicSign?search={filtro}&ruc={ruc}&startdate={startDate?.ToString("yyyy-MM-dd")}&enddate={endDate?.ToString("yyyy-MM-dd")}&status={status}&page={pagina}&pagesize={cantidad}";

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync(new Uri(url));

                    if (response.IsSuccessStatusCode)
                    {
                        solicitudes = await response.GetContentAsync<List<ElectronicSignModel>>();
                    }
                    else
                    {
                        var error = await response.GetContentAsync<OperationResult>();
                    }
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                // Si hubo errores se envia la lista vacia
            }

            return solicitudes;
        }

        public static OperationResult<ElectronicSignModel> BuscarPendiente(string token, string ruc = null)
        {
            var solicitud = new OperationResult<ElectronicSignModel>(false);

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response =  httpClient.GetAsync($"{Constants.WebApiUrl}/ElectronicSign/Pending?ruc={ruc}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        solicitud =  response.GetContent<OperationResult<ElectronicSignModel>>();
                    }
                }

            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return solicitud;
        }

        public static OperationResult<ElectronicSignModel> EstadoFirmaElectronica(string token, string ruc = null)
        {
            var solicitud = new OperationResult<ElectronicSignModel>(false);

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = httpClient.GetAsync($"{Constants.WebApiUrl}/electronicSign/status?ruc={ruc}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        solicitud = response.GetContent<OperationResult<ElectronicSignModel>>();
                    }
                }

            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return solicitud;
        }

        public static async Task<OperationResult<PurchaseSubscription>> SolicitudPendientePorSubscripcionAsync(string token, string RUC)
        {
            var solicitudes = new OperationResult<PurchaseSubscription>(false);

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/Subscription/{RUC}/pendingrequest");

                    if (response.IsSuccessStatusCode)
                    {
                        solicitudes = await response.GetContentAsync<OperationResult<PurchaseSubscription>>();                 
                    }
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<PurchaseSubscription>(ex);
            }

            return solicitudes;
        }

        public static long ValidarEmailBySolicitud(string token, string email = null)
        {
            var result = 0;

            try
            {
                var httpClient = ClientHelper.GetClient(token);                {
                    var response = httpClient.GetAsync($"{Constants.WebApiUrl}/electronicSign/{email}/requestedAmount").Result;
                    if (response.IsSuccessStatusCode) 
                    {
                        result = response.GetContent<int>();
                    }
                }

            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return result;
        }

        public static async Task<OperationResult<bool>> GuardarCertificado(string token, CertificateConf certificate)
        {
            var solicitud = new OperationResult<bool>(false, HttpStatusCode.InternalServerError, false);

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var model = new {
                        certificate.Ruc,
                        certificate.CertificatePass,
                        certificate.CertificateRaw
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(model));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await httpClient.PostAsync($"{Constants.WebApiUrl}/ElectronicSign/Config", content); 
                    if (response.IsSuccessStatusCode)
                    {                        
                        solicitud = new OperationResult<bool>(true, HttpStatusCode.OK, true); 
                    }
                }

            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return solicitud;
        }

    }
}
