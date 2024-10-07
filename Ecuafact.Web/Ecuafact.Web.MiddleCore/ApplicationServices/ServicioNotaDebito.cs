using Ecuafact.Web.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{    
    public class ServicioNotaDebito
    {
        public static async Task<HttpResponseMessage> GuardarNotaDebitoAsync(string issuerToken, DebitNoteRequestModel model)
        {
            var httpClient = ClientHelper.GetClient(issuerToken);
            {
                var content = new StringContent(JsonConvert.SerializeObject(model));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response;

                if (model.Id.HasValue && model.Id.Value > 0)
                {
                    response = await httpClient.PutAsync($"{Constants.WebApiUrl}/DebitNote/{model.Id}", content).ConfigureAwait(false);
                }
                else
                {
                    response = await httpClient.PostAsync($"{Constants.WebApiUrl}/DebitNote", content).ConfigureAwait(false);
                }

                return response;
            }
        }



        public static DebitNoteModel BuscarNotaDebito(string token, string numeroDocumento)
        {
            var document = new DebitNoteModel();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/DebitNote?search={numeroDocumento}").Result;

                if (response.IsSuccessStatusCode)
                {
                    document = response.GetContent<DebitNoteModel>();
                }
            }
            return document;
        }



        public static DebitNoteModel BuscarNotaDebitoPorId(string token, object id)
        {
            var document = new DebitNoteModel();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/DebitNote/{id}").Result;

                if (response.IsSuccessStatusCode)
                {
                    document = response.GetContent<DebitNoteModel>();
                }
            }
            return document;
        }

        public static List<DebitNoteModel> ObtenerNotaDebitos(string token)
        {
            var documentos = new List<DebitNoteModel>();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/DebitNote?status=all").Result;

                if (response.IsSuccessStatusCode)
                {
                    documentos = response.GetContent<List<DebitNoteModel>>();
                }
            }
            return documentos;
        }

    }
}
