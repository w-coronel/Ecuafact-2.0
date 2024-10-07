using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecuafact.Web.Domain.Entities;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public class ServicioRetencion
    {  
        public static async Task<HttpResponseMessage> GuardarRetencionAsync(string issuerToken, RetentionRequestModel model)
        {
            var httpClient = ClientHelper.GetClient(issuerToken);

            var content = new StringContent(JsonConvert.SerializeObject(model));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response;
            if (model.Id.HasValue && model.Id.Value > 0)
            {
                response = await httpClient.PutAsync($"{Constants.WebApiUrl}/Retention/{model.Id}", content).ConfigureAwait(false);
            }
            else
            {
                response = await httpClient.PostAsync($"{Constants.WebApiUrl}/Retention", content).ConfigureAwait(false);
            }

            return response;
           
        }



        public static RetentionModel BuscarRetencion(string token, string numeroDocumento)
        {
            var document = new RetentionModel();

            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync(new Uri($"{Constants.WebApiUrl}/Retention?search={numeroDocumento}")).Result;

            if (response.IsSuccessStatusCode)
            {
                document = response.GetContent<RetentionModel>();
            }

            return document;
        }



        public static RetentionModel BuscarRetencionPorId(string token, object id)
        {
            var document = new RetentionModel();

            var httpClient = ClientHelper.GetClient(token);

            var response = httpClient.GetAsync(new Uri($"{Constants.WebApiUrl}/Retention/{id}")).Result;

            if (response.IsSuccessStatusCode)
            {
                document = response.GetContent<RetentionModel>();
            }

            return document;
        }

        public static List<RetentionModel> ObtenerRetenciones(string token)
        {
            var documentos = new List<RetentionModel>();

            var httpClient = ClientHelper.GetClient(token);
            
            var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Retention?status=all").Result;

            if (response.IsSuccessStatusCode)
            { 
                documentos = response.GetContent<List<RetentionModel>>();
            }
            
            return documentos;
        }
         

    }
}
