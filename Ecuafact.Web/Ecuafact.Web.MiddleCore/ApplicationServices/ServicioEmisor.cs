using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Services;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Web;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Ecuafact.Web.Domain.Extensions;
using System.Linq;
using System.Collections.Generic;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public static class ServicioEmisor
    {       
        public static IssuerDto ObtenerEmisorActual(string token)
        {
            var emisor = new IssuerDto();
            
            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Issuers").Result;

            if (response.IsSuccessStatusCode)
            {
                emisor = response.GetContent<IssuerDto>();
            }

            return emisor;
        }

        public static IssuerDto ObtenerEmisorPorRUC(string token, string issuerRuc)
        {
            var httpClient = ClientHelper.GetClient(token);
             
            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Issuers?ruc={issuerRuc}").Result;

            if (response.IsSuccessStatusCode)
            { 
                return response.GetContent<IssuerDto>();
            }
             
            return default;
        }

        public static X509Certificate2 ValidateCertificate(HttpPostedFileBase certificate, string key)
        {
            var raw = certificate.GetBytes();
            return new X509Certificate2(raw, key);
        }

        public static async Task<OperationResult<IssuerDto>> GuardarAsync(string token, IssuerDto model)
        {
            if (model != null)
            {
                if (model.CertificateRaw != null && model.CertificateRaw.ContentLength > 0)
                {
                    var certExtension = Path.GetExtension(model.CertificateRaw.FileName);
                    if (string.IsNullOrEmpty(certExtension) || certExtension == ".")
                    {
                        certExtension = ".p12";
                    }

                    model.CertificateFile = model.CertificateRaw.GetBytes();
                }

                if (model.LogoRaw != null && model.LogoRaw.ContentLength > 0)
                {
                    model.Logo = $"{model.RUC}_logo.jpg";
                    model.LogoFile = model.LogoRaw.GetBytes();
                }
                  
                // ACTUALIZAR  
                var response = await ClientHelper.GetClient(token)
                    .PutAsync($"{Constants.WebApiUrl}/Issuers/{model.Id}", model.ToContent());
                
                var result = new OperationResult<IssuerDto>(response.IsSuccessStatusCode, response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    result.Entity = await response.GetContentAsync<IssuerDto>();

                    if (model.LogoFile?.Length > 0)
                    {
                        try
                        {
                            // Guardamos el logo en la carpeta de logos de nuestra aplicacion para uso posterior
                            var LOGOS_LOCATION = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)
                                                            .Replace("file:\\", "")
                                                            .Replace("/", "\\")
                                                            .Replace("bin", "logos");
                            File.WriteAllBytes(Path.Combine(LOGOS_LOCATION, model.Logo), model.LogoFile);
                        }
                        catch (Exception e)
                        { 
                        }
                    }
                    
                }
                else
                {
                    var obj = await response.GetContentAsync<ErrorResult>();

                    if (obj != null)
                    {
                        result.DevMessage = obj.DeveloperMessage;
                        result.UserMessage = obj.UserMessage;
                        result.StatusCode = obj.ErrorCode;
                    }
                }

                return result;
            }

            return new OperationResult<IssuerDto>(false, HttpStatusCode.InternalServerError);
        }
         
        public static async Task<OperationResult<IssuerDto>> CrearAsync(string token, SRIRequestModel model)
        {
            var httpClient = ClientHelper.GetClient(token);
            {
                var result = await httpClient.PostAsync($"{Constants.WebApiUrl}/Issuers", model.ToContent());

                return await result.GetContentAsync<OperationResult<IssuerDto>>();
            }
        }  

        public static IssuerDto ObtenerEmisorPorId(string token, long? id)
        {
            var emisor = new IssuerDto();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Issuers/{id}").Result;
                if (response.IsSuccessStatusCode)
                { 
                    emisor = response.GetContent<IssuerDto>();
                }
            }
            return emisor;
        }


        public static async Task<OperationResult<SubscriptionModel>> SubscriptionAsync(string token, long id)
        {
            var httpClient = ClientHelper.GetClient(token);
            {
                var result = await httpClient.GetAsync($"{Constants.WebApiUrl}/Subscription/{id}");

                return await result.GetContentAsync<OperationResult<SubscriptionModel>>();
            }
        }

        #region establecimientos 
        public static async Task<List<Establishments>> SearchEstablishmentsAsync(string token, string search = "", int? page = null, int? pageSize = null)
        {
            try
            {
                var qs = "";

                qs += $"search={search}";

                if (page != null)
                {
                    qs += string.IsNullOrEmpty(qs) ? "" : "&" + $"pageNumber={page}";
                }

                if (pageSize != null)
                {
                    qs += string.IsNullOrEmpty(qs) ? "" : "&" + $"pageSize={pageSize}";
                }

                var httpClient = ClientHelper.GetClient(token);

                var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/establishment?{qs}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<List<Establishments>>();
                }
            }
            catch { }

            return new List<Establishments>();
        }
        public static async Task<Establishments> GetEstablishmentsByIdAsync(string token, long id)
        {
            try
            {

                var httpClient = ClientHelper.GetClient(token);

                var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/establishment/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<Establishments>();
                }
            }
            catch { }

            return new Establishments();
        }
        public static async Task<HttpResponseMessage> CrearEstablishmentsAsync(string token, Establishments item)
        {

            return await ClientHelper
                .GetClient(token)
                .PostAsync($"{Constants.WebApiUrl}/establishment", item.ToContent());

        }
        public static async Task<HttpResponseMessage> ActualizarEstablishmentsAsync(string token, Establishments item)
        {

            return await ClientHelper
                .GetClient(token)
                .PutAsync($"{Constants.WebApiUrl}/establishment/{item.Id}", item.ToContent());

        }

        #endregion

        #region puntos de emisión
        public static async Task<List<IssuePoint>> SearchIssuePointAsync(string token, long establishmentsId, string search = "", int? page = null, int? pageSize = null)
        {
            try
            {
                var qs = "";

                qs += $"search={search}";

                if (page != null)
                {
                    qs += string.IsNullOrEmpty(qs) ? "" : "&" + $"pageNumber={page}";
                }

                if (pageSize != null)
                {
                    qs += string.IsNullOrEmpty(qs) ? "" : "&" + $"pageSize={pageSize}";
                }

                var httpClient = ClientHelper.GetClient(token);

                var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/issuePoint/{establishmentsId}/Search?{qs}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<List<IssuePoint>>();
                }
            }
            catch { }

            return new List<IssuePoint>();
        }
        public static async Task<IssuePoint> GetIssuePointByIdAsync(string token, long id)
        {
            try
            {

                var httpClient = ClientHelper.GetClient(token);

                var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/issuePoint/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<IssuePoint>();
                }
            }
            catch { }

            return new IssuePoint();
        }
        public static async Task<List<IssuePoint>> GetIssuePointsByEstablishmentIdAsync(string token, long id)
        {
            try
            {

                var httpClient = ClientHelper.GetClient(token);

                var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/issuePoints/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<List<IssuePoint>>();
                }
            }
            catch { }

            return new List<IssuePoint>();
        }
        public static async Task<List<IssuePoint>> GetIssuePointsByIssuer(string token)
        {
            try
            {

                var httpClient = ClientHelper.GetClient(token);

                var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/issuePoints/issuer");

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<List<IssuePoint>>();
                }
            }
            catch { }

            return new List<IssuePoint>();
        }
        public static async Task<HttpResponseMessage> CrearIssuePointAsync(string token, IssuePoint item)
        {

            return await ClientHelper
                .GetClient(token)
                .PostAsync($"{Constants.WebApiUrl}/issuePoint", item.ToContent());

        }
        public static async Task<HttpResponseMessage> ActualizarIssuePointAsync(string token, IssuePoint item)
        {

            return await ClientHelper
                .GetClient(token)
                .PutAsync($"{Constants.WebApiUrl}/issuePoint/{item.Id}", item.ToContent());

        }
        public static CarrierIssuer BuscarEmisorPorRUC(string token, string issuerRuc)
        {
            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/carrier/{issuerRuc}/issuers?").Result;

            if (response.IsSuccessStatusCode)
            {
                return response.GetContent<CarrierIssuer>();
            }

            return default;
        }
        public static async Task<HttpResponseMessage> EliminarIssuePointAsync(string token, long id)
        {
            var httpClient = ClientHelper.GetClient(token);

            return await httpClient.DeleteAsync($"{Constants.WebApiUrl}/carrier/issuePoint/{id}", default); ;

        }

        #endregion
    }

}

