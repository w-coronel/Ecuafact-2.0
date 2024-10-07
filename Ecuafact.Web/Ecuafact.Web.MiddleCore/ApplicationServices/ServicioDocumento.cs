using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public static class ServicioDocumento 
    {
        public static async Task<List<DocumentModel>> BuscarDocumentosAsync(string token, DocumentosQueryModel model)
        {
            return await BuscarDocumentosAsync(token, model.SearchTerm, model.From.ToDateTime(), model.To.ToDateTime(), null, null, documentType: model.DocumentType, status: model.Status);
        }

        public static async Task<List<DocumentModel>> BuscarDocumentosAsync(string token, string filtro = null, 
            DateTime? startDate = null, DateTime? endDate = null, int? pagina = null, int? cantidad = null, string documentType = "", DocumentStatusEnum? status = null)
        {
            var documentos = new List<DocumentModel>();

            try
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.

                var qs = $"search={filtro}";

                if (!string.IsNullOrEmpty(documentType) && documentType != "0")
                {
                    qs += $"&documentType={documentType}";
                }

                if (startDate != null)
                {
                    qs += $"&startdate={startDate.Value.ToString("yyyy-MM-dd")}";
                }

                if (endDate != null)
                {
                    qs += $"&endDate={endDate.Value.ToString("yyyy-MM-dd")}";
                }

                if (status != null)
                {
                    qs += $"&status={Convert.ToInt32(status.Value)}";
                }

                if (pagina != null)
                {
                    qs += $"&page={pagina}";
                }

                if (cantidad != null)
                {
                    qs += $"&pagesize={cantidad}";
                }

                string url = $"{Constants.WebApiUrl}/documents?{qs}";

                var httpClient = ClientHelper.GetClient(token);

                var response = await httpClient.GetAsync(new Uri(url));

                if (response.IsSuccessStatusCode)
                {
                    documentos = await response.GetContentAsync<List<DocumentModel>>();
                }
            }
            catch (Exception ex)
            {
                ex.GetType();
                // Si hubo errores se envia la lista vacia
            }

            return documentos;
        }

        public static async Task<DatatableList<DocumentModel>> ObtenerDocumentosPagedAsync (string token, string filtro = null, DateTime? startDate = null, DateTime? endDate = null, 
            int? pagina = null, int? cantidad = null, string documentType = "", DocumentStatusEnum? status = null, string establishmentCode = null, string issuePointCode = null, 
            bool descendingOrder = true, long column = 2)
        {
            try
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.

                var qs = $"search={filtro}";

                if (!string.IsNullOrEmpty(documentType) && documentType != "0")
                {
                    qs += $"&documentType={documentType}";
                }
                if (startDate != null)
                {
                    qs += $"&startdate={startDate.Value.ToString("yyyy-MM-dd")}";
                }
                if (endDate != null)
                {
                    qs += $"&endDate={endDate.Value.ToString("yyyy-MM-dd")}";
                }
                if (status != null)
                {
                    qs += $"&status={Convert.ToInt32(status.Value)}";
                }
                if (pagina != null)
                {
                    qs += $"&page={pagina}";
                }
                if (cantidad != null)
                {
                    qs += $"&pagesize={cantidad}";
                }
                if (!string.IsNullOrWhiteSpace(establishmentCode) && establishmentCode != "0")
                {
                    qs += $"&establishmentCode={establishmentCode}";
                }
                if (!string.IsNullOrWhiteSpace(issuePointCode))
                {
                    qs += $"&issuePointCode={issuePointCode}";
                }

                string url = $"{Constants.WebApiUrl}/documents?{qs}";
                var httpClient = ClientHelper.GetClient(token);
                var response = await httpClient.GetAsync(new Uri(url));
                if (response.IsSuccessStatusCode)
                {
                    var document = response.GetContent<List<DocumentModel>>();
                    int pageCount;

                    if (!int.TryParse(response.Headers.GetValue("X-Page-Count"), out pageCount))
                    {
                        pageCount = document?.Count ?? 0;
                    }

                    IEnumerable<DocumentModel> data;                  
                    data = document.OrderByColumn(descendingOrder, column);

                    var result = new DatatableList<DocumentModel>(data) { recordsFiltered = pageCount, recordsTotal = pageCount };

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new DatatableList<DocumentModel>(ex.Message);
            }

            return new DatatableList<DocumentModel>();
        }


        public static async Task<OperationResult> EmitirDocumentoAsync(string issuerToken, object id, string reason)
        { 
            var response = await ClientHelper
                    .GetClient(issuerToken)
                    .PostAsync($"{Constants.WebApiUrl}/Documents/{id}/Send?reason={reason}");

            if (response.IsSuccessStatusCode)
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK);
            }

            return response.GetContent<OperationResult>();
        }

        public static async Task<OperationResult> EliminarDocumentoAsync(string issuerToken, object id, string reason = null)
        {
            try
            {

                if (id == null)
                {
                    return new OperationResult(false, System.Net.HttpStatusCode.NotFound);
                }

                var httpClient = ClientHelper.GetClient(issuerToken);

                var response =
                    await httpClient.DeleteAsync($"{Constants.WebApiUrl}/Documents/{id}?reason={reason}");
             
                return response.GetContent<OperationResult>();

            }
            catch (Exception ex)
            {
                return new OperationResult(ex);
            }
            
        }

        public static async Task<OperationResult> EnviarCorreoAsync(string issuerToken, string email, string id)
        {
            if (id == null)
            {
                return await Task.FromResult(new OperationResult(false, System.Net.HttpStatusCode.NotFound));
            }

            var httpClient = ClientHelper.GetClient(issuerToken);

            var response =
                await httpClient.PostAsync($"{Constants.WebApiUrl}/Documents/{id}/Email?to={email}");

            return await response.GetContentAsync<OperationResult>();
        }       

        public static async Task<HttpResponseMessage> DescargarDocumentosAsync(string token, string filtro = null, DateTime? startDate = null, 
            DateTime? endDate = null, string documentType = "", DocumentStatusEnum? status = null)
        {
            try
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.

                var qs = $"search={filtro}";

                if (!string.IsNullOrEmpty(documentType) && documentType != "0")
                {
                    qs += $"&documentType={documentType}";
                }

                if (startDate != null)
                {
                    qs += $"&startdate={startDate.Value.ToString("yyyy-MM-dd")}";
                }

                if (endDate != null)
                {
                    qs += $"&endDate={endDate.Value.ToString("yyyy-MM-dd")}";
                }

                if (status != null)
                {
                    qs += $"&status={Convert.ToInt32(status.Value)}";
                }

                return await ClientHelper.GetClient(token).GetAsync($"{Constants.WebApiUrl}/documents/export?{qs}");
            }
            catch (Exception ex)
            {
                ex.GetType();
                return new HttpResponseMessage();
            }

            
        }

        public static IEnumerable<DocumentModel> OrderByColumn(this IEnumerable<DocumentModel> document, bool descending, long columna)
        {
            if (descending)
            {
                switch (columna)
                {
                    case 0: return document.OrderByDescending(c => c.Sequential);
                    case 1: return document.OrderByDescending(c => c.ContributorName);
                    case 3: return document.OrderByDescending(c => c.Total);
                    case 4: return document.OrderByDescending(c => c.Status);
                    default: return document.OrderByDescending(c => c.IssuedOn);
                }
            }
            else {
                switch (columna)
                {
                    case 0: return document.OrderBy(c => c.Sequential);
                    case 1: return document.OrderBy(c => c.ContributorName);
                    case 3: return document.OrderBy(c => c.Total);
                    case 4: return document.OrderBy(c => c.Status);
                    default: return document.OrderBy(c => c.IssuedOn);
                }
            }
        }
    }
}
