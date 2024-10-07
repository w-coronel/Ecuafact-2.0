using System;
using System.Collections.Generic;

using Ecuafact.Web.Domain.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net;
using X.PagedList;
using System.Linq;
using Ecuafact.Web.Domain.Services;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public static class ServicioContribuyentes
    {
          
        public static async Task<DatatableList<ContributorDto>> ObtenerContribuyentesPagedAsync(string token, string search = "", int? page = null, int? pageSize = null, bool? showDisabled = false, bool descendingOrder = false, long columna = 0)
        {
            try
            {
                var qs = "";

                qs += $"search={search}";

                if (page != null)
                {
                    qs += string.IsNullOrEmpty(qs) ? "" : "&" + $"page={page}";
                }

                if (pageSize != null)
                {
                    qs += string.IsNullOrEmpty(qs) ? "" : "&" + $"pageSize={pageSize}";
                }

                qs += string.IsNullOrEmpty(qs) ? "" : "&" + $"showDisabled={showDisabled?.ToString()?.ToLower() ?? "all"}";

                var httpClient = ClientHelper.GetClient(token);

                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/contributors?{qs}").Result;

                if (response.IsSuccessStatusCode)
                {
                    var contributors = await response.GetContentAsync<List<ContributorDto>>();
                    int pageCount;

                    if (!int.TryParse(response.Headers.GetValue("X-Page-Count"), out pageCount))
                    {
                        pageCount = contributors?.Count ?? 0;
                    }
                    
                    IEnumerable<ContributorDto> data;                   

                    switch (columna.ToString())
                    {
                        case "0": data = descendingOrder ? contributors.OrderByDescending(c => c.EmailAddresses): contributors.OrderBy(m => m.EmailAddresses); break;
                        case "1": data = descendingOrder ? contributors.OrderByDescending(c => c.IdentificationTypeId) : contributors.OrderBy(m => m.Identification); break;
                        case "2": data = descendingOrder ? contributors.OrderByDescending(c => c.BussinesName) : contributors.OrderBy(m => m.BussinesName); break;
                        case "3": data = descendingOrder ? contributors.OrderByDescending(c => c.TradeName) : contributors.OrderBy(m => m.TradeName); break;
                        case "4": data = descendingOrder ? contributors.OrderByDescending(c => c.Address) : contributors.OrderBy(m => m.Address); break;
                        case "5": data = descendingOrder ? contributors.OrderByDescending(c => c.Phone) : contributors.OrderBy(m => m.Phone); break;
                        default : data = descendingOrder ? contributors.OrderByDescending(c => c.Id) : contributors.OrderBy(m => m.Id); break;
                    }
                    var result = new DatatableList<ContributorDto>(data) { recordsFiltered = pageCount, recordsTotal = pageCount };

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new DatatableList<ContributorDto>(ex.Message);
            }

            return new DatatableList<ContributorDto>();
        }
       
        public static async Task<List<ContributorDto>> SearchContribuyentesAsync(string token, string search = "", int? page = null, int? pageSize = null)
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

                var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/contributors?{qs}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<List<ContributorDto>>();
                }
            }
            catch { }

            return new List<ContributorDto>();
        }

        public static async Task<Select2List<ContributorDto>> Select2ContribuyentesAsync(string token, string filtro = null, int page = 1)
        {
            var contributors = await ObtenerContribuyentesPagedAsync(token, filtro, page, 10, descendingOrder: false);

            var result = contributors.data.Select(m => new Select2ListItem<ContributorDto>(m)
            {
                id = m.Id,
                text = m.Identification + " - " + m.BussinesName
            }).ToSelect2(contributors.more);

            return result;
        }

         

        public static async Task<ContributorDto> ObtenerContribuyentePorId(string token, long id)
        {
            var contribuyente = new ContributorDto(true);

            string url = $"{Constants.WebApiUrl}/contributors/{id}";

            var httpClient = ClientHelper.GetClient(token);

            var response = await httpClient.GetAsync(new Uri(url));

            if (response.IsSuccessStatusCode)
            {
                contribuyente = await response.GetContentAsync<ContributorDto>();
            }


            return contribuyente;
        }
         

        public static async Task<HttpResponseMessage> CrearAsync(string token, ContributorRequestModel contribuyente)
        {
            var httpClient = ClientHelper.GetClient(token);
            return await httpClient.PostAsync($"{Constants.WebApiUrl}/contributors", contribuyente.ToContent());

        }


        public static async Task<HttpResponseMessage> EditarAsync(string token, long id, ContributorRequestModel contribuyente)
        {
            var httpClient = ClientHelper.GetClient(token);
            return await httpClient.PutAsync($"{Constants.WebApiUrl}/contributors/{id}", contribuyente.ToContent()); ;
        }

        public static async Task<HttpResponseMessage> EliminarAsync(string token, long id)
        {
            var httpClient = ClientHelper.GetClient(token);

            return await httpClient.DeleteAsync($"{Constants.WebApiUrl}/contributors/{id}", default); ;
             
        }

        public static async Task<ContributorDto> ObtenerContribuyenteAsync(string token, object id)
        {
            var httpClient = ClientHelper.GetClient(token);

            var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/Contributors/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.GetContentAsync<ContributorDto>();
            }

            return await Task.FromResult<ContributorDto>(default);
        }

        public static async Task<HttpResponseMessage> ImportarContributorAsync(string token, ContributorImportModel item)
        {

            return await ClientHelper
                .GetClient(token)
                .PostAsync($"{Constants.WebApiUrl}/contributors/fileimport", item.ToContent());

        }

        public static async Task<HttpResponseMessage> DescargarClientesAsync(string token)
        {
            return await ClientHelper.GetClient(token).GetAsync($"{Constants.WebApiUrl}/contributors/export");
        }

    }
}
