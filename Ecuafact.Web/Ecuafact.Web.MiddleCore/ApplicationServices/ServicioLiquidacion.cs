using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecuafact.Web.Domain.Entities;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Linq;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public class ServicioLiquidacion
    {

        public static List<SettlementModel> BuscarLiquidacions(string issuerToken, ObjectQueryModel<List<SettlementModel>> model)
        {
            var query = BuscarLiquidacionsAsync(issuerToken, model.SearchTerm, model.From.ToDateTime(), model.To.ToDateTime());
            return query.Result;
        }

        public static async Task<HttpResponseMessage> GuardarLiquidacionAsync(string issuerToken, SettlementRequestModel model)
        {
            var httpClient = ClientHelper.GetClient(issuerToken);

            var content = new StringContent(JsonConvert.SerializeObject(model));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response;

            if (model.Id.HasValue && model.Id.Value > 0)
            {
                response = await httpClient.PutAsync($"{Constants.WebApiUrl}/Settlement/{model.Id}", content).ConfigureAwait(false);
            }
            else
            {
                response = await httpClient.PostAsync($"{Constants.WebApiUrl}/Settlement", content).ConfigureAwait(false);
            }

            return response;
        }

        public static async Task<List<SettlementModel>> BuscarLiquidacionContribAsync(string token, long contributorId, string filtro, bool authorized = true)
        {
#if DEBUG
            authorized = false;
#endif

            var documentos = new List<SettlementModel>();

            try
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.

                string url = $"{Constants.WebApiUrl}/Contributors/{contributorId}/Settlements?search={filtro}&authorized={authorized}";

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync(new Uri(url));

                    if (response.IsSuccessStatusCode)
                    {
                        documentos = await response.GetContentAsync<List<SettlementModel>>();
                    }
                }

            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return documentos;
        }

        public static async Task<Select2List<SettlementModel>> SearchDocumentsAsync(string token, string term, string type, int page = 1)
        {

            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    term = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.

                string url = $"{Constants.WebApiUrl}/Settlement?search={term}&page={page}";

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync(new Uri(url));

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.GetSelect2ListAsync<SettlementModel>(o => new Select2ListItem<SettlementModel>(o)
                        {
                            id = o.Id,
                            text = $"FACTURA {o.DocumentNumber} {o.IssuedOn:dd/MM/yyyy} | {o.ContributorIdentification} {o.ContributorName}"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                // Si hubo errores se envia la lista vacia
            }


            return new Select2List<SettlementModel>();
        }

        public static async Task<List<SettlementModel>> BuscarLiquidacionsAsync(string token, string filtro,
             DateTime? startDate = null, DateTime? endDate = null, int pagina = 1, int? cantidad = null)
        {
            var documentos = new List<SettlementModel>();

            try
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.

                string url = $"{Constants.WebApiUrl}/Settlement?search={filtro}&startdate={startDate?.ToString("yyyy-MM-dd")}&enddate={endDate?.ToString("yyyy-MM-dd")}&page={pagina}&pagesize={cantidad}";

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync(new Uri(url));

                    if (response.IsSuccessStatusCode)
                    {
                        documentos = await response.GetContentAsync<List<SettlementModel>>();
                    }
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                // Si hubo errores se envia la lista vacia
            }

            return documentos;
        }



        public static SettlementModel BuscarLiquidacionPorNumero(string token, string numeroLiquidacion)
        {
            var document = new SettlementModel();

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Settlement?search={numeroLiquidacion}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        document = response.GetContent<SettlementModel>();
                    }
                }
            }
            catch (Exception ex)
            {
                // Do Nothing
                ex.ToString();
            }

            return document;
        }



        public static SettlementModel BuscarLiquidacionPorId(string token, object id)
        {
            SettlementModel document = null;

            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Settlement/{id}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        document = response.GetContent<SettlementModel>();
                    }
                }
            }
            catch (Exception ex)
            {
                // Do Nothing
                ex.ToString();
            }

            return document;
        }

        public static List<SettlementModel> ObtenerLiquidacions(string token)
        {
            var documentos = new List<SettlementModel>();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Settlement?status=all").Result;

                if (response.IsSuccessStatusCode)
                {
                    documentos = response.GetContent<List<SettlementModel>>();
                }
            }
            return documentos;
        }


    }
}
