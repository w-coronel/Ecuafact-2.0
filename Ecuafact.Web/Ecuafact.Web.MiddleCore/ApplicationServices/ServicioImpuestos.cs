using Ecuafact.Web.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public static class ServicioImpuestos
    { 
        public static List<RetentionTax> ObtenerImpuestos(string token)
        {
            var taxes = new List<RetentionTax>();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/taxes").Result;
                if (response.IsSuccessStatusCode)
                {
                    taxes = response.GetContent<List<RetentionTax>>();
                }
            }

            return taxes;
        }

        public static Task<HttpResponseMessage> EliminarAsync(string issuerToken, long id)
        {
            throw new NotImplementedException();
        } 

        public static async Task<List<RetentionTax>> ObtenerImpuestosAsync(string token, long? type = null, string filtro = null)
        {
            var taxes = new List<RetentionTax>();

            try
            {
                var url = $"{Constants.WebApiUrl}/taxes";

                if (type>0)
                {
                    url += $"/{type}";
                }

                if (!string.IsNullOrEmpty(filtro))
                {
                    url += $"?searchTerm={filtro}";
                }

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        taxes = response.GetContent<List<RetentionTax>>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                // Si hubo errores se envia la lista vacia
            }

            return taxes;
        }

        public static RetentionTax ObtenerImpuestoById(string token, long id)
        { 
            var response = ClientHelper
                .GetClient(token)
                .GetAsync($"{Constants.WebApiUrl}/taxes/{id}")
                .Result;

            if (response.IsSuccessStatusCode) // No hubo un buen resultado
            {
                var result = response.GetContent<RetentionTax>();

                if (result.Id > 0)
                {
                    return result; // Si existe el impuesto
                }
            }
            

            return new RetentionTax();
        }

        public static async Task<HttpResponseMessage> CrearAsync(string token, RetentionTax item)
        {
            var httpClient = ClientHelper.GetClient(token);
            {
                var response = await httpClient.PostAsync($"{Constants.WebApiUrl}/taxes", item.ToContent());

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }

            // No se pudo guardar el registro
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }

        public static async Task<HttpResponseMessage> ActualizarAsync(string token, RetentionTax item)
        {
            var httpClient = ClientHelper.GetClient(token);
            {
                var response = await httpClient.PutAsync(new Uri($"{Constants.WebApiUrl}/taxes/{item.Id}"), item.ToContent());

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

            }

            // No se pudo guardar el registro
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}





