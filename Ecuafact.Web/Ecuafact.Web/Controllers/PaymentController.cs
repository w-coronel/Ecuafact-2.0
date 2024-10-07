using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Services;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ecuafact.Web.Controllers
{
    public class PaymentController : AppControllerBase
    {
        
        public ActionResult Index()
        {
            return RedirectToAction("Checkout");
        }

        public async Task<ActionResult> Checkout(int? purchaseOrderId = null)
        {
            purchaseOrderId = purchaseOrderId ?? SessionInfo.ShoppingCart?.PurchaseOrderId;
            if (purchaseOrderId != null)
            {
                var commerce = SessionInfo.Catalog?.eCommerceType?.FirstOrDefault();
                if (commerce == null)
                {
                    var model = await ServicioCompras.GenerarPagoAsync(IssuerToken, purchaseOrderId.Value, Constants.alignet);
                    if (model != null)
                    {
                        ViewBag.alignet = true;
                        model.modalComercio = Constants.modalComercio;
                        model.urlComercio = Constants.urlComercio;
                        return View(model);
                    }
                }
                else {
                    var model = await ServicioCompras.GenerarPagoAsync(IssuerToken, purchaseOrderId.Value, commerce.Code);
                    if (model != null)
                    {
                        ViewBag.ComercioCode = commerce.Code;
                        ViewBag.OpeningModes = commerce.OpeningModes;
                        ViewBag.urlComercio = commerce.UrlCommerce;
                        ViewBag.client_app_code = commerce.ClientAppCode;
                        ViewBag.client_app_key = commerce.ClientAppKey;
                        ViewBag.env_mode = commerce.Ambient == AmbientEnum.Development ? "stg" : "prod";
                        ViewBag.alignet = (commerce.Code == Constants.alignet);
                        return View(model);
                    }
                }
               

                // Regresa al inicio
                //return RedirectToAction("");
            }

            if (purchaseOrderId == null)
            {
                SessionInfo.Notifications.Add("No hay pagos que procesar", SessionInfo.AlertType.Error);
            }
            else
            {
                SessionInfo.Notifications.Add("La orden de pago especificada ya no esta disponible.", SessionInfo.AlertType.Warning);
            }

           return RedirectToAction("", "Dashboard");
        }

        public async Task<ActionResult> TypePayment(int? purchaseOrderId = null)
        {
            purchaseOrderId = purchaseOrderId ?? SessionInfo.ShoppingCart?.PurchaseOrderId;
            if (purchaseOrderId != null)
            {
                return View(model: purchaseOrderId);
            }

            if (purchaseOrderId == null)
            {
                SessionInfo.Notifications.Add("No hay pagos que procesar", SessionInfo.AlertType.Error);
            }
            else
            {
                SessionInfo.Notifications.Add("La orden de pago especificada ya no esta disponible.", SessionInfo.AlertType.Warning);
            }

            return RedirectToAction("", "Dashboard");
        }

        public async Task<ActionResult> BankTransfer(int? purchaseOrderId = null)
        {
            purchaseOrderId = purchaseOrderId ?? SessionInfo.ShoppingCart?.PurchaseOrderId;

            if (purchaseOrderId != null)
            {
                var model = await ServicioCompras.GenerarPagoTransferAsync(IssuerToken, purchaseOrderId.Value);

                if (model != null)
                {
                    return View(model);
                }

                // Regresa al inicio
                //return RedirectToAction("");
            }

            if (purchaseOrderId == null)
            {
                SessionInfo.Notifications.Add("No hay pagos que procesar", SessionInfo.AlertType.Error);
            }
            else
            {
                SessionInfo.Notifications.Add("La orden de pago especificada ya no esta disponible.", SessionInfo.AlertType.Warning);
            }

            return RedirectToAction("", "Dashboard");
        }

        [HttpPost]
        public ActionResult Checkout(PaymentRequestModel model)
        {
            return View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> Finish()
        {
            var jsonResult = GetRequestJSON();

            var paymentResult = JsonConvert.DeserializeObject<PaymentResultModel>(jsonResult);
            if (paymentResult != null)
            {
                paymentResult.Date = DateTime.Now;
                paymentResult.Result = jsonResult;

                var result = await ServicioCompras.GuardarResultadoAsync(paymentResult);

                if (!string.IsNullOrEmpty(paymentResult.ErrorCode))
                {
                    if (paymentResult.ErrorCode?.Trim() == "00")
                    {
                        SessionInfo.PendingPayments = new SessionInfo.Pagos();

                        if (paymentResult.TipoProceso == 1 && paymentResult.SubscriptionActiva)
                        {                            
                            SessionInfo.Alert.SetAlert("El pago se realizo con exito! la suscripción fue renovada y activada para emitir documentos electróniicos", SessionInfo.AlertType.Success);
                            var response = ServicioUsuario.ValidateToken(HttpContext.User.Identity.Name);
                            if (response.UserInfo != null)
                            {
                                // Actualizamos la informacion actual del emisor
                                AuthController.PerformIssuerAuthentication(response, true, SessionInfo.Issuer.RUC);
                            }

                            return RedirectToAction("", "Dashboard");
                        }
                        else{
                            SessionInfo.Alert.SetAlert("El pago se realizo con exito!", SessionInfo.AlertType.Success);
                            return RedirectToAction("", "FirmaElectronica");
                        }
                    }
                    else
                    {
                        SessionInfo.Alert.SetAlert(paymentResult.Message, SessionInfo.AlertType.Warning);

                        if (result.IsSuccess)
                        {
                            return RedirectToAction("Checkout", new { purchaseOrderId = result.Entity?.PurchaseOrder?.PurchaseOrderId });
                        }
                    }
                } 
            }
            else
            {
                SessionInfo.Alert.SetAlert("No se pudo procesar el pago!", SessionInfo.AlertType.Error);
            }

            return Redirect("~/");
        }

        [AllowAnonymous]
        public async Task<ActionResult> FinishStatus(long? paymentId)
        {
            if (paymentId != null)
            {
                var result = await ServicioCompras.ObtenerOrdenPagoAsync(IssuerToken, paymentId.Value);
                if (result.IsSuccess)
                {
                    ViewBag.url = Url.Action("TypePayment", new { purchaseOrderId = result.Entity?.PurchaseOrder.PurchaseOrderId });
                    ViewBag.content = "Volver opción de pago";

                    if (result.Entity.carrierCode?.Trim() == "00")
                    {                        
                        ViewBag.url = Url.Action("", "Dashboard");
                        ViewBag.content = "Volver a la página de Inicio";
                    }
                    return View(result.Entity);
                }

                SessionInfo.Notifications.Add(result.DevMessage, SessionInfo.AlertType.Error);
            }

            if (paymentId == null)
            {
                SessionInfo.Notifications.Add("No hay orden de pago", SessionInfo.AlertType.Error);
            }

            return RedirectToAction("", "Dashboard");

        }

        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> PaymentStatusAsync(PaymentTransaction model)
        {
            if (model == null)
            {
                return Json(new OperationResult(false, System.Net.HttpStatusCode.BadRequest) { UserMessage = "No se pudo procesar el pago!" });
            }

            var result = await ServicioCompras.GuardarPaymentResultadoAsync(model);
            if (result.IsSuccess)
            {
                if (model?.transaction?.carrier_code?.Trim() == "00")
                {
                    if (model.PurchaseOrder.Products.ToLower().Trim() != "firma electrónica")
                    {
                        AuthController.IssuerSubscriptionValidate(SessionInfo.Issuer.RUC);
                    }
                    SessionInfo.Alert.SetAlert("El pago se realizo con exito!", SessionInfo.AlertType.Success);
                }
                else
                {
                    SessionInfo.Alert.SetAlert("No se pudo procesar el pago", SessionInfo.AlertType.Error);
                }
            }
            result.Entity.transaction = model.transaction;
            return Json(result as OperationResult, JsonRequestBehavior.AllowGet);
        }

        private async Task HandleResult()
        {
            


        }

        public async Task<ActionResult> InvoiceOrder(int id = 0)
        {
            var result = await ServicioCompras.ValidarPagoSubscripcionAsync(IssuerToken, SessionInfo.Issuer.RUC);
            if (result.IsSuccess)
            {
                if (result.Entity.Subscription.Status == SubscriptionStatusEnum.Activa  && result.Entity.Subscription.LicenceType.Code != Constants.PlanBasic &&  
                    result.Entity.PurchaseOrder.Status == PurchaseOrderStatusEnum.Payed && result.Entity.Subscription.SubscriptionExpirationDate.HasValue)
                {
                    var dias = result.Entity.Subscription.SubscriptionExpirationDate.Value - DateTime.Now;
                    var balence = result.Entity.Subscription.BalanceDocument != null ? result.Entity.Subscription.BalanceDocument:0;                    
                    if (Convert.ToInt32(dias.TotalDays) > 30 && balence > 10)
                    {
                        return RedirectToAction("index", "Dashboard");
                    }
                        
                }
            }
            var plan = SessionInfo.Catalog.LicenceType.Where(l => l.Id == id)?.FirstOrDefault();
            if (plan == null)
            {
                return RedirectToAction("TypeLicence");
            }

            var typeLicence = await ServicioCompras.ValidarTipoLicenciaAsync(IssuerToken, plan.Id);
            if (typeLicence.IsSuccess)
            {
                var response = ServicioUsuario.ValidateToken(HttpContext.User.Identity.Name);
                if (response.UserInfo != null)
                {
                    // Actualizamos la informacion actual del emisor
                    AuthController.PerformIssuerAuthentication(response, true, SessionInfo.Issuer.RUC);
                }
                return RedirectToAction("index", "Dashboard");                
            }

            var model = new SubscriptionRequest()
            {
                IssuerId = SessionInfo.Issuer.Id,
                RUC = SessionInfo.Issuer.RUC,
                LicenceType = plan,
                LicenceTypeId = plan.Id,
                InvoiceInfo = new SubscriptionInvoice()
                {
                    Identification = SessionInfo.Issuer.RUC,
                    Name = SessionInfo.Issuer.BussinesName,
                    Address = SessionInfo.Issuer.MainAddress,
                    Email = SessionInfo.Issuer.Email,
                    Phone = SessionInfo.Issuer.Phone,
                    Product = $"Plan {plan.Name}({((plan.Code =="L03" || plan.Code == "L04") ? "Emisión ilimitada":plan.AmountDocument)} documentos anuales), {(plan.Code == "L04" ? $"{plan.Description?.Split('|')[0]}, {plan.Description?.Split('|')[3]}" : plan.Description?.Split('|')[2])}",
                    Price = plan.Price,
                    SubTotal = plan.Price,
                    Iva = (plan.Price * (plan.TaxBase/100)),
                    Total = (plan.Price + (plan.Price * (plan.TaxBase / 100))),
                    IvaRate = plan.TaxBase
                }
            };
            
            return View(model);
        }

        public async Task<ActionResult> TypeLicence()
        {
            ViewBag.basicPlanChange = true;
            var result = await ServicioCompras.BuscarPendienteAsync(IssuerToken, SessionInfo.Issuer.RUC);
            if (result.IsSuccess)
            {
                if (result.Entity.PurchaseOrderId > 0)
                {
                    if (result.Entity.PurchaseOrder.Status == PurchaseOrderStatusEnum.Transfer)
                    {
                        return RedirectToAction("Ver", new { id = result.Entity.Id });
                    }
                    else if (result.Entity.PurchaseOrder.Status != PurchaseOrderStatusEnum.Payed)
                    {                                         
                        return RedirectToAction("TypePayment", new { purchaseOrderId = result.Entity.PurchaseOrderId });
                    }
                    else if (result.Entity.PurchaseOrder.Status == PurchaseOrderStatusEnum.Payed)
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                }
            }

            if (SessionInfo.UserSession?.Subscription == null)
            {
                var createdOn = SessionInfo.Issuer.CreatedOn;
                var date = DateTime.Now.AddYears(-1).AddDays(1);
                if (createdOn > date)
                {
                    ViewBag.basicPlanChange = false;
                }                
            }

            return View();
        }

        public async Task<ActionResult> Ver(string id = null)
        {
            var model = await ServicioCompras.BuscarPendienteAsync(IssuerToken, SessionInfo.Issuer.RUC);

            if (model.IsSuccess)
            {               
                return View(model.Entity);
            }
            else
            {
                SessionInfo.Alert.SetAlert(model.UserMessage, SessionInfo.AlertType.Error);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<ActionResult> GenerarOrdenAsync(SubscriptionRequest model)
        {            
            var result = await ServicioCompras.GuardarOrdenSuscripcionAsync(IssuerToken, model);
            if (result.IsSuccess)
            {
                if (result.Entity.InvoiceId > 0 && result.Entity.Subscription.Status == SubscriptionStatusEnum.Activa)
                {
                    var response = ServicioUsuario.ValidateToken(HttpContext.User.Identity.Name);
                    if (response.UserInfo != null)
                    {
                        // Actualizamos la informacion actual del emisor
                        AuthController.PerformIssuerAuthentication(response, true, SessionInfo.Issuer.RUC);
                    }
                    SessionInfo.Alert.SetAlert("la suscripción fue renovada y activada para emitir documentos electróniicos", SessionInfo.AlertType.Success);
                    return RedirectToAction("Index", "Dashboard");
                }
                return RedirectToAction("TypePayment", "Payment", new { purchaseorderid = result.Entity?.PurchaseOrderId });               
            }
            else
            {
                SessionInfo.Alert.SetAlert(result.UserMessage, SessionInfo.AlertType.Error);
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public async Task<JsonResult> ValidateCode(string code, string newPlanCode)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Json(new OperationResult(false, System.Net.HttpStatusCode.BadRequest) { UserMessage = "Código promocional inválido!" });
            }
            var result = await ServicioCompras.ValidarCodigoPromocional(IssuerToken, code);
            if (result.IsSuccess)
            {
                var referenceCode = result.Entity;
                if (referenceCode.SpecialPromotion)
                {
                    if (referenceCode.ApplyDiscount.Equals(newPlanCode))
                    {
                        return Json(new OperationResult<ReferenceCodes>(true, System.Net.HttpStatusCode.OK, result.Entity) { UserMessage = "ha canjeado correctamente su código promocional" });
                    }
                }
                else
                {
                    return Json(new OperationResult<ReferenceCodes>(true, System.Net.HttpStatusCode.OK, result.Entity) { UserMessage = "ha canjeado correctamente su código promocional" });
                }
            }

            return  Json(new OperationResult(false, System.Net.HttpStatusCode.BadRequest) { UserMessage = $"El código {code} promocional inválido!" });

        }

        [HttpPost]
        public async Task<JsonResult> BuscarAsync(string uid)
        {

            try
            {
                string token = Constants.EcuanexusToken;
                var result = await Task.FromResult(ServicioCompras.ObtenerEmisorPorRuc(token,uid));
                if (result != null)
                {
                    return Json(new OperationResult<IssuerDto>(true, HttpStatusCode.OK, result), JsonRequestBehavior.AllowGet);
                }               
            }
            catch (Exception ex)
            {
                return Json(new OperationResult(ex), JsonRequestBehavior.AllowGet);
            }

            return Json(new OperationResult(false, HttpStatusCode.BadRequest), JsonRequestBehavior.AllowGet);
        }

        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GuardarComprobanteAsync(PaymentVoucherRequest voucher)
        {
            if (voucher == null)
            {
                return Json(new OperationResult(false, System.Net.HttpStatusCode.BadRequest) { UserMessage = "La solicitud enviada es inválida!" });
            }
            
            var result = await ServicioCompras.GuardarComprobanteAsync(IssuerToken, voucher);

            if (result.IsSuccess)
            {
                if(result.Entity.PurchaseSubscription != null)
                {
                    var response = ServicioUsuario.ValidateToken(HttpContext.User.Identity.Name);
                    if (response.UserInfo != null)
                    {
                        // Actualizamos la informacion actual del emisor
                        AuthController.PerformIssuerAuthentication(response, true, SessionInfo.Issuer.RUC);
                    }
                }
                SessionInfo.PendingPayments = new SessionInfo.Pagos();
                result.UserMessage = "OK";
            }

            return Json(result as OperationResult, JsonRequestBehavior.AllowGet);

        }


        private string GetRequestJSON()
        {
            // Genero el JSON para los datos recibidos:
            var keys = Request.Form.AllKeys;
            var json = "{";
            foreach (var key in keys)
            {
                if (key != keys.FirstOrDefault())
                {
                    json += ", ";
                }

                json += $"\"{key?.Trim()}\" : \"{Request.Form[key]?.Trim()}\" ";
            }

            json += " }";

            SaveJson(json);
            return json;
        }

        private static void SaveJson(string json)
        {
            var path = Path.Combine(Constants.VPOSDirectory, "LOGS");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Guardamos la informacion recibida en el log de registro.
            var fname = Path.Combine(path, $"PAYMENT.RESULT.{DateTime.Now.ToFileTime()}.json");
            System.IO.File.WriteAllText(fname, json);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> ValidarOrdenPago(long purchaseOrderId = 0)
        {
            var result = await ServicioCompras.ValidarPagoOrdenAsync(IssuerToken, purchaseOrderId);
            if (result.IsSuccess)
            {
                if (result.Entity.carrierCode == "00" && result.Entity.currentStatus.Contains("APPROVED-Pagada") && result.Entity.status.Contains("success"))
                {
                    var data = new { statusCode = "00", status = "success", url= Url.Content("~/Dashboard/index"), statusText =$"La orden {purchaseOrderId:0000000000} se encuentra pagada.."};
                    return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
               
            }

            return Json(new[] { new { statusCode = "", status = ""} }, JsonRequestBehavior.AllowGet);

        }
    }
}

