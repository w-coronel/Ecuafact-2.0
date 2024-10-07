using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Filters;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Newtonsoft.Json;
using System.Net;
using System.Web.Mvc;

namespace Ecuafact.Web.Controllers
{
    [ExpressAuthorize(Roles = "Issuer|Admin")]
    public class NotaVentaController : DocumentControllerBase
    {

        protected override string SessionName => SessionInfo.NOTAVENTA_SESSION;
        protected override string DocumentType => "02";

        protected override object DocumentById(string id)
        {
            SalesNoteRequestModel model = null;

            if (!string.IsNullOrEmpty(id))
            {
                
            }

            return model;
        }

        public ActionResult Editar(string id = null)
        {
            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            {
                var model = new SalesNoteRequestModel();
                if (model == null)
                {
                    SessionInfo.Alert.SetAlert("El documento especificado no existe!", SessionInfo.AlertType.Error);
                    return RedirectToAction("Emitidos", "Comprobantes");
                }                

                return View(model);
            }

            SessionInfo.Alert.SetAlert("Renueva tu suscripción por un plan Pymes o Pro y podrás solicitar tu firma electrónica", SessionInfo.AlertType.Info);
            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult Nuevo()
        {
            if (SessionInfo.UserSession?.Subscription?.Status == SubscriptionStatusEnum.Activa)
            {
                var model = new SalesNoteRequestModel();
                return View("Editar", model);
            }
            SessionInfo.Alert.SetAlert("Renueva tu suscripción por un plan Pymes o Pro y podrás solicitar tu firma electrónica", SessionInfo.AlertType.Info);
            return RedirectToAction("Index", "Dashboard");

        }

        [HttpPost]
        [HandleJsonException]
        public async System.Threading.Tasks.Task<JsonResult> SaveSalesNoteAsync(SalesNoteRequestModel model)
        {
            var errors = GetValidationError();

            if (ModelState.IsValid && model != null)
            {
                if(model.Details.Count > 0)
                {
                    int imten = 1;
                    model.Details.ForEach(det => {                        
                        det.MainCode = $"COD_{imten}";
                        det.ProductId = 0;
                        imten++;
                    });
                }

                var response = await ServicioComprobantes.GuardarNotaVentaAsync(IssuerToken, model);
                var text = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    ClearSession();

                    var doc = JsonConvert.DeserializeObject<DocumentReceived>(text);
                    return new JsonResult
                    {
                        Data = new
                        {
                            id = doc.pk,
                            result = doc,
                            error = default(object),
                            status = response.StatusCode,
                            statusText = "Nota de Venta Guardado",
                            url = Url.Action("Recibidos", "Comprobantes")
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        ContentType = "application/json"
                    };
                }               
            }

            return new JsonResult()
            {
                Data = new
                {
                    id = 0,
                    result = default(object),
                    error = errors,
                    status = HttpStatusCode.InternalServerError,
                    statusText = $"Error en la Nota de Venta. {errors?.ExceptionMessage}"
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentType = "application/json"
            };

        }
    }
}
