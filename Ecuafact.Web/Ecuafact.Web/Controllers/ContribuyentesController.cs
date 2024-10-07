using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Ecuafact.Web.Filters;
using System.Collections.Generic;
using System;
using Ecuafact.Web.Domain.Services;
using System.Linq;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize]

    public class ContribuyentesController : AppControllerBase
    {
        protected override string SessionName => $"{SessionInfo.CONTRIBUYENTES_SESSION}{DateTime.Now.ToString("yyyyMMddhhmms").Substring(0, 13)}";
        // GET: Contribuyentes
        //[HttpGet]
        public ActionResult Index()
        {

            // Lista a todos los contribuyentes 
            var model = Session[SessionName] as IEnumerable<ContributorDto>;
            if (model == null) 
            {
                model = new List<ContributorDto>();
            }

            return this.View(model);
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

            var model = await ServicioContribuyentes.ObtenerContribuyentesPagedAsync(IssuerToken, search, page, length, false, order == "desc", columna);

            // Solo es una validacion del request [es un ID request] 
            // debe ser el mismo que el requerido.
            model.draw = draw;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> BuscarAsync(string uid, string token)
        {
            
            try
            {
                // validamos el token
                if (token == (Session["_ReferralToken"] as string))
                {
                    return Json(new OperationResult(false, HttpStatusCode.Unauthorized), JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(uid))
                {
                    return Json(new OperationResult(false, HttpStatusCode.BadRequest), JsonRequestBehavior.AllowGet);
                }


                var result = await ServicioContribuyentes.ObtenerContribuyenteAsync(IssuerToken, uid);

                if (result != null)
                {
                    return Json(new OperationResult<ContributorDto>(true, HttpStatusCode.OK, result), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Entonces determino si es cedula o ruc y busco el dato adecuado
                    result = await ServicioContribuyentes.ObtenerContribuyenteAsync(IssuerToken, uid.Length == 10 ? $"{uid}001" : uid.Substring(0, 10));

                    if (result != null)
                    {
                        return Json(new OperationResult<ContributorDto>(true, HttpStatusCode.OK, result), JsonRequestBehavior.AllowGet);
                    }
                }

                // Si no existe el registro el contribuyente se obtiene de la base de datos del sri.

                var data = await ServicioSRI.FindByRucAsync(IssuerToken, uid.Length == 10 ? $"{uid}001" : uid);
                
                if (data.IsSuccess && data.Entity != null)
                {
                    return Json(new OperationResult<ContributorDto>(true, HttpStatusCode.OK, data.Entity.ToContributor()), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResult(ex), JsonRequestBehavior.AllowGet);
            }

            return Json(new OperationResult(false, HttpStatusCode.BadRequest), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> SearchContribAsync(string q = null, int page = 1)
        {
            Select2List<ContributorDto> result = null;

            try
            {
                result = await ServicioContribuyentes.Select2ContribuyentesAsync(IssuerToken, q, page);
            }
            catch (Exception ex)
            {
                SessionInfo.Notifications.Add(ex.ToString(), SessionInfo.AlertType.Error);

                result = new Select2List<ContributorDto>();
            }

            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public async Task<ActionResult> GetContributorsAsync()
        {
            var model = await ServicioContribuyentes.SearchContribuyentesAsync(IssuerToken);

            Session[SessionName] = model;

            return PartialView("_listaContribuyentesPartial", model);
        }

        public ActionResult Crear()
        { 
            var model = new ContributorDto();
            return View("Crear", model);
        }


        public  ActionResult Nuevo()
        { 

            ViewBag.IdContrib = 0;
            var model = new ContributorDto();

            return PartialView("_mantContribuyentePartial", model);
        }

        public ActionResult Importar()
        {
            return PartialView("importContributor", new ContributorDto());
        }


        public async Task<ActionResult> Editar(int id)
        { 
            var model = await ServicioContribuyentes.ObtenerContribuyentePorId(IssuerToken, id);
            ViewBag.IdContrib = id;

            return PartialView("_mantContribuyentePartial", model);
        }


        [HttpPost]
        [HandleJsonException]
        public async Task<HttpResponseMessage> EliminarAsync(long? id)
        {
            return await ServicioContribuyentes.EliminarAsync(IssuerToken, id.Value);
        }


        [HttpPost]
        [HandleJsonException]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<JsonResult> ActualizarAsync(ContributorRequestModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                HttpResponseMessage response;

                if (model.Id > 0)
                {
                    response = await ServicioContribuyentes.EditarAsync(IssuerToken, model.Id, model);
                }
                else
                {
                    response = await ServicioContribuyentes.CrearAsync(IssuerToken, model);
                }
                
                var text = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    ClearSession();

                    var doc = JsonConvert.DeserializeObject<ContributorDto>(text);
                    SessionInfo.Notifications.Add($"Registro del Contribuyente {doc.Identification} {doc.BussinesName} ha sido guardado. ", Url.Action("Index"), SessionInfo.AlertType.Success, "fa fa-users");

                    return new JsonResult
                    {
                        Data = new
                        {
                            id = doc.Id,
                            result = doc,
                            error = default(object),
                            status = response.StatusCode,
                            statusText = "Registro Guardado"
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        ContentType = "application/json"
                    };
                }
                else
                {
                    var error = JsonConvert.DeserializeObject<OperationResult>(text);
                    SessionInfo.Notifications.Add(text, Url.Action("Index"), SessionInfo.AlertType.Error, "fa fa-user-times");

                    return new JsonResult
                    {
                        Data = new
                        {
                            id = 0,
                            result = default(object),
                            error = error,
                            status = response.StatusCode,
                            statusText = error.UserMessage ?? "Error al guardar el registro"
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        ContentType = "application/json"
                    };
                }
            }
            else
            {
                var error = GetValidationError();
                return new JsonResult
                {
                    Data = new
                    {
                        id = 0,
                        result = default(object),
                        error = error,
                        status = HttpStatusCode.BadRequest,
                        statusText = error.Message
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    ContentType = "application/json"
                };
            }
        }

        [HttpPost]
        [HandleJsonException]
        public async Task<JsonResult> ImportarContributorAsync(ContributorImportModel model)
        {
            var errors = "Los datos enviados son inválidos!";
            try
            {
                HttpResponseMessage response = null;
                if (model != null)
                {
                    response = await ServicioContribuyentes.ImportarContributorAsync(IssuerToken, model);
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
                                message = "Importacion de contribuyentes exitosamente!"
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

        public async Task<ActionResult> Descargar()
        {
            // Verificamos si es recibido o emitido:
            var result = await ServicioContribuyentes.DescargarClientesAsync(IssuerToken);

            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStreamAsync();

                return File(content, result.Content.Headers.ContentType.ToString(), result.Content.Headers.ContentDisposition.FileName);
            }

            return HttpNotFound(result.ReasonPhrase);
            
        }

    }
}