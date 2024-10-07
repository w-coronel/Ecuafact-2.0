using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Ecuafact.Web.Models;
using Newtonsoft.Json;
using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using System.Threading.Tasks;
using System.Net;
using Ecuafact.Web.Filters;
using Ecuafact.Web.Domain.Services;
using System.IO;
using Ecuafact.Web.Reporting;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize]
    public class ProductoController : AppControllerBase
    {

        //[HttpGet]
        public ActionResult Index()
        {
            var model = ServicioProductos.ObtenerProductos(IssuerToken);

            return this.View(model);
        }




        public ActionResult Nuevo()
        {
            return PartialView("_mantProductoPartial", new ProductDto());
        }

        public ActionResult Importar()
        {
            return PartialView("ImportProduct", new ProductDto());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="draw">Pagina</param>
        /// <param name="start">Inicio</param>
        /// <param name="length">Cantidad de registros x pagina</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetDatatableAsync()
        {
            int draw = Convert.ToInt32(Request["draw"] ?? "1");
            int start = Convert.ToInt32(Request["start"] ?? "1");
            int length = Convert.ToInt32(Request["length"] ?? "0");
            string search = Request["search[value]"];
            long columna = Convert.ToInt32(Request["order[0][column]"]);
            string order = Request["order[0][dir]"];

            var page = 1;
            if (start > 0 && length > 0)
            {
                page = Convert.ToInt32(start / length) + 1;
            }

            var model = await ServicioProductos.ObtenerProductosPagedAsync(IssuerToken, search, page, length, false, order == "desc");

            var tiposProducto = SessionInfo.Catalog.ProductTypes.ToList();
            var impuestosIVA = SessionInfo.Catalog.IVARates.ToList();
            var impuestosICE = SessionInfo.Catalog.IVARates.ToList();

            if (model.length > 0)
            {
                model.data.ToList().ForEach(p => {
                    p.Tipo = tiposProducto.Find(tipo => p.ProductTypeId == tipo.Id).Name;
                    p.Iva = impuestosIVA.Find(iva => p.IvaRateId == iva.Id).Name;
                    p.Ice = impuestosICE.Find(ice => p.IceRateId == ice.Id).Name;

                });
            }

            // Solo es una validacion del request [es un ID request] 
            // debe ser el mismo que el requerido.
            model.draw = draw;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> SearchProductAsync(string q = null, int page = 1)
        {
            Select2List<ProductDto> result = null;

            try
            {
                result = await ServicioProductos.Select2ProductosAsync(IssuerToken, q, page);
            }
            catch (Exception ex)
            {
                SessionInfo.LogException(ex);
                result = new Select2List<ProductDto>();
            }

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public async Task<JsonResult> GetProductByCode(string code)
        {
            ProductDto result;
            try
            {
                result = await ServicioProductos.ObtenerProductoByCodeAsync(IssuerToken, code);
            }
            catch (Exception ex)
            {
                SessionInfo.LogException(ex);
                result = new ProductDto();
            }

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public async Task<JsonResult> GetProductById(string id)
        {
            ProductDto result;
            try
            {
                result = await ServicioProductos.ObtenerProductoByIdAsync(IssuerToken, id);

            }
            catch (Exception ex)
            {
                SessionInfo.LogException(ex);
                result = new ProductDto();
            } 

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public async Task<ActionResult> Editar(long id)
        {

            var model = await ServicioProductos.ObtenerProductoByIdAsync(IssuerToken, id);

            return PartialView("_mantProductoPartial", model);
        }

        public async Task<HttpResponseMessage> EliminarAsync(long id)
        {
            return await ServicioProductos.EliminarAsync(IssuerToken, id);

        }

        [HttpPost]
        [HandleJsonException]
        public async Task<JsonResult> GuardarAsync(ProductDto model)
        {
            try
            {
                model.IsEnabled = (model.Status == 1);
                HttpResponseMessage response = null;

                if (model.Id > 0)
                {
                    response = await ServicioProductos.ActualizarAsync(IssuerToken, model);
                }
                else
                {
                    response = await ServicioProductos.CrearAsync(IssuerToken, model);
                }

                var text = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var product = JsonConvert.DeserializeObject<ProductDto>(text);
                    return new JsonResult
                    {
                        Data = new
                        {
                            id = product.Id,
                            result = product,
                            error = default(object),
                            status = response.StatusCode,
                            message = "Producto Guardado con exito!"
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        ContentType = "application/json"
                    };
                }

                var errors = JsonConvert.DeserializeObject<OperationResult>(text);

                return new JsonResult
                {
                    Data = new
                    {
                        id = 0,
                        result = default(object),
                        error = errors,
                        status = response.StatusCode,
                        message = errors.UserMessage ?? "Error en los datos del producto!"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentType = "application/json"
                };
            }
            catch (Exception ex)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        id = 0,
                        result = default(object),
                        error = GetValidationError(),
                        status = HttpStatusCode.InternalServerError,
                        statusText = ex.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentType = "application/json"
                };
            }  
        }

        [HttpPost]
        [HandleJsonException]
        public async Task<JsonResult> ImportarProductoAsync(ProductImportModel model)
        {
            var errors = "Los datos enviados son inválidos!";
            try
            {
                HttpResponseMessage response = null;
                if (model != null)
                {
                    response = await ServicioProductos.ImportarProdcutoAsync(IssuerToken, model);
                    var text = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var product = JsonConvert.DeserializeObject<bool>(text);
                        return new JsonResult
                        {
                            Data = new
                            {
                                result = product,
                                error = default(object),
                                status = response.StatusCode,
                                message = "Importacion de productos exitosamente!"
                            },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            ContentType = "application/json"
                        };
                    }

                    errors = JsonConvert.DeserializeObject<OperationResult>(text).UserMessage ?? "Error al importar productos!";

                }
            }
            catch (Exception ex)
            {
                errors = ex.Message;
            }

           
            return new JsonResult
            {
                Data = new
                {
                    result = false,
                    error = errors,
                    status = HttpStatusCode.InternalServerError,
                    message = errors
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentType = "application/json"
            };

        }

        public async Task<ActionResult> Descargar( )
        {
            // Verificamos si es recibido o emitido:
            var result = await ServicioProductos.DescargarproductosAsync(IssuerToken);

            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStreamAsync();

                return File(content, result.Content.Headers.ContentType.ToString(), result.Content.Headers.ContentDisposition.FileName);
            }

            return HttpNotFound(result.ReasonPhrase);

            //return HttpNotFound("El documento no existe!");
        }
    }
}
