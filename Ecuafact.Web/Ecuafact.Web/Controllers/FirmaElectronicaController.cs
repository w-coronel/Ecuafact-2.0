using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ecuafact.Web.Domain.Services;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;

namespace Ecuafact.Web.Controllers
{
    public class FirmaElectronicaController : AppControllerBase
    {
        // GET: FirmaElectronica
        public async Task<ActionResult> Index()
        {
            var model = await ServicioFirma.BuscarAsync(IssuerToken);

            return View(model);
        }

        public async Task<ActionResult> Ver(string id = null)
        {
            var model = await ServicioFirma.GetSolicitudAsync(IssuerToken, id);

            if (model.IsSuccess)
            {
                if(model.Entity.Status == ElectronicSignStatusEnum.Error && model.Entity.InvoiceId > 0)
                {
                    return RedirectToAction("Editar", new { id });
                }

                return View(model.Entity);
            }
            else
            {
                SessionInfo.Alert.SetAlert(model.UserMessage, SessionInfo.AlertType.Error);

                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> Finish(string id = null)
        {
            var model = await ServicioFirma.GetSolicitudAsync(IssuerToken, id);

            if (model.IsSuccess)
            {
                return View(model.Entity);
            }
            else
            {
                SessionInfo.Alert.SetAlert(model.UserMessage, SessionInfo.AlertType.Error);

                return RedirectToAction("Index");
            }
        }

        public async Task<JsonResult> BuscarPendienteAsync(string id = null)
        {
            var solicitud = await ServicioFirma.BuscarPendienteAsync(IssuerToken, id);

            return Json(solicitud, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Editar(string id = null)
        {
            var esign = await ServicioFirma.GetSolicitudAsync(IssuerToken, id);            
            if (esign?.Entity == null)
            {
                return RedirectToAction("Nuevo");
            }
            var model = esign?.Entity?.ToRequest();
            return View(model);
        }

        public async Task<ActionResult> Nuevo()
        {
            if (SessionInfo.UserSession?.Subscription != null)
            {
                if (SessionInfo.UserSession?.Subscription.Status == SubscriptionStatusEnum.Activa && 
                    SessionInfo.UserSession?.Subscription.LicenceType.Code != Constants.PlanBasic)
                {
                    var model = BuildElectronicSign();
                    var result = await ServicioFirma.SolicitudPendientePorSubscripcionAsync(IssuerToken, SessionInfo.Issuer?.RUC);
                    if (result.IsSuccess)
                    {
                        model.SendProcessElectronicSign = true;
                    }

                    return View("Nuevo", model);
                }
                else if (SessionInfo.UserSession?.Subscription.Status == SubscriptionStatusEnum.ValidatingPayment)
                {
                    SessionInfo.Alert.SetAlert("Tu solicitud de firma electrónica no podrá ser realizada, debido a que existe un pago de suscripción que aún no hemos validado", SessionInfo.AlertType.Warning);
                }
                else
                {
                    SessionInfo.Alert.SetAlert("Antes de solicitar tu firma electrónica, por favor renovar tu suscripción, te recordamos que el pago de la  renovación de suscripción incluye GRATIS la emisión de la firma electrónica", SessionInfo.AlertType.Warning);
                }
            }
            else {
                SessionInfo.Alert.SetAlert("Es necesario que selecciones un plan para la emisión de tus comprobantes electrónicos. Incluye GRATIS la emisión de la firma electrónica", SessionInfo.AlertType.Warning);
            }

            return RedirectToAction("index", "Dashboard");
        }

        public ActionResult ConfigurarCertificado()
        {
            return PartialView("Config");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GuardarFirmaAsync(ElectronicSignRequest firma)
        {
            if (firma == null)
            {
                return Json(new OperationResult(false, System.Net.HttpStatusCode.BadRequest) { UserMessage = "La solicitud enviada es inválida!" });
            }

            firma.SignatureValidy = SignatureValidyEnum.OneYear;
            firma.FileFormat = FileFormatEnum.File;

            firma.VerificationType = VerificationTypeEnum.Ecuanexus;
            firma.Phone = firma.Phone.Replace('.', ',');
            var result = await ServicioFirma.GuardarFirmaAsync(IssuerToken, firma);

            if (result.IsSuccess)
            {
                SessionInfo.UserSession.Subscription.RequestElectronicSign = 1;
                SessionInfo.ShoppingCart = result.Entity?.PurchaseOrder;
                var url = Url.Action("Checkout", "Payment", new { purchaseorderid = result.Entity?.PurchaseOrder.PurchaseOrderId });

                if (Constants.TypePayment)
                {
                    url = Url.Action("TypePayment", "Payment", new { purchaseorderid = result.Entity?.PurchaseOrder.PurchaseOrderId });
                }

                if (result.Entity?.PurchaseOrder.ReferenceCodes != null)
                {
                    if ((bool)result.Entity?.PurchaseOrder.ReferenceCodes.SkipPaymentOrder)
                    {
                        url = Url.Action("Finish", "FirmaElectronica", new { id = result.Entity?.Id });
                    }
                }
                else if (result.Entity.InvoiceId > 0)
                {
                    url = Url.Action("Finish", "FirmaElectronica", new { id = result.Entity?.Id });
                }

                result.Entity.PurchaseOrder.UrlRedirect = url;
            }

            return Json(result as OperationResult, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditarFirmaAsync(ElectronicSignRequest firma)
        {
            if (firma == null)
            {
                return Json(new OperationResult(false, System.Net.HttpStatusCode.BadRequest) { UserMessage = "La solicitud enviada es inválida!" });
            }

            firma.SignatureValidy = SignatureValidyEnum.OneYear;
            firma.FileFormat = FileFormatEnum.File;
            firma.VerificationType = VerificationTypeEnum.Ecuanexus;
            firma.Phone = firma.Phone.Replace('.', ',');

            var result = await ServicioFirma.EditarFirmaAsync(IssuerToken, firma);

            if (result.IsSuccess)
            {
                SessionInfo.ShoppingCart = result.Entity?.PurchaseOrder;
                var url = Url.Action("Ver", "FirmaElectronica", new { id = result.Entity?.Id });
               
                result.Entity.PurchaseOrder.UrlRedirect = url;
            }

            return Json(result as OperationResult, JsonRequestBehavior.AllowGet);

        }

        private ElectronicSignRequest BuildElectronicSign()
        {

            var model = new ElectronicSignRequest
            {
                SignatureValidy = SignatureValidyEnum.OneYear,
                VerificationType = VerificationTypeEnum.Ecuanexus,
                DocumentType = IdentificationTypeEnum.IdentityCard,
                FileFormat = FileFormatEnum.File,
                Identification = SessionInfo.UserInfo.Username,
                FirstName = SessionInfo.UserInfo.Name?.ToUpper(),
                LastName = "",
                Address = SessionInfo.Issuer?.MainAddress?.ToUpper() ?? SessionInfo.UserInfo.Address?.ToUpper(),
                RUC = SessionInfo.Issuer?.RUC ?? (SessionInfo.UserInfo.Username.Length == 10 ? SessionInfo.UserInfo.Username + "001" : SessionInfo.UserInfo.Username),
                BusinessName = SessionInfo.Issuer?.BussinesName?.ToUpper() ?? SessionInfo.UserInfo.Name?.ToUpper(),
                BusinessAddress = SessionInfo.Issuer?.MainAddress?.ToUpper() ?? SessionInfo.UserInfo.Address?.ToUpper(),
                City = SessionInfo.Issuer?.City?.ToUpper(),
                Province = SessionInfo.Issuer?.Province?.ToUpper(),
                Country = "EC",
                Email = SessionInfo.Issuer?.Email,
                Phone = SessionInfo.Issuer?.Phone,
                Email2 = "",
                ConfirmEmail2 =""
            };

            model.SignType = model.RUC.IsNaturalIdentity() ? RucTypeEnum.Natural : RucTypeEnum.Juridical;

            if (model.Identification.Length == 10)
            {
                model.DocumentType = IdentificationTypeEnum.IdentityCard;
            }
            else if (model.Identification.Length == 13)
            {
                model.DocumentType = IdentificationTypeEnum.RUC;
            }
            else
            {
                model.DocumentType = IdentificationTypeEnum.Passport;
            }

            model.InvoiceInfo = new ElectronicSignInvoice
            {
                Identification = model.RUC,
                Name = model.BusinessName,
                Address = model.Address,
                Email = model.Email,
                Phone = model.Phone
            };

            var nombres = SessionInfo.UserInfo.Name?.ToUpper().Split(' ');

            model.FirstName = SessionInfo.UserInfo.Name?.ToUpper();

            if (nombres.Length < 4)
            {
                model.FirstName = nombres[0];
            }
            else if (nombres.Length > 3)
            {
                model.FirstName = nombres[0] + " " + nombres[1];
            }

            model.LastName = SessionInfo.UserInfo.Name.ToUpper().Replace(model.FirstName, "").Trim();

            return model;
        }

        [HttpGet]
        public JsonResult ValidarRequestEmail(string email = null)
        {
            var amount = ServicioFirma.ValidarEmailBySolicitud(SecurityToken, email);
            return new JsonResult()
            {
                Data = new
                {
                    id = amount,
                    result = amount,
                    status = SessionInfo.AlertType.Success,                    
                    mensaje = amount > 3 ? $"El correo electrónico {email} esta registrado en otras solicitudes, para continuar con esta solicitud debe ingresar el correo 2 y número 2 : los datos correspondientes al cliente representado  y adjuntar la  carta de autorización firmada por el cliente con el formato respectivo." : "",
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                ContentType = "application/json"
            };
           
        }

        [HttpPost]
        [HandleJsonException]
        public async Task<JsonResult> GuardarCertificadoAsync(CertificateConf certificate)
        {
            var errors = "Error al procesar la solicitud!";
            try
            {                
                if (certificate != null)
                {
                    certificate.Ruc = SessionInfo.Issuer.RUC;
                    var response = await ServicioFirma.GuardarCertificado(IssuerToken, certificate); 
                    if (response.IsSuccess)
                    {                       
                        return new JsonResult {
                            Data = new {
                                result = true,
                                error = default(object),
                                status = response.StatusCode,
                                message = "Se ha configurado exitosamente el certificado"
                            },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            ContentType = "application/json"
                        };
                    }
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
    }



}
