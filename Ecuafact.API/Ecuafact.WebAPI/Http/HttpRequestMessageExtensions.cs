using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Dependencies;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Models.Authentication;
using System.Net;
using Newtonsoft.Json;
using System;
using System.Text;
using Ecuafact.WebAPI;
using static Ecuafact.WebAPI.Models.Authentication.LoginResponseModel;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using Ecuafact.WebAPI.Models;
using Ecuafact.WebAPI.Models.Dtos;
using Ecuafact.WebAPI.Domain.Entities.SRI;
using System.Security.Cryptography;
using Ecuafact.WebAPI.PayMe;
using System.Web.Script.Serialization;
using System.IO;
using Newtonsoft.Json.Linq;

namespace System
{
    internal static class HttpRequestMessageExtensions
    {
        internal static string GetMessage(this Exception ex)
        {
#if DEBUG
            return ex?.InnerException?.InnerException?.Message
                                  ?? ex?.InnerException?.Message
                                  ?? ex?.Message;
#else
            return ex?.InnerException?.Message ?? ex?.Message;
#endif

        }

        internal static IIssuersService GetIssuersService(this HttpRequestMessage request)
        {
            return request.GetService<IIssuersService>();
        }
        internal static IUserService GetUserService(this HttpRequestMessage request)
        {
            return request.GetService<IUserService>();
        }
        internal static IDocumentsService GetDocumentService(this HttpRequestMessage request)
        {
            return request.GetService<IDocumentsService>();
        }
        internal static IPurchaseOrderService GetElectronicSignService(this HttpRequestMessage request)
        {
            return request.GetService<IPurchaseOrderService>();
        }
        internal static ISubscriptionService GetSubscriptionService(this HttpRequestMessage request)
        {
            return request.GetService<ISubscriptionService>();
        }
        internal static IPurchaseOrderService GetPurchaseOrderService(this HttpRequestMessage request)
        {
            return request.GetService<IPurchaseOrderService>();
        }
        internal static IRequestSessionsService GetIRequestSessionsService(this HttpRequestMessage request)
        {
            return request.GetService<IRequestSessionsService>();
        }
        internal static TService GetService<TService>(this HttpRequestMessage request)
        {
            IDependencyScope dependencyScope = request.GetDependencyScope();
            TService service = (TService)dependencyScope.GetService(typeof(TService));
            return service;
        }
        internal static ICatalogsService GetCatalogsService(this HttpRequestMessage request)
        {
            return request.GetService<ICatalogsService>();
        }
        internal static IContributorsService GetContributorsService(this HttpRequestMessage request)
        {
            return request.GetService<IContributorsService>();
        }
        internal static IProductsService GetProductsService(this HttpRequestMessage request)
        {
            return request.GetService<IProductsService>();
        }
        public static string BasicAuthTokenEncode(this HttpRequestMessage Request, string user, string password)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes($"{user}:{password}");

            // decoding authToken we get decode value in 'Username:Password' format  
            return Convert.ToBase64String(bytes);
        }
        public static LoginRequestModel BasicAuthTokenDecode(this HttpRequestMessage Request, string token)
        {
            try
            {
                // decoding authToken we get decode value in 'Username:Password' format  
                var decodeauthToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token));

                if (!string.IsNullOrEmpty(decodeauthToken))
                {
                    // spliting decodeauthToken using ':'   
                    var arrUserNameandPassword = decodeauthToken.Split(':');

                    // Obtenemos el objeto de inicio de sesion
                    var loginRequest = new LoginRequestModel
                    {
                        Login = new LoginInfo
                        {
                            User = arrUserNameandPassword[0],
                            Password = arrUserNameandPassword[1],
                            Date = DateTime.Now
                        },
                        SecurityToken = token
                    };


                    //if (loginRequest.login.date < DateTime.Now.AddHours(-12))
                    //{
                    //    // El token expiró y el usuario debe volver a iniciar sesión
                    //    // el tiempo limite que usamos es de doce horas.
                    //    // es un control para aquellos casos que se reutilizan tokens
                    //    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Su sesion ha caducado. Por favor vuelva a iniciar sesion!"));
                    //}

                    return loginRequest;

                }
            }
            catch (Exception) { }

            return null;
        }
        public static async Task<OperationResult<loginSRIResponse>> LoginSRIAsync(this HttpRequestMessage Request, string ruc, string pass, string token)
        {
            var mensaje = Constants.MSG_INVALIDCREDENTIALS;
            var request = new loginSRIRequest
            {
                login = new loginSRI
                {
                    token = token,
                    user = ruc,
                    password = pass
                }
            };
            var response = await Task.FromResult(JsonWebApiHelper.ExecuteJsonWebApi<loginSRIResponse>($"{Constants.ServiceUrl}/LoginSRI", HttpMethod.Post, request));
            if (response.result != null)
            {
                if (response.result?.code == "1000")
                {
                    var result = response.ToResult(true, HttpStatusCode.OK, "OK", response.result?.message ?? "OK");
                    Logger.Log("SRI.LOGIN", result);
                    return await Task.FromResult(result);
                }
                else
                {
                    mensaje = response.result?.message;
                }
            }

            return new OperationResult<loginSRIResponse>(false, HttpStatusCode.Unauthorized, mensaje);

        }

        public static async Task<OperationResult<sriLoginResult>> LoginSRIAsync(this HttpRequestMessage Request, string ruc, string pass)
        {

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Request.BasicAuthTokenEncode(ruc, pass));
                //client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Chrome", "78.0.3904.70"));
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2;WOW64;Trident/6.0)");
                var response = await client.PostAsync("https://srienlinea.sri.gob.ec/movil-servicios/api/v2.0/secured", new StringContent(string.Empty));
                var content = await response.GetContentAsync<sriLoginResult>();
                if (!response.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(content?.codigo) || !content.codigo.Contains("007"))
                    {
                        return new OperationResult<sriLoginResult>(false, response?.StatusCode ?? HttpStatusCode.Unauthorized, content)
                        {
                            UserMessage = response?.ReasonPhrase,
                            DevMessage = content?.mensaje ?? Constants.MSG_INVALIDCREDENTIALS
                        };
                    }
                }

                var result = content.ToResult(true, HttpStatusCode.OK, "OK", content?.mensaje ?? "OK");

                Logger.Log("SRI.LOGIN", result);

                return await Task.FromResult(result);
            }

        }
        public static async Task<OperationResult<LoginResponseModel>> PerformLoginAsync(this HttpRequestMessage Request, string securityToken)
        {
            var userInfo = Request.BasicAuthTokenDecode(securityToken);

            using (HttpClient client = new HttpClient())
            {               
                var response = await client.PostAsJsonAsync($"{Constants.ServiceUrl}/login", userInfo);
                if (response.IsSuccessStatusCode)
                {
                    var model = await response.GetContentAsync<LoginResponseModel>();
                    return Request.LoadPermissions(model);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return new OperationResult<LoginResponseModel>(false, HttpStatusCode.Unauthorized, Constants.MSG_INVALIDUSERPASS) { DevMessage = $"{Constants.MSG_INVALIDUSERPASS}: {content}" };
                }
            }
        }
        public static async Task<OperationResult<PurchasePayment>> Refund(this HttpRequestMessage Request, PurchasePayment model)
        {
            try
            {
                var ecommerce = Request.GetPurchaseOrderService().GeteCommerceByCode(Constants.paymentez);

                if (!ecommerce.IsSuccess)
                {
                    return new OperationResult<PurchasePayment>(false, HttpStatusCode.InternalServerError, ecommerce.DevMessage);
                }                

                var _result = await Task.FromResult(ecommerce.Entity.RefundProcess(model));
                if (_result.IsSuccess)
                {
                    var result = _result.Entity;
                    if (result != null)
                    {
                        if (result.ContainsKey("RESPONSE_HTTP_CODE") && result.ContainsKey("RESPONSE_JSON"))
                        {
                            if (result["RESPONSE_HTTP_CODE"] == "OK")
                            {

                                var resulJson = result["RESPONSE_JSON"];
                                JavaScriptSerializer js = new JavaScriptSerializer();
                                response _response = js.Deserialize<response>(resulJson);

                                model.Refund = true;
                                model.ErrorMessage = $"{_response.status}: {_response.detail}";
                                model.Status = _response.status;
                                model.LastModifiedOn = DateTime.Now;
                                return await Task.FromResult(Request.GetPurchaseOrderService().UpdatePurchasePayment(model));
                            }
                        }
                    }
                }

                return new OperationResult<PurchasePayment>(false, HttpStatusCode.InternalServerError, "");

            }
            catch (Exception ex)
            {
                Logger.Log($"Refun.{model?.TransactionId}.{DateTime.Now}.ERROR.{DateTime.Now.Millisecond}", ex.Message, "REQUEST: ", model);
                return new OperationResult<PurchasePayment>(false, HttpStatusCode.InternalServerError, ex.Message);
            }

        }
        internal static OperationResult<Dictionary<string, string>> RefundProcess(this ECommerce commerce, PurchasePayment model)
        {
            string RESPONSE_HTTP_CODE = "RESPONSE_HTTP_CODE";
            string RESPONSE_JSON = "RESPONSE_JSON";


            //RequestBody body = RequestBody.create(JSON, json);
            // turn our request string into a byte stream
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; //TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(commerce.UrlApiRestService);
            request.ContentType = "application/json; charset=utf-8";
            request.Accept = "application/json";
            request.Method = "POST";
            request.Headers.Add("Auth-Token", PaymentezService.getAuthToken(commerce.ServerAppCode, commerce.ServerAppKey));

            StreamWriter streamWriter = new StreamWriter(request.GetRequestStream());
            JObject o = JObject.Parse(paymentezVerifyJson(model.TransactionId));
            streamWriter.Write(o);
            streamWriter.Flush();
            Dictionary<string, string> mapResult = new Dictionary<string, string>();
            try
            {
                Logger.Log($"Refun.{model?.TransactionId}.{DateTime.Now.Millisecond}.PROCESS.OK", "Linea 289");

                HttpWebResponse reponse = (HttpWebResponse)request?.GetResponse(); 
                mapResult.Add(RESPONSE_HTTP_CODE, "" + reponse?.StatusCode);
                using (var streamReader = new StreamReader(reponse?.GetResponseStream()))
                {
                    mapResult.Add(RESPONSE_JSON, streamReader.ReadToEnd());
                }

                Logger.Log($"Refun.{model?.TransactionId}.{DateTime.Now.Millisecond}.PROCESS.OK", "Linea 296");
            }
            catch (WebException ex)
            {
                Logger.Log($"Refun.{model?.TransactionId}.PROCESS.ERROR.{DateTime.Now}", ex.Message, "REQUEST: ", model);
                using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    mapResult.Add(RESPONSE_JSON, streamReader?.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Refun.{model?.TransactionId}.PROCESS.ERROR.{DateTime.Now.Millisecond}", ex.Message, "REQUEST: ", model);
                return new OperationResult<Dictionary<string, string>>(false, HttpStatusCode.InternalServerError, ex.Message);
            }

            streamWriter.Close();
            Logger.Log($"Refun.{model?.TransactionId}.{DateTime.Now.Millisecond}.PROCESS.OK", "Linea 313");
            var result = new OperationResult<Dictionary<string, string>>(true, HttpStatusCode.OK, mapResult);
            //return mapResult;
            return result;
        }

        #region paymentez

        private static string getUniqToken(string auth_timestamp, string app_secret_key)
        {
            string uniq_token_string = app_secret_key + auth_timestamp;
            return GetHashSha256(uniq_token_string);
        }
        public static string getAuthToken(string app_code, string app_secret_key)
        {
            string auth_timestamp = "" + (int)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
            string string_auth_token = app_code + ";" + auth_timestamp + ";" + getUniqToken(auth_timestamp, app_secret_key);
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(string_auth_token);
            string auth_token = Convert.ToBase64String(toEncodeAsBytes);
            return auth_token;
        }

        public static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;

            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }

        public static string paymentezVerifyJson(string transaction_id)
        {
            return "{" +
                        "\"transaction\": {" +
                            "\"id\": \"" + transaction_id + "\"" +
                        "}" +
                    "}";
        }
        #endregion

        /// <summary>
        /// Realiza la comprobacion para determinar que un token es valido!
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        internal static OperationResult<LoginResponseModel> ValidateClientToken(this HttpRequestMessage Request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = client.PostAsJsonAsync($"{Constants.ServiceUrl}/validateToken", new { client_token = token }).Result;
                var errorMsg = Constants.MSG_INVALIDCREDENTIALS;

                if (response.IsSuccessStatusCode)
                {
                    var model = response.GetContent<LoginResponseModel>();

                    if (model != null)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return Request.LoadPermissions(model);
                        }

                        errorMsg = model.Error?.Message ?? model.Result?.Message ?? Constants.MSG_INVALIDCREDENTIALS;
                    }
                }

                var content = response?.Content?.ReadAsStringAsync()?.Result;
                Logger.Log("SESSION.VALIDATETOKEN", $"Hubo un error al procesar la petición: {errorMsg}", content);

                return new OperationResult<LoginResponseModel>(false, HttpStatusCode.Unauthorized, errorMsg);
            }
        }

        /// <summary>
        /// Realiza la comprobacion para determinar que un token es valido y validar la subscripcion del !
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        internal static OperationResult<LoginResponseModel> ValidateSubscription(this HttpRequestMessage Request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = client.PostAsJsonAsync($"{Constants.ServiceUrl}/validateToken", new { client_token = token }).Result;
                var errorMsg = Constants.MSG_INVALIDCREDENTIALS;

                if (response.IsSuccessStatusCode)
                {
                    var model = response.GetContent<LoginResponseModel>();

                    if (model != null)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            //model.Subscription = Request.ValidateActivationAccount(model); ojo con esto 
                            return Request.LoadPermissions(model);
                        }

                        errorMsg = model.Error?.Message ?? model.Result?.Message ?? Constants.MSG_INVALIDCREDENTIALS;
                    }
                }

                var content = response?.Content?.ReadAsStringAsync()?.Result;
                Logger.Log("SESSION.VALIDATETOKEN", $"Hubo un error al procesar la petición: {errorMsg}", content);

                return new OperationResult<LoginResponseModel>(false, HttpStatusCode.Unauthorized, errorMsg);
            }
        }

        /// <summary>
        /// Validar y retonar los mensaje del certiificado
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="issuer"></param>
        /// <returns></returns>
        internal static bool ValidateCertificado(this HttpRequestMessage Request, Issuer issuer)
        {

            if (!string.IsNullOrEmpty(issuer.Certificate) && issuer.CertificateExpirationDate != null)
            {
                var Fech = issuer.CertificateExpirationDate;
                var DifFec = Fech.Value - DateTime.Now;
                var dias = Convert.ToInt32(DifFec.TotalDays);
                if (dias == 28 || dias == 18 || dias == 10 || dias == 0)
                {
                    issuer.EmailEsignNotification(dias);
                }
            }

            return true;
        }

        /// <summary>
        /// Validar y retonar los mensaje del certiificado
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="ruc"></param>
        /// <returns></returns>
        internal static Subscription IssuerSubscription(this HttpRequestMessage Request, string ruc)
        {

            if (!string.IsNullOrEmpty(ruc) && ruc != null)
            {
                return Request.GetSubscriptionService().GetSubscription(ruc);
            }

            return null;
        }



        /// <summary>
        /// Carga los permisos dentro de la aplicacion Express de un usuario
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        internal static OperationResult<LoginResponseModel> LoadPermissions(this HttpRequestMessage Request, LoginResponseModel model)
        {
            if (model != null)
            {
                // Si el resultado fue un error entonces debe devolver el mensaje
                if (model.Client == null)
                {
                    var errMsg = Constants.MSG_INVALIDUSERPASS;

                    if (model.Result != null && !string.IsNullOrEmpty(model.Result.Message))
                    {
                        errMsg = model.Result.Message;
                    }

                    return new OperationResult<LoginResponseModel>(false, HttpStatusCode.Unauthorized, errMsg);
                }

                var userPermissions = Request.GetUserService().GetIssuersByUser(model.Client.Username);
                var perm = userPermissions.Find(m => m.Issuer.RUC.StartsWith(model.Client.Username));

                // Si no se encuentra definido un permiso para el usuario actual
                // se agrega el predeterminado.
                if (perm == null)
                {
                    // Se define el RUC que perteneceria de forma predeterminada al usuario.
                    var userRuc = (model.Client.Username.Trim()) +
                        ((model.Client.Username.Trim().Length == 13) ? string.Empty : "001");

                    var issuer = Request.GetIssuersService().GetIssuer(userRuc);

                    // Si existe un emisor con el ruc del usuario
                    if (issuer != null)
                    {
                        userPermissions.Add(new UserPermissions
                        {
                            Id = 0,
                            Issuer = issuer,
                            IssuerId = issuer.Id,
                            Username = model.Client.Username,
                            Role = UserRolEnum.Admin,
                            Modules = "admin"
                        });
                    }
                }

                model.Issuers = new List<LoginResponseModel.LoginIssuerModel>();
                foreach (var item in userPermissions)
                {
                    model.Issuers.Add(new LoginResponseModel.LoginIssuerModel
                    {
                        RUC = item.Issuer.RUC,
                        BusinessName = string.IsNullOrEmpty(item.Issuer.BussinesName) ? item.Issuer.TradeName : item.Issuer.BussinesName,
                        Email = model.Client.Email,
                        Name = item.Issuer.TradeName,
                        PK = item.Issuer.Id.ToString(),
                        UserRole = item.Role,
                        Modules = item.Modules
                    });
                }

                return new OperationResult<LoginResponseModel>(true, HttpStatusCode.OK, model);
            }

            return new OperationResult<LoginResponseModel>(false, HttpStatusCode.Unauthorized, Constants.MSG_INVALIDCREDENTIALS);
        }


        /// <summary>
        /// Este metodo obtiene el token predeterminado del usuario en la APP para obtener la informacion concerniente a sus datos.
        /// </summary>
        /// <param name="ruc"></param>
        /// <returns></returns>
        public static string GetIssuerToken(this HttpRequestMessage Request, string ruc)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.PostAsJsonAsync($"{Constants.ServiceUrl}/login2",
                        new { user = ruc, password = Constants.AppToken }).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var model = response.GetContent<LoginResponseModel>();

                        if (model != null)
                        {
                            if (model.Client != null)
                            {
                                return model.Client.ClientToken;
                            }
                        }
                    }
                }
            }
            catch { }

            return default;
        }
        public static LoginResponseModel RegisterUser(this HttpRequestMessage Request, Issuer model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var registerModel = new
                    {
                        businessName = model.BussinesName,
                        email = model.Email,
                        name = model.BussinesName,
                        streetAddress = model.MainAddress,
                        id = model.RUC,
                        password = Constants.AppToken, // Default Password must be changed
                        phone = model.Phone,
                        dependents = 0,
                        appname = "WEB",
                        origin = ""
                    };

                    var response = client.PostAsJsonAsync($"{Constants.ServiceUrl}/Register", new { new_client = registerModel }).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return response.GetContent<LoginResponseModel>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("REGISTER.ISSUER", ex);
            }

            return default;
        }
        public static OperationResult<SubscriptionDto> ValidateActivationAccount(this HttpRequestMessage Request, string RUC)
        {
            try
            {
                int requestElectronicSign = 0;
                var subscription = Request.GetSubscriptionService().GetSubscription(RUC);
                var Notificar = false;
                if (subscription != null)
                {
                    //var subscription = new Subscription();
                    var fechExpLic = new DateTime();
                    var Dias = 0;
                    var subExpdate = subscription.SubscriptionExpirationDate ?? DateTime.Now;
                    fechExpLic = new DateTime(subExpdate.Year, subExpdate.Month, subExpdate.Day, 23, 59, 59);
                    var DifFec = fechExpLic - DateTime.Now;
                    Dias = Convert.ToInt32(DifFec.TotalDays);
                    if (subscription.Status != SubscriptionStatusEnum.ValidatingPayment && subscription.Status != SubscriptionStatusEnum.Saved)
                    {
                        //valida si la suscripción le faltan menos de 30 dias para su desactivación
                        if (Dias <= 30)
                        {
                            Notificar = true;
                            if (Dias <= 0)
                            {
                                subscription.Status = SubscriptionStatusEnum.Inactiva;
                                subscription.LastModifiedOn = DateTime.Now;
                                subscription.StatusMsg = "Tu suscripción ha caducado!";
                            }
                            else
                            {
                                subscription.StatusMsg = $"Tu suscripción caducara en {Dias} día(s).";
                                subscription.Status = SubscriptionStatusEnum.Activa;
                            }

                            var resp = Request.GetSubscriptionService().UpdateSubscription(subscription);
                            if (resp.IsSuccess)
                            {
                                subscription = resp.Entity;
                            }

                            if ((Dias == 30 || Dias == 1))
                            {
                                subscription.Issuer.EmailActivationAccount(subscription, 0);
                            }
                        }                        
                        //valida si la suscripción ah llegado al limites de documentos emitidos
                        if (subscription.AmountDocument != Constants.AmountDocIlimitado)
                        {

                            if (subscription.BalanceDocument <= 0)
                            {
                                if (subscription.Status != SubscriptionStatusEnum.Inactiva)
                                {
                                    subscription.Status = SubscriptionStatusEnum.Inactiva;
                                    subscription.LastModifiedOn = DateTime.Now;
                                    subscription.StatusMsg = $"La cantidad de documentos de emisión de tu plan {subscription.LicenceType.Name} llego al limite!";
                                    var resp = Request.GetSubscriptionService().UpdateSubscription(subscription);
                                    if (resp.IsSuccess)
                                    {
                                        subscription = resp.Entity;
                                    }
                                }
                                Notificar = true;
                                subscription.Issuer.EmailActivationAccount(subscription, 0, "L");
                            }
                            else if (subscription.BalanceDocument > 0 && subscription.BalanceDocument <= 5)
                            {
                                subscription.StatusMsg = $"Tienes un saldo de {subscription.BalanceDocument} documentos por emitir, tu plan {subscription.LicenceType.Name}  próximamente finalizará!";
                                Notificar = true;
                                if (subscription.BalanceDocument == 5)
                                {
                                    subscription.Issuer.EmailActivationAccount(subscription, 0, "L");
                                }

                            }
                        }
                        //validar la suscripcion del plan basico si la firma si esta caducada
                        if (subscription.LicenceType.Code == Constants.PlanBasic && subscription.Status == SubscriptionStatusEnum.Activa)
                        {
                            var _issuer = Request.GetIssuersService().GetIssuer(RUC);
                            if (_issuer != null)
                            {
                                if (!string.IsNullOrEmpty(_issuer.Certificate) && _issuer.CertificateExpirationDate != null)
                                {
                                    if (_issuer.CertificateExpirationDate < DateTime.Now.AddDays(-1))
                                    {
                                        subscription.Status = SubscriptionStatusEnum.Inactiva;
                                        subscription.LastModifiedOn = DateTime.Now;
                                        subscription.StatusMsg = $"Tu suscripción de plan básico ha caducado por la vigencia de la firma configurada!";
                                        var resp = Request.GetSubscriptionService().UpdateSubscription(subscription);
                                        if (resp.IsSuccess)
                                        {
                                            subscription = resp.Entity;
                                        }

                                        Notificar = true;
                                        subscription.Issuer.EmailActivationAccount(subscription, 0, "L");
                                    }
                                }
                            }
                        }
                        if (subscription.Status == SubscriptionStatusEnum.Activa)
                        { 
                            var purchaseSubscription = Request.GetSubscriptionService().GetIPurchaseSubscriptionBySubscriptionId(subscription.Id);
                            if(purchaseSubscription.IsSuccess)
                            {
                                requestElectronicSign = Convert.ToInt32(purchaseSubscription.Entity?.RequestElectronicSign.Value);
                            }
                        }

                    }
                    else if (subscription.Status == SubscriptionStatusEnum.ValidatingPayment || subscription.Status == SubscriptionStatusEnum.Saved)
                    {
                        Notificar = true;
                    }

                    var model = subscription.ToSubscriptionDto();
                    model.RequestElectronicSign = requestElectronicSign;
                    model.StatusMsg = (model.Status == SubscriptionStatusEnum.Saved ? "Pago pendiente" : model.StatusMsg);
                    model.Day = Dias;
                    model.Notify = Notificar;
                    return new OperationResult<SubscriptionDto>(true, HttpStatusCode.OK, model);
                }

                return new OperationResult<SubscriptionDto>(false, HttpStatusCode.Unauthorized, $"El ruc: {RUC} no tiene subscripción activa.");

            }
            catch (Exception ex)
            {
                return new OperationResult<SubscriptionDto>(false, HttpStatusCode.Unauthorized, ex.Message);
            }

        }
        public static OperationResult<PurchaseOrder> PurchaseOrderSubscription(this HttpRequestMessage Request, Issuer issuer, Subscription subscription)
        {
            // Solo se puede procesar una solicitud por usuario

            var price = Constants.Subscription.Price;
            var ivaProduct = decimal.Round(price * 0.12M, 2);
            var total = price + ivaProduct;
            var order = new PurchaseOrder
            {
                BusinessName = issuer.BussinesName,
                Identification = issuer.RUC,
                Address = issuer.MainAddress,
                Email = issuer.Email,
                Phone = issuer.Phone,
                IssuedOn = DateTime.Today,
                Products = "Suscripción de cuenta(Periodo anual)",
                Subtotal0 = 0,
                Subtotal12 = price,
                IVA = ivaProduct,
                ICE = 0,
                Interests = 0,
                Additional = 0,
                Total = total,
                ZIP = "000",
                Status = PurchaseOrderStatusEnum.Saved,
                IssuerId = issuer.Id,
                FirstName = issuer.BussinesName,
                LastName = issuer.BussinesName,
                City = "GUAYAQUIL",
                Province = "GUAYAS",
                Country = "EC",
            };
            var result = Request.GetPurchaseOrderService().Add(order);
            if (result.IsSuccess)
            {
                var _purchSubsc = new PurchaseSubscription()
                {
                    IssuerId = issuer.Id,
                    CreatedOn = DateTime.Now,
                    PurchaseOrderId = result.Entity.PurchaseOrderId,
                    SubscriptionId = subscription.Id,
                    Status = PurchaseOrderSubscriptionStatusEnum.Saved,
                    InvoiceId = 0
                };
                Request.GetSubscriptionService().AddPurchaseSubscription(_purchSubsc);
            }

            return result;
        }
        public static async Task<OperationResult<LoginResponseModel>> RegisterUsers(this HttpRequestMessage Request, RegisterClientModel model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var registerModel = new
                    {
                        businessName = model.BusinessName,
                        email = model.Email,
                        name = model.Name,
                        streetAddress = model.StreetAddress,
                        id = model.Username,
                        password = model.Password, // Default Password must be changed
                        phone = model.Phone,
                        dependents = 0,
                        appname = model.AppName,
                        origin = ""
                    };

                    var response = client.PostAsJsonAsync($"{Constants.ServiceUrl}/Register", new { new_client = registerModel }).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return new OperationResult<LoginResponseModel>(true, HttpStatusCode.OK, response.GetContent<LoginResponseModel>());
                    }

                    return new OperationResult<LoginResponseModel>(false, HttpStatusCode.NotFound, response.GetContent<LoginResponseModel>());
                }

                throw new Exception("Hubo un error al registrar el usuario!");
            }
            catch (Exception ex)
            {
                Logger.Log("REGISTER.ISSUER", ex);
                return new OperationResult<LoginResponseModel>(false, HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        public static async Task<OperationResult<UserPayment>> RegisterUserPayment(this HttpRequestMessage Request, RegisterClientModel model)
        {
            try
            {
                var genrarToken = false;
                var plan = Request.GetCatalogsService().GetLicenceTypes().Where(p => p.Code == model.codProducto)?.FirstOrDefault();
                var userPayment = new UserPayment
                {
                    Identification = model.Username,
                    BussinesName = model.Name,
                    LicenceTypeId = plan.Id,
                    NamePlan = plan.Name ?? "",
                    StatusMsg = $"Pago del Plan {plan.Name}",
                    OrderNumber = model.numeroPedido ?? ""
                };

                var response = Request.GetUserService().AddUserPayment(userPayment);
                if (response.IsSuccess)
                {
                    #region configuración del emisor
                    // Validar si existe el usuario, si no se crea.
                    var ruc = model.Username.Length == 10 ? $"{model.Username}001" : model.Username;
                    var issuer = Request.GetIssuersService().GetIssuer(ruc);
                    if (issuer == null)
                    {
                        var sri = new SRIContrib
                        {
                            RUC = ruc,
                            BusinessName = model.Name,
                            TradeName = model.Name,
                            CreatedOn = DateTime.Today,
                            Status = SRIContribStatus.Pasive,
                            Establishments = new List<Establishment>{
                                new Establishment{
                                    EstablishmentNumber = "1",
                                    CommercialName = "",
                                    Street = model.StreetAddress
                                }
                            }
                        };
                        issuer = sri.ToIssuer();
                        issuer.Phone = model.Phone;
                        issuer.Email = model.Email;
                        var result = Request.GetIssuersService().AddIssuer(issuer, issuer.RUC);
                        genrarToken = true;
                    }
                    if (issuer?.Id > 0)
                    {
                        #region suscripción
                        var _subscriptionLog = new SubscriptionLog();
                        var subscription = Request.GetSubscriptionService().GetSubscription(issuer.RUC);
                        if (subscription == null)
                        {
                            subscription = new Subscription
                            {
                                IssuerId = issuer.Id,
                                LicenceTypeId = plan.Id,
                                RUC = issuer.RUC,
                                Status = SubscriptionStatusEnum.ValidatingPayment,
                                StatusMsg = "Pago realizado por WooCommerce",
                                AmountDocument = plan.AmountDocument,
                                IssuedDocument = null,
                                BalanceDocument = null
                            };
                        }
                        else
                        {
                            _subscriptionLog = new SubscriptionLog {
                                RUC = subscription.RUC,
                                IssuerId = subscription.IssuerId,
                                SubscriptionId = subscription.Id,
                                LicenceTypeId = subscription.LicenceTypeId,
                                SubscriptionStartDate = subscription.SubscriptionStartDate,
                                SubscriptionExpirationDate = subscription.SubscriptionExpirationDate,
                                IssuedDocument = subscription.IssuedDocument ?? 0,
                                BalanceDocument = subscription.BalanceDocument ?? 0,
                                Observation = subscription.StatusMsg,
                                Status = subscription.Status,
                            };

                            subscription.Status = SubscriptionStatusEnum.ValidatingPayment;
                            subscription.StatusMsg = "Pago realizado por WooCommerce";
                            subscription.LicenceTypeId = plan.Id;
                            subscription.LastModifiedOn = DateTime.Now;
                            subscription.AmountDocument = plan.AmountDocument;
                            subscription.IssuedDocument = null;
                            subscription.BalanceDocument = null;
                        }

                        #endregion

                        #region cupones y descuentos
                        var coupon = new ReferenceCodes();
                        if (!string.IsNullOrWhiteSpace(model.discountCoupon))
                        {
                            coupon = Request.GetPurchaseOrderService().GetReferenceCodes(model.discountCoupon)?.Entity;
                        }
                        #endregion

                        #region generamos la orden de compra 
                        decimal totalDiscount = 0;
                        decimal price = plan.Price;
                        if (coupon?.Id > 0)
                        {
                            totalDiscount = decimal.Round(price * (coupon.DiscountRate / 100), 2, MidpointRounding.AwayFromZero);
                            price -= decimal.Round(price * (coupon.DiscountRate / 100), 2, MidpointRounding.AwayFromZero);
                        }
                        var ivaProduct = decimal.Round(price * (plan.TaxBase / 100), 2, MidpointRounding.AwayFromZero);
                        var total = price + ivaProduct;

                        var order = new PurchaseOrder
                        {
                            BusinessName = issuer.BussinesName,
                            Identification = issuer.RUC,
                            Address = issuer.MainAddress,
                            Email = issuer.Email,
                            Phone = issuer.Phone,
                            IssuedOn = DateTime.Today,
                            Products = $"Plan {plan.Name}({(plan.Code == "L03" ? "Emisión ilimitada" : plan.AmountDocument)} documentos), {plan.Description?.Split('|')[2]}",
                            Subtotal0 = 0,
                            Subtotal12 = price,
                            IVA = ivaProduct,
                            ICE = 0,
                            Interests = 0,
                            Additional = 0,
                            Total = total,
                            ZIP = "000",
                            Status = PurchaseOrderStatusEnum.Transfer,
                            IssuerId = issuer.Id,
                            FirstName = issuer.BussinesName,
                            LastName = issuer.BussinesName,
                            City = issuer.City ?? "GUAYAQUIL",
                            Province = issuer.Province ?? "GUAYAS",
                            Country = "EC",
                            TotalDiscount = totalDiscount,
                            Discount = 0,
                            BeniReferCodeId = null,
                            CreatedOn = DateTime.Now,
                            DiscountCode = coupon.Code ?? "",
                            BankTransfer = true
                        };


                        //Se genera la orden de compra para la suscripción :                       
                        var _purchaseSubscription = new PurchaseSubscription()
                        {
                            Status = PurchaseOrderSubscriptionStatusEnum.ValidatingPayment,
                            IssuerId = issuer.Id,
                            CreatedOn = DateTime.Now,
                            LicenceTypeId = plan.Id,
                            UserPaymentId = userPayment.Id,
                            RequestElectronicSign = RequestElectronicSignEnum.Pending,
                            RequestElectronicSignMsg = RequestElectronicSignEnum.Pending.GetDisplayValue(),
                            PurchaseOrder = order,
                            Subscription = subscription
                        };
                        var resulPurSubscription = await Task.FromResult(Request.GetSubscriptionService().BuySubscription(_purchaseSubscription));
                        if (resulPurSubscription.IsSuccess)
                        {
                            resulPurSubscription.Entity.PaymentType = PaymentTypeEnum.CreditCard;
                            await Request.GetSubscriptionService().GenerateInvoice(resulPurSubscription.Entity);

                            if (coupon?.Id > 0)
                            {
                                var benyRefeCode = new BeneficiaryReferenceCode()
                                {
                                    DiscountCode = coupon.Code,
                                    Identification = issuer.RUC,
                                    ReferenceCodId = coupon.Id,
                                    IssuerId = issuer.Id,
                                    Status = ReferenceCodeStatusEnum.Applied,
                                    StatusMsg = $"Aplicado:Código de descuento por {coupon.Description}",
                                    LastModifiedOn = DateTime.Now,
                                    BeneficiaryId = 0
                                };
                                await Task.FromResult(Request.GetPurchaseOrderService().AddBeneficiaryReferenceCode(benyRefeCode));
                            }
                            if (genrarToken)
                            {
                                var userToken = $"{Guid.NewGuid()}-{issuer.RUC}";
                                await Task.FromResult(Request.GetIRequestSessionsService().GenerateSession(issuer.RUC, issuer.RUC, userToken));
                            }

                        }

                        #endregion
                    }

                    #endregion
                }

                return response;

            }
            catch (Exception ex)
            {
                Logger.Log("REGISTER.ISSUER", ex);
                return new OperationResult<UserPayment>(false, HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// Permite desvincular un emisor del usuario
        /// </summary>
        /// <param name="model"></param>    
        /// <param name="Request"></param>  
        /// <returns></returns>
        public static async Task<OperationResult<List<LoginIssuerModel>>> RevokePermissions(this HttpRequestMessage Request, UserPermissionsModel model)
        {

            var _issuer = Request.GetIssuersService().GetIssuer(model.RUC);
            if (_issuer == null)
            {
                return await Task.FromResult(new OperationResult<List<LoginIssuerModel>>(false, HttpStatusCode.RequestEntityTooLarge, $"El ruc {model.RUC} no esta registrado en el sistema.."));
            }

            var result = Request.GetUserService().RevokePermissions(model.Username, _issuer.Id);
            if (result.IsSuccess)
            {
                var userPermissions = Request.GetUserService().GetIssuersByUser(model.Username);
                var perm = userPermissions.Find(m => m.Issuer.RUC.StartsWith(model.Username));

                // Si no se encuentra definido un permiso para el usuario actual
                // se agrega el predeterminado.
                if (perm == null)
                {
                    // Se define el RUC que perteneceria de forma predeterminada al usuario.
                    var userRuc = (model.Username.Trim()) +
                        ((model.Username.Trim().Length == 13) ? string.Empty : "001");

                    var issuer = Request.GetIssuersService().GetIssuer(userRuc);

                    // Si existe un emisor con el ruc del usuario
                    if (issuer != null)
                    {
                        userPermissions.Add(new UserPermissions
                        {
                            Id = 0,
                            Issuer = issuer,
                            IssuerId = issuer.Id,
                            Username = model.Username,
                            Role = UserRolEnum.Admin,
                            Modules = "admin"
                        });
                    }
                }

                var Issuers = new List<LoginIssuerModel>();
                foreach (var item in userPermissions)
                {
                    Issuers.Add(new LoginIssuerModel
                    {
                        RUC = item.Issuer.RUC,
                        BusinessName = string.IsNullOrEmpty(item.Issuer.BussinesName) ? item.Issuer.TradeName : item.Issuer.BussinesName,
                        Email = item.Issuer.Email,
                        Name = item.Issuer.TradeName,
                        PK = item.Issuer.Id.ToString(),
                        UserRole = item.Role,
                        Modules = item.Modules
                    });
                }

                return await Task.FromResult(new OperationResult<List<LoginIssuerModel>>(true, HttpStatusCode.OK, Issuers));
            }

            var errorMsg = result?.DevMessage ?? $"Error al desvincular el ruc {model.RUC}";

            return await Task.FromResult(new OperationResult<List<LoginIssuerModel>>(false, HttpStatusCode.InternalServerError, errorMsg));
        }

        #region Procesar suscripción
        public static async Task<OperationResult<PurchaseSubscription>> SubscriptionProcess(this HttpRequestMessage Request, int purchaseOrderId, IssuerDto issuer, string bank)
        {
            var enviarEmail = false;
            string typeEmail = "";
            var purchaseOrder = Request.GetSubscriptionService().GetSubscriptionByPurchase(purchaseOrderId);
            var message = "La suscripción anual especificada ya fue facturada";
            try
            {                
                if (purchaseOrder.IsSuccess)
                {
                    if (purchaseOrder.Entity.IssuerId != issuer.Id)
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.Unauthorized, "Usted no esta autorizado para generar este documento");
                    }
                    if (!purchaseOrder.Entity.InvoicePrrocessed)
                    {
                        Logger.Log($"LOG.{purchaseOrder.Entity.Subscription.RUC}.ORDERID.{purchaseOrder.Entity.PurchaseOrderId}.{DateTime.Now.ToFileTime()}", "Request Message: ", purchaseOrder);
                        var _result = await Task.FromResult(Request.GetSubscriptionService().PurchaseSubscriptionInvoiceProcessed(purchaseOrder.Entity.Id));
                        purchaseOrder.Entity.InvoicePrrocessed = _result.IsSuccess && _result.Entity.InvoicePrrocessed;
                        purchaseOrder.Entity.PurchaseOrder.Bank = bank;
                        if (purchaseOrder.Entity.InvoiceId == null ||purchaseOrder.Entity.InvoiceId == 0)
                        {
                            if (purchaseOrder.Entity.PurchaseOrder == null)
                            {
                                var _purchaseOrder = await Task.FromResult(Request.GetPurchaseOrderService().GetPurchaseOrderById(purchaseOrder.Entity.PurchaseOrderId));
                                purchaseOrder.Entity.PurchaseOrder = _purchaseOrder.Entity;
                            }
                            var _invoice = await Request.GenerateInvoice(purchaseOrder.Entity);
                            if (_invoice.IsSuccess)
                            {
                                var plan = await Task.FromResult(Request.GetSubscriptionService().GetLicenceTypeById(purchaseOrder.Entity.LicenceTypeId.Value));
                                purchaseOrder.Entity.InvoiceResult = _invoice.Entity?.Reason;
                                purchaseOrder.Entity.InvoiceDate = _invoice?.Entity?.IssuedOn;
                                purchaseOrder.Entity.InvoiceId = _invoice?.Entity?.Id;
                                purchaseOrder.Entity.InvoiceNumber = _invoice?.Entity?.DocumentNumber;
                                purchaseOrder.Entity.LastModifiedOn = DateTime.Now;
                                purchaseOrder.Entity.Status = PurchaseOrderSubscriptionStatusEnum.Payed;
                                purchaseOrder.Entity.PaymentType = purchaseOrder.Entity.PaymentType ?? PaymentTypeEnum.BankTransfer;
                                purchaseOrder.Entity.RequestElectronicSign = plan.IncludeCertificate ? RequestElectronicSignEnum.Pending : RequestElectronicSignEnum.NotInclude;
                                purchaseOrder.Entity.RequestElectronicSignMsg = plan.IncludeCertificate ? RequestElectronicSignEnum.Pending.GetDisplayValue() : RequestElectronicSignEnum.NotInclude.GetDisplayValue();

                                //actualizar la orden
                                purchaseOrder.Entity.PurchaseOrder.InvoiceDate = _invoice?.Entity?.IssuedOn;
                                purchaseOrder.Entity.PurchaseOrder.InvoiceNumber = _invoice?.Entity?.DocumentNumber;
                                purchaseOrder.Entity.PurchaseOrder.LastModifiedOn = DateTime.Now;
                                purchaseOrder.Entity.PurchaseOrder.Status = PurchaseOrderStatusEnum.Payed;

                                //actualizar la suscripción
                                var expirationDate = DateTime.Now.AddYears(1);
                                var _subscription = await Task.FromResult(Request.GetSubscriptionService().GetSubscription(purchaseOrder.Entity.SubscriptionId));
                                if (_subscription.Status == SubscriptionStatusEnum.Activa && _subscription.SubscriptionExpirationDate > DateTime.Now && _subscription.LicenceType.Code != Constants.PlanBasic)
                                {
                                    TimeSpan totalDias = _subscription.SubscriptionExpirationDate.Value - DateTime.Now;
                                    var dias = (int)totalDias.TotalDays;
                                    if (dias > 0)
                                    {
                                        expirationDate = expirationDate.AddDays(dias + 1);
                                    }
                                }
                                purchaseOrder.Entity.SubscriptionLog = new SubscriptionLog
                                {
                                    RUC = _subscription.RUC,
                                    IssuerId = _subscription.IssuerId,
                                    SubscriptionId = _subscription.Id,
                                    LicenceTypeId = _subscription.LicenceTypeId,
                                    SubscriptionStartDate = _subscription.SubscriptionStartDate ?? DateTime.Now,
                                    SubscriptionExpirationDate = _subscription.SubscriptionExpirationDate ?? expirationDate,
                                    IssuedDocument = _subscription.IssuedDocument,
                                    BalanceDocument = _subscription.BalanceDocument,
                                    Observation = _subscription.StatusMsg,
                                    Status = _subscription.Status,
                                };

                                _subscription.Status = SubscriptionStatusEnum.Activa;
                                _subscription.SubscriptionStartDate = DateTime.Now;
                                _subscription.SubscriptionExpirationDate = expirationDate;
                                _subscription.LastModifiedOn = DateTime.Now;
                                _subscription.StatusMsg = "Ok";
                                _subscription.LicenceTypeId = plan.Id;
                                _subscription.AmountDocument = plan.AmountDocument;
                                _subscription.IssuedDocument = null;
                                _subscription.BalanceDocument = null;
                                purchaseOrder.Entity.Subscription = _subscription;
                                enviarEmail = true;
                            }
                            else
                            {
                                purchaseOrder.Entity.InvoiceResult = $"{DateTime.Now} - No se generó la factura para esta solicitud \r\n{_invoice.UserMessage ?? purchaseOrder.Entity.InvoiceResult}.";
                            }

                            var result = await Task.FromResult(Request.GetSubscriptionService().UpdatePurchaseSubscription(purchaseOrder.Entity));
                            if (enviarEmail)
                            {
                                var valor = purchaseOrder.Entity?.UserPaymentId > 0 ? "A" : "";
                                if (!string.IsNullOrWhiteSpace(typeEmail))
                                { valor = "M"; }
                                purchaseOrder.Entity.Subscription.LicenceType = new LicenceType { Name = purchaseOrder.Entity.PurchaseOrder.Products };
                                purchaseOrder.Entity.Subscription.Issuer.EmailActivationAccount(purchaseOrder.Entity.Subscription, 0, valor);
                            }

                            return await Task.FromResult(result);
                        }                       
                    }                    
                    Logger.Log($"SUBSCRIPTION.{purchaseOrder.Entity?.Subscription?.RUC}.{DateTime.Now.ToFileTime()}.INVOICE.ERROR", message, purchaseOrder);
                    return await Task.FromResult(purchaseOrder.Entity.ToResult(false, HttpStatusCode.Conflict, message));
                }

                throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "No se encuentra la orden de compra");
            }
            catch (Exception ex)
            {
                Logger.Log($"SUBSCRIPTION.{purchaseOrder.Entity?.Subscription?.RUC}.{DateTime.Now.ToFileTime()}.INVOICE.ERROR", ex);
                return new OperationResult<PurchaseSubscription>(false, HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private static async Task<OperationResult<Document>> GenerateInvoice(this HttpRequestMessage Request, PurchaseSubscription purchaseSubscription)
        {
            var _request = purchaseSubscription.PurchaseOrder?.ToInvoiceRequest(null);
            var product = purchaseSubscription.PurchaseOrder.Products.Contains("Firma Electrónica") ? "ELECTRONICSIGN" : "SUBSCRIPTION";
            try
            {
                if (_request != null)
                {
                    #region validar data de la factura
                    var _session = Request.GetService<IRequestSessionsService>().GetSessionByToken(Constants.NexusToken);
                    if (_session == null)
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, null, "El token es inválido o ha expirado!");
                    }
                    var _issuer = Request.GetIssuersService().GetIssuer(_session.IssuerId);
                    _request.ContributorId = Request.ValidateContributor(_request, _issuer);
                    if (!Request.ValidateDetails(_request.Details, _issuer, out string errormsg))
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, errormsg, "Documento Invalido!");
                    } 
                    var document = _request.ToDocument2(_issuer,DocumentTypeEnum.Invoice.GetCoreValue());                   
                    document.PurchaseOrder =purchaseSubscription.PurchaseOrderId.ToString();
                    document.InvoiceInfo = document.InvoiceInfo.Build2(_issuer, _request);
                    #endregion validar data de la factura

                    #region validar si hay factura a la orden
                    var _invoiceResult  = Request.GetDocumentService().GetAllIssuerDocuments(_issuer.Id);
                    var _invoice = _invoiceResult.Where(doc => doc.PurchaseOrder == purchaseSubscription.PurchaseOrderId.ToString())
                                                 .FirstOrDefault();
                    if(_invoice != null)
                    {
                        throw Request.BuildHttpErrorException(HttpStatusCode.BadRequest, "La suscripción anual especificada ya fue facturada");
                    }
                    #endregion validar si hay factura a la orden

                    #region generar factura
                    var result = Request.GetDocumentService().AddDocument(_issuer.Id, document);
                    if (!result.IsSuccess)
                    {
                        throw Request.BuildHttpErrorException(result.StatusCode, result.DevMessage, result.UserMessage);
                    }
                    Logger.Log($"LOG.{_issuer.RUC}.ORDERID.{purchaseSubscription.PurchaseOrderId}.INVOICE.CREATE.{DateTime.Now.ToFileTime()}", "Request Message: ", _request);
                    return await Task.FromResult(new OperationResult<Document>(true, HttpStatusCode.OK, result.Entity));
                    #endregion generar factura
                }

                throw new Exception("Hubo un error al facturar la orden de compra especificada."); 
                
            }
            catch (Exception ex)
            {
                Logger.Log($"{product}.{purchaseSubscription.Subscription.RUC}.INVOICE.ERROR", $"Hubo un error al generar la factura", ex, "FACTURA:", _request, "Orden de Compra:", purchaseSubscription);
                return new OperationResult<Document>(false, HttpStatusCode.InternalServerError, $"Hubo un error al generar la factura") { DevMessage = ex.ToString() };
            }

        }

        private static long ValidateContributor(this HttpRequestMessage Request, InvoiceRequestModel model, Issuer issuer)
        {
            long _contributorId = 0;
            // Valida si el contribuyente existe :
            // como dato adicional valida si es consumidor final cuando todos los numeros son 9. ej: 9999999999999
            if (!string.IsNullOrEmpty(model.Identification) && !model.Identification.All(c => c == '9'))
            {               
                var contributor = Request.GetContributorsService().GetContributorByRUC(model.Identification, issuer.Id);
                if (contributor == null)
                {
                    // Verificamos el tipo de identificacion
                    var contribIdType = Request.GetCatalogsService().GetIdentificationTypes().FirstOrDefault(p => p.SriCode == model.IdentificationType);
                    // Crear Contribuyente:
                    contributor = new Contributor
                    {
                        Id = 0,
                        Identification = model.Identification,
                        BussinesName = model.ContributorName,
                        TradeName = model.ContributorName,
                        Phone = model.Phone ?? "2999999",
                        Address = model.Address ?? "Av. Principal",
                        IssuerId = issuer.Id,
                        IdentificationTypeId = contribIdType.Id,
                        EmailAddresses = model.EmailAddresses ?? issuer.Email ?? "",
                        IsCustomer = true,
                        IsSupplier = false
                    };

                    //Realizamos el proceso de guardado del contribuyente:
                    var result = Request.GetContributorsService().Add(contributor);

                    if (result != null)
                    {
                        _contributorId = result.Entity.Id;
                    }
                }

                _contributorId = contributor.Id;
            }

            return _contributorId;

        }

        private static bool ValidateDetails(this HttpRequestMessage Request, IEnumerable<DocumentDetailModel> documentDetails, Issuer issuer, out string errormsg)
        {
            // VALIDACION DE LOS DETALLES DEL DOCUMENTO
            foreach (var detailModel in documentDetails)
            {
                Product product = null;
                // Si el id del producto es nulo, entonces se tiene que validar la informacion enviada:
                if (detailModel.ProductId == 0)
                {
                    // Primero: Busca el producto por el codigo:
                    product = Request.GetProductsService().GetProductByCode(detailModel.MainCode, issuer.Id);

                    // Si no se encuentra por codigo
                    if (product == null || string.IsNullOrEmpty(detailModel.MainCode))
                    {
                        // Segundo: Busca el producto por descripcion:
                        product = Request.GetProductsService().SearchProducts(detailModel.Description ?? "-", issuer.Id).FirstOrDefault();
                    }

                    // Si se especifica el nombre del producto y el producto no existe, 
                    // entonces se lo tiene que generar automaticamente
                    if (product == null)
                    {
                        if (!string.IsNullOrEmpty(detailModel.Description))
                        {
                            // Si se especifico los datos especificos del producto entonces se lo crea automaticamente
                            product = Request.GetProductsService().AddProduct(new Product
                            {
                                MainCode = detailModel.MainCode,
                                AuxCode = detailModel.AuxCode,
                                Name = detailModel.Description,
                                IssuerId = issuer.Id,
                                UnitPrice = 0M,
                                IsEnabled = true,
                                ProductTypeId = 1,
                                IvaRateId = 1,
                                IceRateId = 1
                            }).Entity;
                        }
                        else
                        {
                            errormsg = $"No se ha especificado la descripcion del producto: {JsonConvert.SerializeObject(detailModel)}";
                            return false;
                        }
                    }
                }
                else
                {
                    product = Request.GetProductsService().GetIssuerProduct(detailModel.ProductId, issuer.Id);
                }

                // Si todo va bien tiene que completar la informacion del producto en el documento
                if (product != null)
                {
                    detailModel.ProductId = product.Id;
                    detailModel.Description = string.IsNullOrEmpty(detailModel.Description) ? product.Name : detailModel.Description;
                    detailModel.MainCode = string.IsNullOrEmpty(detailModel.MainCode) ? product.MainCode : detailModel.MainCode;
                    detailModel.AuxCode = string.IsNullOrEmpty(detailModel.AuxCode) ? product.AuxCode : detailModel.AuxCode;
                }
                else
                {
                    errormsg = $"No se ha especificado los datos del producto: {JsonConvert.SerializeObject(detailModel)}";
                    return false;
                }


                // PROCESO DE VERIFICACION DE LOS DETALLES DE LOS IMPUESTOS
                // EN EL CASO DE QUE ESTOS NO EXISTAN PERO SE ENVIA LA INFORMACION
                if (detailModel.Taxes == null || detailModel.Taxes.Count == 0)
                {
                    detailModel.Taxes = new List<TaxModel>();


                    if (string.IsNullOrEmpty(detailModel.ValueAddedTaxCode))
                    {
                        detailModel.ValueAddedTaxCode = "0";
                    }

                    // Agregamos el Valor para el Impuesto IVA
                    var imp = Request.GetCatalogsService().GetVatRates().FirstOrDefault(model => model.SriCode == detailModel.ValueAddedTaxCode);
                    if (imp == null)
                    {
                        imp = new VatRate { Id = 0, SriCode = detailModel.ValueAddedTaxCode, RateValue = 0M, Name = "0%" };
                    }
                    else
                    {
                        if (imp.RateValue > 0 && detailModel.ValueAddedTaxValue == 0)
                        {
                            detailModel.ValueAddedTaxValue = decimal.Round(detailModel.SubTotal * (imp.RateValue / 100), 2);
                        }
                    }

                    detailModel.Taxes.Add(new TaxModel
                    {
                        Code = "2",
                        PercentageCode = imp.SriCode,
                        Rate = imp.RateValue,
                        TaxableBase = detailModel.SubTotal,
                        TaxValue = detailModel.ValueAddedTaxValue
                    });

                    //Agregamos el ICE si esta especificado por lo menos el codigo:
                    if (!string.IsNullOrEmpty(detailModel.SpecialConsumTaxCode))
                    {
                        var ice = Request.GetCatalogsService()
                                         .GetIceRates()
                                         .FirstOrDefault(model => model.SriCode == detailModel.SpecialConsumTaxCode);

                        // Si no existe en nuestra tabla:
                        if (ice == null)
                        {
                            ice = new IceRate { SriCode = detailModel.SpecialConsumTaxCode, Rate = 0M, Name = "Desconocido" };
                        }

                        // Si existe el impuesto ICE Especificado
                        if ((ice.Rate ?? 0) > 0)
                        {
                            var impRate = ice.Rate.Value;
                            var impBase = (detailModel.SpecialConsumTaxValue * 100) / ice.Rate.Value;

                            detailModel.Taxes.Add(new TaxModel
                            {
                                Code = "3",
                                PercentageCode = ice.SriCode,
                                Rate = impRate,
                                TaxableBase = impBase,
                                TaxValue = detailModel.SpecialConsumTaxValue
                            });
                        }
                    }
                }

            }


            errormsg = "OK";
            return true;

        }
        #endregion Procesar suscripción
    }
}
