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
    public class ServicioFactura 
    { 

        public static List<InvoiceModel> BuscarFacturas(string issuerToken, ObjectQueryModel<List<InvoiceModel>> model)
        {
            var query = BuscarFacturasAsync(issuerToken, model.SearchTerm, model.From.ToDateTime(), model.To.ToDateTime());
            return query.Result;
        }

        public static async Task<HttpResponseMessage> GuardarFacturaAsync(string issuerToken, InvoiceRequestModel model)
        {
            var httpClient = ClientHelper.GetClient(issuerToken);
            var content = new StringContent(JsonConvert.SerializeObject(model));
            //var _model = JsonConvert.SerializeObject(model);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response;

            if (model.Id.HasValue && model.Id.Value > 0)
            {
                response = await httpClient.PutAsync($"{Constants.WebApiUrl}/Invoice/{model.Id}", content).ConfigureAwait(false);
            }
            else
            {
                response = await httpClient.PostAsync($"{Constants.WebApiUrl}/Invoice", content).ConfigureAwait(false);
            }

            return response;
        }

        public static async Task<List<InvoiceModel>> BuscarFacturaContribAsync(string token, long contributorId, string filtro, bool authorized=true, bool authorizeDate = false)
        {
            #if DEBUG
              authorized = false;
            #endif

            var documentos = new List<InvoiceModel>();

            try
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.

                string url = $"{Constants.WebApiUrl}/Contributors/{contributorId}/Invoices?search={filtro}&authorized={authorized}&authorizeDate={authorizeDate}";

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync(new Uri(url));

                    if (response.IsSuccessStatusCode)
                    {
                        documentos = await response.GetContentAsync<List<InvoiceModel>>();
                    }
                }
                
            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return documentos;
        }

        public static async Task<Select2List<InvoiceModel>> SearchDocumentsAsync(string token, string term, string type, int page = 1)
        {

            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    term = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.
                 
                string url = $"{Constants.WebApiUrl}/Invoice?search={term}&page={page}";

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync(new Uri(url));

                    if (response.IsSuccessStatusCode)
                    {                       
                        return await response.GetSelect2ListAsync<InvoiceModel>(o => new Select2ListItem<InvoiceModel>(o)
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
             

            return new Select2List<InvoiceModel>();
        }

        public static async Task<Select2List<InvoiceModel>> SearchInvoicesAsync(string token, string term, string type, int page = 1, bool authorized = true, bool authorizeDate = true)
        {
            try
            {
                int pageSize = 10;
                if (string.IsNullOrEmpty(term))
                {
                    term = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.

                string url = $"{Constants.WebApiUrl}/invoices/issuers?search={term}&page={page}&pageSize={pageSize}&authorized={authorized}&authorizeDate={authorizeDate}";
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync(new Uri(url));

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.GetSelect2ListAsync<InvoiceModel>(o => new Select2ListItem<InvoiceModel>(o)
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


            return new Select2List<InvoiceModel>();
        }

        public static async Task<List<InvoiceModel>> BuscarFacturasAsync(string token, string filtro,
             DateTime? startDate = null, DateTime? endDate = null, int pagina = 1, int? cantidad = null)
        {
            var documentos = new List<InvoiceModel>();

            try
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.

                string url = $"{Constants.WebApiUrl}/Invoice?search={filtro}&startdate={startDate?.ToString("yyyy-MM-dd")}&enddate={endDate?.ToString("yyyy-MM-dd")}&page={pagina}&pagesize={cantidad}";

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync(new Uri(url));

                    if (response.IsSuccessStatusCode)
                    {
                        documentos = await response.GetContentAsync<List<InvoiceModel>>();
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



        public static InvoiceModel BuscarFacturaPorNumero(string token, string numeroFactura)
        {
            var document = new InvoiceModel();

            try
            { 
                var httpClient = ClientHelper.GetClient(token);
                {
                   var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Invoice?search={numeroFactura}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        document = response.GetContent<InvoiceModel>();
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



        public static InvoiceModel BuscarFacturaPorId(string token, object id)
        {
            InvoiceModel document = null;

            try
            { 
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Invoice/{id}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        document = response.GetContent<InvoiceModel>();
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

        public static List<InvoiceModel> ObtenerFacturas(string token)
        {
            var documentos = new List<InvoiceModel>();
       
            var httpClient = ClientHelper.GetClient(token);
            {
               var  response = httpClient.GetAsync($"{Constants.WebApiUrl}/Invoice?status=all").Result;

                if (response.IsSuccessStatusCode)
                { 
                    documentos = response.GetContent<List<InvoiceModel>>();
                }
            }
            return documentos;
        } 


    }
}
