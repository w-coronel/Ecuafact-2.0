using System;
using System.Web.Mvc;
using System.Web.Security;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.MiddleCore.NexusApiServices;
using System.Linq;
using Ecuafact.Web.Domain.Entities.API;
using System.Collections.Generic;
using System.Web;
using System.Threading;
using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Models;
using System.Threading.Tasks;

namespace Ecuafact.Web.Controllers
{
    public class AuthController : AppControllerBase
    {

        [AllowAnonymous]
        public ActionResult Index(string id = null, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(returnUrl) || returnUrl == "null" || returnUrl.Contains("auth"))
            {
                returnUrl = Url.Content("~/");
            }

            SessionInfo.ReturnUrl = Url.Content(returnUrl);

            // Si el usuario ya esta autenticado debe ir a la pagina principal
            if (IsAuthenticated || EnsureUserIsLoggedIn())
            {
                return Redirect(SessionInfo.ReturnUrl);
            }
            else
            {
                // Configuramos la pagina de inicio de sesión
                if (!string.IsNullOrEmpty(id?.Trim()))
                {
                    var errorAPI = this.Decode<ErrorAPI>(id);

                    if (errorAPI != null)
                    {
                        ViewBag.isRedirectedToLogin = true;
                        ViewBag.AlertColor = errorAPI.AlertColor;
                        ViewBag.ApiMessage = errorAPI.ApiMessage;

                        SessionInfo.Alert.SetAlert(errorAPI.ApiMessage, type: SessionInfo.AlertType.Error);
                    }
                    else
                    {
                        ViewBag.isRedirectedToLogin = false;
                    }
                }
                else
                {
                    ViewBag.isRedirectedToLogin = false;
                }

                return View(new LoginModel());
            }
        }

        [AllowAnonymous]
        public ActionResult Carrier(string id = null, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(returnUrl) || returnUrl == "null" || returnUrl.Contains("auth/Carrier"))
            {
                returnUrl = Url.Content("~/Carrier/Establishment/Index");
            }

            SessionInfo.ReturnUrl = Url.Content(returnUrl);

            // Si el usuario ya esta autenticado debe ir a la pagina principal
            if (IsAuthenticated || EnsureUserIsLoggedIn())
            {
                return Redirect(SessionInfo.ReturnUrl);
            }
            else
            {
                // Configuramos la pagina de inicio de sesión
                if (!string.IsNullOrEmpty(id?.Trim()))
                {
                    var errorAPI = this.Decode<ErrorAPI>(id);

                    if (errorAPI != null)
                    {
                        ViewBag.isRedirectedToLogin = true;
                        ViewBag.AlertColor = errorAPI.AlertColor;
                        ViewBag.ApiMessage = errorAPI.ApiMessage;

                        SessionInfo.Alert.SetAlert(errorAPI.ApiMessage, type: SessionInfo.AlertType.Error);
                    }
                    else
                    {
                        ViewBag.isRedirectedToLogin = false;
                    }
                }
                else
                {
                    ViewBag.isRedirectedToLogin = false;
                }

                return View(new LoginModel());
            }
        }

        internal static bool EnsureUserIsLoggedIn()
        {
            var HttpContext = System.Web.HttpContext.Current;

            bool wasLoggedIn = (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated);
            var token = HttpContext.User?.Identity?.Name;

            if (string.IsNullOrEmpty(token) && RequestCookies[USER_COOKIE] != null)
            {
                token = RequestCookies[USER_COOKIE].Value.Decode();
            }

            // Si el usuario ha iniciado sesion pero no
            // se encuentra cargada su informacion se debe cargar sus datos localmente
            if (wasLoggedIn || !string.IsNullOrEmpty(token))
            {
                //proceso de validacion:
                var response = ServicioUsuario.ValidateToken(token);

                if (response.UserInfo != null)
                {
                    PerformIssuerAuthentication(response, true);
                    HttpContext.User = Thread.CurrentPrincipal;
                    return true;
                }
            }

            return false;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Authenticate(LoginModel data)
        {
            
            try
            {
                var response = ServicioUsuario.LogIn(data);
                  
                if (response.UserInfo != null)
                {
                    // Si es Ecuanexus Express solo inicia si el emisor esta registrado
                    if (Constants.Ecuanexus && data.Username.Contains("0992882549001"))
                    {
                        return Json(new { id = 1, description = "Redireccionando a Ecuafact APP...", info = "", url = "https://app.ecuafact.com" }, JsonRequestBehavior.AllowGet);
                    }

                    SessionInfo.SoporteUrl = Constants.SoporteUrl;
                    var isIssuer = PerformIssuerAuthentication(response, data.RememberMe);

                    // Si ya existe la Cookie del usuario, se remueve para volver a crearla.
                    if (data.RememberMe)
                    {
                        // Registramos la Cookie:
                        HttpCookie cookie = new HttpCookie(USER_COOKIE, $"{response.UserInfo.ClientToken}|{Guid.NewGuid()}".Encode())
                        {
                            Expires = DateTime.Now.AddDays(15D) // Expira en 15 dias.
                        };

                        ResponseCookies.Add(cookie);
                    }

                    HttpContext.User = Thread.CurrentPrincipal;

                    if (string.IsNullOrEmpty(SessionInfo.ReturnUrl) || SessionInfo.ReturnUrl.Contains("Auth") || SessionInfo.ReturnUrl.Contains("dashboard"))
                    {
                        SessionInfo.ReturnUrl = Url.Content("~/");
                    }

                    if (response.UserInfo.Messages != null)
                    {
                        foreach (var item in response.UserInfo.Messages)
                        {
                            SessionInfo.Notifications.Add(item, SessionInfo.AlertType.Warning);
                        }
                    }

                    // Envia al usuario a la pagina principal
                    return Json(new { id = 1, message = "Usuario Autenticado!", url = SessionInfo.ReturnUrl, data = response, issuer = isIssuer });
                }

                return Json(new { id = 0, message = response.Error?.Message ?? "El usuario no existe o la contraseña esta incorrecta!", info = response }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { id = 0, message = "Error de autenticacion: " + ex.Message, info = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        [AllowAnonymous]
        //public ActionResult AuthenticateCarrier(LoginModel data)
        //{
        //    try
        //    {
        //        var response = ServicioUsuario.LogIn(data);

        //        if (response.UserInfo != null)
        //        {
        //            SessionInfo.SoporteUrl = Constants.SoporteUrl;
        //            var ruc = response.UserInfo.Username.Length == 13 ? response.UserInfo.Username : $"{response.UserInfo.Username}001";
        //            var isIssuer = PerformIssuerAuthentication(response, data.RememberMe, ruc);
        //            // Si ya existe la Cookie del usuario, se remueve para volver a crearla.
        //            if (data.RememberMe)
        //            {
        //                // Registramos la Cookie:
        //                HttpCookie cookie = new HttpCookie(USER_COOKIE, $"{response.UserInfo.ClientToken}|{Guid.NewGuid()}".Encode())
        //                {
        //                    Expires = DateTime.Now.AddDays(15D) // Expira en 15 dias.
        //                };

        //                ResponseCookies.Add(cookie);
        //            }

        //            HttpContext.User = Thread.CurrentPrincipal;

        //            if (string.IsNullOrEmpty(SessionInfo.ReturnUrl) || SessionInfo.ReturnUrl.Contains("Auth/carrier") || SessionInfo.ReturnUrl.Contains("establishment"))
        //            {
        //                SessionInfo.ReturnUrl = Url.Content("~/carrier");
        //            }

        //            if (response.UserInfo.Messages != null)
        //            {
        //                foreach (var item in response.UserInfo.Messages)
        //                {
        //                    SessionInfo.Notifications.Add(item, SessionInfo.AlertType.Warning);
        //                }
        //            }

        //            // Envia al usuario a la pagina principal
        //            return Json(new { id = 1, message = "Usuario Autenticado!", url = SessionInfo.ReturnUrl, data = response, issuer = isIssuer });
        //        }

        //        return Json(new { id = 0, message = response.Error?.Message ?? "El usuario no existe o la contraseña esta incorrecta!", info = response }, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { id = 0, message = "Error de autenticacion: " + ex.Message, info = ex.ToString() }, JsonRequestBehavior.AllowGet);
        //    }

        //}

        internal static bool PerformIssuerAuthentication(LoginResponseModel login, bool rememberMe = true, string defaultRUC = null)
        {
            SessionInfo.LoginInfo = login;

            long x;
            login.Issuers = login.Issuers?
                .Where(model => long.TryParse(model?.RUC, out x))?
                .OrderBy(p => Convert.ToInt32(p.RUC != login.UserInfo.Username))
                .ToList()
                ?? new List<LoginIssuerModel>();

            if (string.IsNullOrEmpty(defaultRUC))
            {
                // Antes que nada revisamos si existen Cookies con el RUC por DEFAULT para la Sesion.
                var cookie = RequestCookies[ISSUER_COOKIE];

                // Si la Cookie existe entonces se define el RUC como default
                if (cookie != null && (cookie.Expires == new DateTime(1, 1, 1) || cookie.Expires > DateTime.Now))
                {
                    defaultRUC = cookie.Value.Decode();
                }
                else
                {
                    defaultRUC = login.UserInfo.Username;
                }
            }

            // Entonces si es correcta la informacion actual, obtenemos el emisor actual para este usuario
            var emisorActual = login.Issuers.FirstOrDefault(p => p.RUC.StartsWith(defaultRUC));

            if (emisorActual == null)
            { 
                login.Issuers.Add(emisorActual = new LoginIssuerModel
                {
                    RUC = defaultRUC,
                    BusinessName = login.UserInfo.BusinessName,
                    ClientToken = login.UserInfo.ClientToken,
                    Email = login.UserInfo.Email,
                    Name = login.UserInfo.Name,
                    PK = "0"
                });
                
            }
             
            FormsAuthentication.SetAuthCookie(login.UserInfo.ClientToken, rememberMe);
            bool isIssuer = EstablecerEmisor(emisorActual.RUC);           
            if (!isIssuer) { SessionInfo.RewardsUrl = $"{Constants.RewardsUrl}{login.UserInfo.ClientToken}"; }
            Thread.CurrentPrincipal = login.ToLogin();           
            return isIssuer;
        }

        public JsonResult Emisor(string id, string returnUrl = null)
        {
            // Si se procede el cambio de emisor entonces se actualiza la pagina de inicio
            var url_ = SessionInfo.ReturnUrl;
            var referrerUrl = Request.UrlReferrer.AbsolutePath.ToLower();

            if (EstablecerEmisor(id))
            {
                var issuer = SessionInfo.Issuer;
                SessionInfo.Alert.SetAlert(
                   "<b>Usted ha seleccionado el siguiente emisor:</b><br/>"+
                   $"<img style='width: 200px;' class='img-thumbnail' src='{Url.Content(Server.GetLogoUrl(issuer.Logo, issuer.RUC))}'></img>"+
                   $"<h4>{issuer.RUC}</h4> " +
                   $"<h5>{issuer.BussinesName}</h5>", 
                    SessionInfo.AlertType.Success);

                if (referrerUrl.Contains("firmaelectronica/nuevo") || referrerUrl.Contains("firmaelectronica/editar") || referrerUrl.Contains("firmaelectronica/finish") ||
                    referrerUrl.Contains("payment/checkout") ||  referrerUrl.Contains("payment/typepayment") || referrerUrl.Contains("payment/typelicence") ||
                    referrerUrl.Contains("payment/banktransfer") || referrerUrl.Contains("payment/finishstatus") || referrerUrl.Contains("payment/finish") ||
                    referrerUrl.Contains("payment/Invoiceorder") || SessionInfo.UserRole == Ecuafact.Web.Models.SecuritySessionRole.Cooperative) 
                {
                    return Json(new { id = issuer.Id, url = Url.Action("", "dashboard"), reload = false, issuer }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return Json(new { id = issuer.Id, url = Url.Action("", "dashboard"), reload = true, issuer }, JsonRequestBehavior.AllowGet);
                }

                // Si se especifica la url de retorno.
                return Json(new { id=issuer.Id, url = Url.Content(returnUrl), reload = true, issuer }, JsonRequestBehavior.AllowGet);
            }

            // Se procede a retornar al inicio de sesion
            return Json(new { id=0, url = Url.Action("", "auth"), issuer = default(IssuerDto) }, JsonRequestBehavior.AllowGet);
        }

        private static bool EstablecerEmisor(string issuerRUC)
        {

            // Verifica la informacion del emisor para el usuario actual
            var session = ServicioAPI.RequestSession(SessionInfo.SecurityToken, issuerRUC);

            // Si no se genera la session significa que el usuario no tiene acceso
            if (session == null || string.IsNullOrEmpty(session.Token))
            {
                return false;
            }

            // Si ya existe la Cookie, se remueve para volver a crearla.
            if (RequestCookies.AllKeys.Contains(ISSUER_COOKIE))
            {
                RequestCookies.Remove(ISSUER_COOKIE);
            }

            // Registramos la Cookie:
            HttpCookie cookie = new HttpCookie(ISSUER_COOKIE, $"{issuerRUC}|{Guid.NewGuid()}".Encode())
            {
                Expires = DateTime.Now.AddHours(12) // Expira en doce horas. A menos que vuelva a ingresar
            };

            ResponseCookies.Add(cookie); 
            SessionInfo.UserSession = session;
            SessionInfo.LoginInfo.CurrentIssuer = SessionInfo.LoginInfo.Issuers.Find(model => model.RUC == issuerRUC);
            SessionInfo.RewardsUrl = $"{Constants.RewardsUrl}{session.Token}";
            PagoPendiente(issuerRUC);
            EstadoSolcitudFirma(issuerRUC);

            return true;
        }

        internal static bool IssuerSubscriptionValidate(string issuerRUC = null)
        {
            // Validamos la suscripcion del emisor actual
            var subscription = ServicioAPI.RequestSubscription(SessionInfo.UserSession.Token, issuerRUC);

            // validamos si tiene subscripcion
            if (subscription != null)
            {
                SessionInfo.UserSession.Subscription = subscription;
                return true;
            }

            return false;
        }

        private static void PagoPendiente(string RUC)
        {
            SessionInfo.PendingPayments = new SessionInfo.Pagos();
            var pendingResult = ServicioFirma.BuscarPendiente(SessionInfo.SecurityToken, RUC);          
           
            if (pendingResult.IsSuccess && pendingResult.Entity != null)
            {
                if (pendingResult.Entity.PurchaseOrder.Status != PurchaseOrderStatusEnum.Payed && pendingResult.Entity.PurchaseOrder.Status != PurchaseOrderStatusEnum.Transfer)
                {
                    var invoiceId = pendingResult.Entity.InvoiceId ?? 0;
                    if (pendingResult.Entity.CreatedOn >= new DateTime(2021, 07, 15) && invoiceId == 0)
                    {
                        SessionInfo.PendingPayments.Pendiente = pendingResult.IsSuccess;
                        SessionInfo.PendingPayments.ElectronicSign = pendingResult.Entity;
                        SessionInfo.PendingPayments.UrlPago = $"payment/typePayment?purchaseorderid={pendingResult.Entity.PurchaseOrderId}";
                    }
                }
            }            
        }

        private static void EstadoSolcitudFirma(string RUC)
        {            
            var pendingResult = ServicioFirma.EstadoFirmaElectronica(SessionInfo.SecurityToken, RUC);           
        }

        internal static string USER_COOKIE
        {
            get { return $"Ecuafact.Web_SessionId"; }
        }

        internal static string ISSUER_COOKIE
        {
            get { return $"+ISSUER+{SessionInfo.LoginInfo?.UserInfo?.Username}+".Encode().Replace("==", ""); }
        }

        //
        // GET: /Registro/
        [AllowAnonymous]
        public ActionResult Registro()
        {
            var model = new RegisterModel();
            var queryString = Request.QueryString;
            if (queryString.Count > 0)
            {
                if (!String.IsNullOrEmpty(queryString["utm_source"]) && !String.IsNullOrEmpty(queryString["utm_medium"]) && !String.IsNullOrEmpty(queryString["utm_campaign"]))
                {
                    model.UTMS = new UTMS(){ 
                        source = queryString["utm_source"]?? "",
                        medium = queryString["utm_medium"] ?? "",
                        campaign = queryString["utm_campaign"] ?? "",
                    };
                }
            }
            ViewBag.isRedirectedToLogin = ViewBag.isRedirectedToLogin ?? false;
            return View(model);
        }
         
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Registro(RegisterModel data)
        {
            ViewBag.isRedirectedToLogin = false;
            data.Identification = data.User;

            var request = new registerRequest
            {
                new_client = new register
                {
                    id = data.Identification,
                    businessName = data.Name,
                    name = data.Name,
                    password = data.Password,
                    email = data.Email,
                    phone = data.Phone,
                    dependents = data.Dependents ?? 0,
                    streetAddress = data.Address,
                    appname = "WEB"
                }
            };

            // Agregamos el nuevo usuario
            var response = ServicioUsuario.RegisterUser(request);

            if (response?.UserInfo != null)
            {
                if (data.UserIssuer)
                {
                    if (data.UTMS != null){
                        if (!String.IsNullOrEmpty(data.UTMS.source) && !String.IsNullOrEmpty(data.UTMS.medium) && !String.IsNullOrEmpty(data.UTMS.campaign)){
                            ServicioUsuario.UserCampaign(data);
                        } 
                    }
                    
                    var login = this.Authenticate(new LoginModel { Username = data.User, Password = data.Password }) as JsonResult;
                    if (login != null && login.Data != null)
                    {
                        var dynamicData = login.Data as dynamic;

                        if (dynamicData.id != null && dynamicData.id == 1)
                        {
                            SessionInfo.Alert.SetAlert("Su cuenta fue creada exitosamente!", type: SessionInfo.AlertType.Success);
                            return RedirectToAction("", "Inicio");
                        }
                    }
                }

                SessionInfo.Alert.SetAlert("Su cuenta fue creada exitosamente, inicie sesión para continuar..", type: SessionInfo.AlertType.Success);
                return RedirectToAction("Index");
            }

            if (response?.Result != null)
            {
                ViewBag.isRedirectedToLogin = true;
                ViewBag.AlertColor = "alert-danger";
                ViewBag.ApiMessage = response.Result.Message;
                SessionInfo.Alert.SetAlert(response.Result.Message, type: SessionInfo.AlertType.Error);

                return View("Registro", data);
                //return RedirectToLogin(Convert.ToInt32(response.Result.Code), response.Result.Message);
            }
            else
            {
                SessionInfo.Alert.SetAlert("Error al crear la cuenta.", type: SessionInfo.AlertType.Error);
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<string> Feature()
        {
            var uid = Request.Headers["uid"];
            var username = Request.Headers["usn"];
            var token = (string)Session["TOKEN_REGISTRO"];

            if (token != null && token.Equals(uid, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(username))
                {
                    var ruc = username.Length == 10 ? username + "001" : username;

                    var result = await ServicioSRI.FindByRucAsync(Constants.ServiceToken, ruc);

                    if (result.IsSuccess)
                    {
                        return result.Entity.BusinessName;
                    }
                }
            } 

            return null;
        }
          
        //
        // GET: /ResetearContrasena/
        [AllowAnonymous]
        public ActionResult Reset()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Reset(string id)
        {
            var request = new passwordRecoveryRequest();
            request.id = id;

            var response = ServicioUsuario.ForgotPassword(request);

            if (response != null)
            {

                if (response.result.code == "1000")
                {
                    SessionInfo.Alert.SetAlert("Un mensaje de notificación fue enviado a su correo..", SessionInfo.AlertType.Success);
                    return RedirectToAction("");
                }

                SessionInfo.Alert.SetAlert(response.result.message, SessionInfo.AlertType.Error);

            }
            else
            {
                SessionInfo.Alert.SetAlert("No se puede procesar el comando para el usuario especificado.", SessionInfo.AlertType.Error);
            }
            return View();
        }

        //
        // GET: /CambiarContrasena/
        [AllowAnonymous]
        public ActionResult Password(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                if (!IsAuthenticated && !EnsureUserIsLoggedIn())
                {
                    return RedirectToAction("");
                }

                id = SessionInfo.SecurityToken;
            }

            var model = new PasswordModel { UniqueId = id };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ChangePassword(PasswordModel model)
        {
            // Si el usuario inicio sesion y se usa su token de seguridad
            // no se asigna esta variable.
            string requestToken = null;

            try
            {

                if (model == null)
                {
                    throw new Exception("Los datos de su solicitud son invalidos!" + Environment.NewLine + " No se pudo cambiar la clave!");
                }

                if (SessionInfo.LoginInfo != null && SessionInfo.SecurityToken == model.UniqueId)
                {
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        throw new Exception("Su clave actual no es válida! ");
                    }

                    // Validamos que el usuario que pide el cambio de clave es el verdadero
                    var userValid = ServicioUsuario.LogIn(new LoginModel { Username = SessionInfo.LoginInfo.UserInfo.Username, Password = model.Password });
                    if (userValid == null || string.IsNullOrEmpty(userValid.UserInfo?.Username))
                    {
                        throw new Exception("Su clave actual no es válida! ");
                    }
                }
                else
                {
                    requestToken = model.UniqueId;
                }

                if (string.IsNullOrEmpty(model.NewPassword) || model.NewPassword.Length < 4)
                {
                    throw new Exception("Su nueva clave debe tener al menos 4 caracteres! ");
                }

                if (model.NewPassword != model.NewPassword2)
                {
                    throw new Exception("Ambas contraseñas deben ser iguales! ");
                }

                var result = ServicioUsuario.ChangePassword(new PasswordChangeRequest
                {
                    unique_Id = model.UniqueId,
                    password = model.NewPassword
                });

                if (result.result.code == "1000")
                {
                    SessionInfo.Alert.SetAlert("Su clave ha sido cambiada!", SessionInfo.AlertType.Success);
                    return RedirectToAction("");
                }
                else
                {
                    SessionInfo.Alert.SetAlert(result.result.message ?? "Hubo un error al realizar el proceso!", SessionInfo.AlertType.Error);
                }
            }
            catch (Exception ex)
            {
                SessionInfo.Alert.SetAlert(ex.Message, SessionInfo.AlertType.Error);
            }

            // Esto es importante ya que el token de seguridad es privado y no se debe publicar en la url
            if (!string.IsNullOrEmpty(requestToken))
            {
                return RedirectToAction("Password", new { id = requestToken });
            }
            else
            {
                return RedirectToAction("Password");
            }
        }

        [HttpPost]
		public JsonResult LogOff()
		{
			Thread.CurrentPrincipal = HttpContext.User = null;
			FormsAuthentication.SignOut();
            Session.Abandon();
            Session.RemoveAll();
            Session.Clear();
            Session.Remove("usuario");
            Response.AppendHeader("Cache-Control", "no-store");
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));

            if (RequestCookies[USER_COOKIE] != null)
            {
                ResponseCookies[USER_COOKIE].Value = null;
                ResponseCookies[USER_COOKIE].Expires = DateTime.Now.AddDays(-1);
                RequestCookies.Remove(USER_COOKIE);
            }
             
			return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
		}

        public ActionResult LogOut()
        {
            try
            {
                var result = LogOff().Data as dynamic;

                if (result!=null && result.Id == 0)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                SessionInfo.LogException(ex);
            }
            
            return Redirect("~/");
        }

        [HttpPost]
        public JsonResult LogOffC()
        {
            Thread.CurrentPrincipal = HttpContext.User = null;
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.RemoveAll();
            Session.Clear();
            Session.Remove("usuario");
            Response.AppendHeader("Cache-Control", "no-store");
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));

            if (RequestCookies[USER_COOKIE] != null)
            {
                ResponseCookies[USER_COOKIE].Value = null;
                ResponseCookies[USER_COOKIE].Expires = DateTime.Now.AddDays(-1);
                RequestCookies.Remove(USER_COOKIE);
            }


            return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOutC()
        {
            try
            {
                var result = LogOffC().Data as dynamic;

                if (result != null && result.id == 0)
                {
                    return RedirectToAction("Carrier");
                }
            }
            catch (Exception ex)
            {
                SessionInfo.LogException(ex);
            }

            return Redirect("~/");
        }
    }

}
