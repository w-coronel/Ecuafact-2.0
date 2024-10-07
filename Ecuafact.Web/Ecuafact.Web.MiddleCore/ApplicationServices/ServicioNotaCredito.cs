using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecuafact.Web.Domain.Entities;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public class ServicioNotaCredito
    { 
        public static async Task<HttpResponseMessage> GuardarNotaCreditoAsync(string issuerToken, CreditNoteRequestModel model)
        {
            var httpClient = ClientHelper.GetClient(issuerToken);
            {
                var content = new StringContent(JsonConvert.SerializeObject(model));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response;

                if (model.Id.HasValue && model.Id.Value > 0)
                {
                    response = await httpClient.PutAsync($"{Constants.WebApiUrl}/CreditNote/{model.Id}", content).ConfigureAwait(false);
                }
                else
                {
                    response = await httpClient.PostAsync($"{Constants.WebApiUrl}/CreditNote", content).ConfigureAwait(false);
                }

                return response;
               
            }
        }



        public static CreditNoteModel BuscarNotaCredito(string token, string numeroDocumento)
        {
            var document = new CreditNoteModel();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/CreditNote?search={numeroDocumento}").Result;

                if (response.IsSuccessStatusCode)
                {
                    document = response.GetContent<CreditNoteModel>();
                }
            }
            return document;
        }



        public static CreditNoteModel BuscarNotaCreditoPorId(string token, object id)
        {
            var document = new CreditNoteModel();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/CreditNote/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    document = response.GetContent<CreditNoteModel>();
                }
            }
            return document;
        }

        public static List<CreditNoteModel> ObtenerNotaCreditos(string token)
        {
            var documentos = new List<CreditNoteModel>();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/CreditNote?status=all").Result;

                if (response.IsSuccessStatusCode)
                { 
                    documentos = response.GetContent<List<CreditNoteModel>>();
                }
            }
            return documentos;
        }
         
    }
}
