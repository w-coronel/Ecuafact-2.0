using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Filters;
using Ecuafact.WebAPI.Http;
using Ecuafact.WebAPI.Models;
using X.PagedList;

namespace Ecuafact.WebAPI.Controllers
{
    /// <summary>
    /// PRODUCTOS Y SERVICIOS
    /// </summary>
    [EcuafactExpressAuthorize]
    [DisplayName("Productos")]
    public class ProductsController : ApiController
    {
        private readonly IProductsService _productsService;
        private readonly ICatalogsService _catalogsService;

        /// <summary>
        /// /
        /// </summary>
        /// <param name="productsService"></param>
        public ProductsController(IProductsService productsService, ICatalogsService catalogsService)
        {
            _productsService = productsService;
            _catalogsService = catalogsService;
        } 

        /// <summary>
        /// Buscar Productos
        /// </summary>
        /// <remarks>
        ///     Permite buscar los productos existentes
        /// </remarks>
        /// <param name="search">Busqueda por los datos del producto</param>
        /// <param name="productType">Buscar por tipo de Producto</param>
        /// <param name="productCode">Buscar por Codigo del Producto</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet, Route("products")]
        public async Task<IPagedList<ProductDto>> SearchProducts(string search = null, ProductTypeEnum? productType = null, string productCode = null, int? page = 1, int? pageSize = null, bool showDisabled = false)
        {
            try
            {
                var issuer = GetAuthenticatedIssuer();
                var products = _productsService.GetIssuerProducts(issuer.Id);

                if (!showDisabled)
                {
                    products = products.AsNoTracking().Where(model => model.IsEnabled);
                }

                if (productType != null && productType != ProductTypeEnum.All)
                {
                    switch (productType.Value)
                    {
                        case ProductTypeEnum.Goods:
                            products = products.AsNoTracking().Where(model => model.ProductTypeId == (short)ProductTypeEnum.Goods);
                            break;
                        case ProductTypeEnum.Services:
                            products = products.AsNoTracking().Where(model => model.ProductTypeId == (short)ProductTypeEnum.Services);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(productCode))
                {
                    var mainCode = productCode;string auxCode = null;
                    if (productCode.Contains("."))
                    {
                        var codes = productCode.Split('.');
                        mainCode = codes[0];
                        auxCode = codes[1];
                    }

                    products = products.AsNoTracking().Where(model => model.MainCode.Contains(mainCode)
                        && (string.IsNullOrEmpty(auxCode) || model.AuxCode.Contains(auxCode)));
                }

                if (!string.IsNullOrEmpty(search))
                {
                    products = products.AsNoTracking().Where(model => model.MainCode.Contains(search)
                        || model.AuxCode.Contains(search)
                        || model.Name.Contains(search));
                }
                 
                products = products.AsNoTracking().OrderBy(model => model.MainCode).ThenBy(model => model.AuxCode);

                pageSize = pageSize ?? 10;
                page = page ?? 1;

                if (pageSize < 1)
                {
                    pageSize = 1;
                }

                if (page < 1)
                {
                    page = 1;
                }

                var _products =  await products.ToPagedListAsync(page.Value, pageSize.Value);
                return  _products.Select(m => m.ToProductDto());

            }
            catch (Exception ex)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.InternalServerError, ex.ToString(), $"Ocurrio un error al realizar la consulta: {ex.Message}");
            }
        }

        /// <summary>
        /// Crear Producto masivamente 
        /// </summary>
        /// <remarks>
        ///     Permite crear masivamente productos con los datos especificados.
        /// </remarks>
        /// <returns></returns>
        [HttpPost, Route("products/fileimport")]
        public HttpResponseMessage Products(ProductImportModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Hubo un error al importar porductos. {error?.ErrorMessage}", error?.Exception?.ToString());
            }

            var issuer = GetAuthenticatedIssuer();
            model.IssuerId = (long)issuer?.Id;
            var ivaRates = _catalogsService.GetVatRates().Where(o => o.IsEnabled).ToList();
            var addProductResult = _productsService.ImportProduct(model.ImportProduct(ivaRates));

            if (!addProductResult.IsSuccess)
            {
                return Request.BuildHttpErrorResponse((HttpStatusCode)addProductResult.StatusCode, addProductResult.DevMessage, addProductResult.UserMessage);
            }            

            var response = Request.CreateResponse(HttpStatusCode.Created, true);

            return response;
        }

        /// <summary>
        /// Exporta los productos en formato excel
        /// </summary>
        /// <remarks>
        ///     Permite exporta los productos en formato excel.
        /// </remarks>
        /// <returns></returns>
        [HttpGet, Route("products/export")]
        public async Task<HttpResponseMessage> ExportProduct()
        {
            try
            {
                var issuer = GetAuthenticatedIssuer();
                var product = await _productsService.GetIssuerProducts(issuer.Id, true).ToListAsync();
                if (product.Count > 0)
                {
                    // Ahora generamos el archivo segun el tipo especificado:
                    HttpResponseMessage result = this.Request.CreateResponse(HttpStatusCode.OK);
                    var stream = new MemoryStream(product.ToExcelProduct());
                    result.Content = new StreamContent(stream);
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/excel");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = $"{issuer.RUC}_Productos.xlsx"
                    };

                    return result;
                }
                return Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, $"Error al descargar la exportación de productos]");
            }
            catch (HttpResponseException ex)
            {
                return ex.Response;
            }
            catch (Exception ex)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.InternalServerError, $"{ex.Message} {ex.InnerException?.Message}", $"Hubo un error al procesar la descarga: {ex.Message}");
            }
        }


        /// <summary>
        /// Obtener Producto
        /// </summary>
        /// <remarks>
        ///     Devuelve el producto por el ID especificado
        /// </remarks>
        /// <param name="id">Id del producto</param>
        /// <returns>Return a product of Issuer by Id</returns>
        [HttpGet, Route("products/{id}")]
        public ProductDto GetProduct(long id)
        { 
            var issuer = GetAuthenticatedIssuer();

            var product = _productsService.GetIssuerProduct(id, issuer.Id);

            if (product == null)
            {
                throw Request.BuildHttpErrorException(HttpStatusCode.NotFound, "Producto no existe", "El producto solicitado no existe");
            }

            return product.ToProductDto();
        }

        /// <summary>
        /// Crear Producto
        /// </summary>
        /// <remarks>
        ///     Permite crear un nuevo producto con los datos especificados.
        /// </remarks>
        /// <returns></returns>
        [HttpPost, Route("products")]
        public HttpResponseMessage PostProduct(ProductRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Hubo un error al guardar el producto. {error?.ErrorMessage}", error?.Exception?.ToString());
            }

            var issuer = GetAuthenticatedIssuer();
            var newProductToSave = model.ToProduct();
            newProductToSave.IssuerId = issuer.Id;

            var addProductResult = _productsService.AddProduct(newProductToSave);

            if (!addProductResult.IsSuccess)
            {
                return Request.BuildHttpErrorResponse((HttpStatusCode)addProductResult.StatusCode, addProductResult.DevMessage, addProductResult.UserMessage);
            }

            var response = Request.CreateResponse(HttpStatusCode.Created, addProductResult.Entity.ToProductDto());

            return response;
        }
              

        /// <summary>
        /// Actualizar Producto
        /// </summary>
        /// <remarks>
        ///     Permite modificar el producto con los datos especificados.
        /// </remarks>
        /// <param name="id">Id del Producto</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut, Route("products/{id}")]
        public HttpResponseMessage PutProduct(long id, ProductRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.GetError();
                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, $"Hubo un error al guardar el producto. {error?.ErrorMessage}", error?.Exception?.ToString());
            }

            var issuer = GetAuthenticatedIssuer();

            var product = _productsService.GetIssuerProduct(id, issuer.Id);

            if (product == null)
            {
                return Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, "Producto no existe", "El producto solicitado no existe");
            }

            var updatedProduct = _productsService.UpdateProduct(model.ToProduct(product));

            if (!updatedProduct.IsSuccess)
            {
                return Request.BuildHttpErrorResponse(updatedProduct.StatusCode, updatedProduct.DevMessage, updatedProduct.UserMessage);
            }

            var response = Request.CreateResponse(HttpStatusCode.Created, updatedProduct.Entity.ToProductDto());

            return response;
        }

        /// <summary>
        /// Eliminar Producto
        /// </summary>
        /// <remarks>
        ///     Permite eliminar el producto especificado.
        /// </remarks>
        /// <param name="id">Id del Producto</param>
        /// <returns></returns>
        [HttpDelete, Route("products/{id}")]
        public HttpResponseMessage DeleteProduct(long id)
        {
            var issuer = GetAuthenticatedIssuer();
            var Product = _productsService.GetIssuerProduct(id, issuer.Id);

            if (Product == null)
                throw new HttpResponseException(Request.BuildHttpErrorResponse(HttpStatusCode.NotFound, "Producto no existe", "El producto solicitado no existe"));

            Product.IsEnabled = false;

            var updatedProduct = _productsService.UpdateProduct(Product);

            if (!updatedProduct.IsSuccess)
            {
                return Request.BuildHttpErrorResponse(updatedProduct.StatusCode, updatedProduct.DevMessage, updatedProduct.UserMessage);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private IssuerDto GetAuthenticatedIssuer()
        {
            var issuerAuth = HttpContext.Current.Session.GetAuthenticatedIssuerSession();

            return issuerAuth;
        }

       

        

    }
}
