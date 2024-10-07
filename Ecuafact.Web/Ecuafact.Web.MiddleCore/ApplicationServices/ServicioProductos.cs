using Ecuafact.Web.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public static class ServicioProductos
    {

        public static async Task<DatatableList<ProductDto>> ObtenerProductosPagedAsync(string token, string search = "", int? page = null, int? pageSize = null, bool showDisabled = false, bool descendingOrder = true)
        {
            try
            {
                var serviceUrl = $"{Constants.WebApiUrl}/Products";
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

                if (showDisabled)
                {
                    qs += string.IsNullOrEmpty(qs) ? "" : "&" + $"showDisabled={showDisabled.ToString().ToLower()}";
                }

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = httpClient.GetAsync($"{serviceUrl}?{qs}").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var products = response.GetContent<List<ProductDto>>();
                        int pageCount;

                        if (!int.TryParse(response.Headers.GetValue("X-Page-Count"), out pageCount))
                        {
                            pageCount = products?.Count ?? 0;
                        }

                        IEnumerable<ProductDto> data;
                        if (descendingOrder)
                        {
                            data = products.OrderByDescending(m => m.Id);
                        }
                        else
                        {
                            data = products.OrderBy(m => m.MainCode);
                        }

                        var result = new DatatableList<ProductDto>(data) { recordsFiltered = pageCount, recordsTotal = pageCount };

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return new DatatableList<ProductDto>(ex.Message);
            }

            return new DatatableList<ProductDto>();
        }

        public static async Task<Select2List<ProductDto>> Select2ProductosAsync(string token, string filtro = null, int page = 1) {
            var products = await ObtenerProductosPagedAsync(token, filtro, page, 10, descendingOrder: false);

            var result = products.data.Select(m => new Select2ListItem<ProductDto>(m)
            {
                id = m.Id,
                text = m.MainCode + " " + m.AuxCode + " - " + m.Name
            }).ToSelect2(products.more);

            return result;
        }

        public static List<ProductDto> ObtenerProductos(string token)
        {
            var productos = new List<ProductDto>();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Products").Result;
                if (response.IsSuccessStatusCode)
                {
                    productos = response.GetContent<List<ProductDto>>();
                }
            }

            return productos;
        }

        public static async Task<HttpResponseMessage> EliminarAsync(string issuerToken, long id)
        {
            var httpClient = ClientHelper.GetClient(issuerToken);

            var response = await httpClient.DeleteAsync($"{Constants.WebApiUrl}/Products/{id}", default); ;

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            return new ApiResult(HttpStatusCode.InternalServerError)
            {
                Message = "Hubo un error al procesar el registro",
                Error = "No se pudo eliminar el registro!"
            }.ToResponseMessage();
        }

        public static List<ProductDto> ObtenerProductos(string token, string filtro)
        {
            var productos = new List<ProductDto>();
            try
            {
                string url = $"{Constants.WebApiUrl}/Products;q=Name?searchTerms={filtro}";

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = httpClient.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode) // No hubo un buen resultado
                    {
                        productos = response.GetContent<List<ProductDto>>();
                    }
                }
            }
            catch (Exception)
            {
                // Si hubo errores se envia la lista vacia
            }

            return productos;
        }

        public static async System.Threading.Tasks.Task<List<ProductDto>> ObtenerProductosAsync(string token, string filtro)
        {
            var productos = new List<ProductDto>();
            try
            {
                string url = $"{Constants.WebApiUrl}/Products?search={filtro}";

                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode) // No hubo un buen resultado
                    {
                        productos = response.GetContent<List<ProductDto>>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                // Si hubo errores se envia la lista vacia
            }

            return productos;
        }

        public static ProductDto ObtenerDetalleProductos(string token, int idproducto)
        {
            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Products/{idproducto}").Result;
                if (response != null)
                {
                    if (response.IsSuccessStatusCode) // No hubo un buen resultado
                    {
                        var result = response.GetContent<ProductDto>();
                        if (result.Id > 0)
                        {
                            return result; // Si existe el producto
                        }
                    }

                }
            }

            return new ProductDto();

        }

        public static ProductDto ObtenerDetalleProductosB(string token, string codigoPrincipal)
        {
            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync($"{Constants.WebApiUrl}/Products;q=MainCode?searchTerm={codigoPrincipal}").Result;
                if (response != null)
                {
                    if (response.IsSuccessStatusCode) // No hubo un buen resultado
                    {
                        var result = response.GetContent<ProductDto>();
                        if (result.Id > 0)
                        {
                            return result; // Si existe el producto
                        }
                    }

                }
            }

            return new ProductDto();
        }

        public static bool VerificarSiExiste(string token, string codprincipal)
        {
            var httpClient = ClientHelper.GetClient(token);
            {
                var response = httpClient.GetAsync(new Uri($"{Constants.WebApiUrl}/Products;q=MainCode?searchTerm={codprincipal}")).Result;

                if (response.IsSuccessStatusCode) // No hubo un buen resultado
                {
                    var result = response.GetContent<ProductDto>();

                    if (result.Id > 0)
                    {
                        return true; // Si existe el producto
                    }
                }
            }

            return false;
        }

        public static async Task<ProductDto> ObtenerProductoByCodeAsync(string token, string mainCode, string auxCode = null)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync(new Uri($"{Constants.WebApiUrl}/Products?mainCode={mainCode}&auxCode={auxCode}"));

                    if (response.IsSuccessStatusCode) // No hubo un buen resultado
                    {
                        var result = response.GetContent<ProductDto>();

                        if (result.Id > 0)
                        {
                            return result; // Si existe el producto
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new ProductDto();
        }

        public static async Task<ProductDto> ObtenerProductoByIdAsync(string token, object id)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(token);
                {
                    var response = await httpClient.GetAsync($"{Constants.WebApiUrl}/Products/{id}");

                    if (response.IsSuccessStatusCode) // No hubo un buen resultado
                    {
                        var result = response.GetContent<ProductDto>();

                        if (result.Id > 0)
                        {
                            return result; // Si existe el producto
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return new ProductDto();
        }

        public static async Task<HttpResponseMessage> CrearAsync(string token, ProductDto item)
        {
         
            return await ClientHelper
                .GetClient(token)
                .PostAsync($"{Constants.WebApiUrl}/Products", item.ToContent());

        }

        public static async Task<HttpResponseMessage> ActualizarAsync(string token, ProductDto item)
        {
           
                return await ClientHelper
                    .GetClient(token)
                    .PutAsync($"{Constants.WebApiUrl}/Products/{item.Id}", item.ToContent());
              
        }

        public static async Task<HttpResponseMessage> ImportarProdcutoAsync(string token, ProductImportModel item)
        {

            return await ClientHelper
                .GetClient(token)
                .PostAsync($"{Constants.WebApiUrl}/products/fileimport", item.ToContent());

        }

        public static async Task<HttpResponseMessage> DescargarproductosAsync(string token)
        {
            return await ClientHelper.GetClient(token).GetAsync($"{Constants.WebApiUrl}/products/export");
        }         
    }
}





