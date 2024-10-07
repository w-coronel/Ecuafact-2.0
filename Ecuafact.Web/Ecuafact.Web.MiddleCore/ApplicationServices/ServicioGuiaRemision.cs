using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecuafact.Web.Domain.Entities;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public class ServicioGuiaRemision 
    { 
        public static async Task<HttpResponseMessage> GuardarGuiaRemisionAsync(string issuerToken, ReferralGuideRequestModel model)
        {
            var httpClient = ClientHelper.GetClient(issuerToken);
            {
                var content = new StringContent(JsonConvert.SerializeObject(model));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response;
                if (model.Id.HasValue && model.Id.Value > 0)
                {
                    response = await httpClient.PutAsync($"{Constants.WebApiUrl}/ReferralGuide/{model.Id}", content).ConfigureAwait(false);
                }
                else
                {
                    response = await httpClient.PostAsync($"{Constants.WebApiUrl}/ReferralGuide", content).ConfigureAwait(false);
                }

                return response;
            }
        }

        public static async Task<List<ReferralGuideModel>> BuscarGuiaRemisionContribAsync(string token, long contributorId, string filtro)
        {
            var documentos = new List<ReferralGuideModel>();

            try
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.

                string url = $"{Constants.WebApiUrl}/ReferralGuide?contributorId={contributorId}&search={filtro}";

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetStringAsync(new Uri(url));

                    if (!string.IsNullOrEmpty(response))
                    {
                        documentos = JsonConvert.DeserializeObject<List<ReferralGuideModel>>(response);
                    }
                }
                
            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return documentos;
        }



        public static ReferralGuideModel BuscarGuiaRemision(string token, string numeroDocumento)
        {
            var document = new ReferralGuideModel();
            string response = "";

            var httpClient = ClientHelper.GetClient(token);
            {
                response = httpClient.GetStringAsync(new Uri($"{Constants.WebApiUrl}/ReferralGuide?search={numeroDocumento}")).Result;

                if (response != null)
                {
                    document = JsonConvert.DeserializeObject<ReferralGuideModel>(response);
                }
            }
            return document;
        }



        public static ReferralGuideModel BuscarGuiaRemisionPorId(string token, object id)
        {
            var document = new ReferralGuideModel();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync(new Uri($"{Constants.WebApiUrl}/ReferralGuide/{id}")).Result;

                if (response.IsSuccessStatusCode)
                {
                    document = response.GetContent<ReferralGuideModel>();
                }
            }
            return document;
        }

        public static List<ReferralGuideModel> ObtenerGuiaRemisions(string token)
        {
            var documentos = new List<ReferralGuideModel>();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/ReferralGuide?status=all").Result;

                if (response.IsSuccessStatusCode)
                {

                    documentos = response.GetContent<List<ReferralGuideModel>>();
                }
            }
            return documentos;
        }
         

    }
}
