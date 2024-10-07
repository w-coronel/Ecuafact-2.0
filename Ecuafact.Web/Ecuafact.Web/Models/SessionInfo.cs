using Ecuafact.Web;
using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.Domain.Entities.API;
using Ecuafact.Web.MiddleCore.ApplicationServices;
using Ecuafact.Web.MiddleCore.NexusApiServices;
using Ecuafact.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
 
public static class SessionInfo
{
    public static string RECIBIDOS_SESSION => $"DOC_RECIBIDOS_{SessionInfo.ApplicationToken ?? AppID}";

    public static string SHOPPINGCART_SESSION => $"SHOPPINGCART_{ApplicationToken ?? AppID}";

    public static string CONTRIBUYENTES_SESSION => $"CONTRIBUYENTES_{SessionInfo.ApplicationToken ?? AppID}";

    public static string BORRADORES_SESSION => $"DOC_BORRADORES_{SessionInfo.ApplicationToken ?? AppID}";

    public static string EMITIDOS_SESSION => $"DOC_EMITIDOS_{SessionInfo.ApplicationToken ?? AppID}";

    public static string FACTURAS_SESSION => $"FAC_{SessionInfo.ApplicationToken ?? AppID}";

    public static string NOTASCREDITO_SESSION => $"NCR_{SessionInfo.ApplicationToken ?? AppID}";

    public static string NOTASDEBITO_SESSION => $"NDB_{SessionInfo.ApplicationToken ?? AppID}";

    public static string LIQUIDACIONES_SESSION => $"LIQ_{SessionInfo.ApplicationToken ?? AppID}";

    public static string RETENCION_SESSION => $"RET_{SessionInfo.ApplicationToken ?? AppID}";

    public static string GUIAREMISION_SESSION => $"GRM_{SessionInfo.ApplicationToken ?? AppID}";
    private static string NOTIFICATION_SESSION => $"NOTIFICATION_{SessionInfo.ApplicationToken ?? AppID}";

    public static string NOTAVENTA_SESSION => $"NDV_{SessionInfo.ApplicationToken ?? AppID}";

    private static Guid __AppID;

    internal static string AppID
    {
        get
        {
            if (__AppID == null)
            {
                __AppID = Guid.NewGuid();
            }
         
            return $"{__AppID}-{SessionInfo.Issuer?.RUC}";
        }
    }

    public static bool ShowNotification
    {
        get
        {
            var notify = true;

            if (Session[NOTIFICATION_SESSION] is bool)
            {
                notify = Convert.ToBoolean(Session[NOTIFICATION_SESSION]);
            }
            else
            {
                Session[NOTIFICATION_SESSION] = notify;
            }

            return notify;
        }
        set { Session[NOTIFICATION_SESSION] = value; }
    }

    internal static PurchaseOrderModel ShoppingCart
    {
        get { return Session[SessionInfo.SHOPPINGCART_SESSION] as PurchaseOrderModel; }
        set { Session[SessionInfo.SHOPPINGCART_SESSION] = value; }
    }

    public static DeductibleLimitResponse Presupuesto
    {
        get { return Session["Presupuesto" + ApplicationToken] as DeductibleLimitResponse; }
        set { Session["Presupuesto" + ApplicationToken] = value; }
    }
        
    private static HttpContext HttpContext
    {
        get
        { 
            return HttpContext.Current;
        }
    }

    internal static HttpSessionState Session
    {
        get { return HttpContext?.Session; }
    }

    public static CatalogInfo Catalog { get; } = new CatalogInfo();

    public static Pagos PagoPendiente { get; } = new Pagos();

    /// <summary>
    /// Return URL Address
    /// </summary>
    public static string ReturnUrl
    {
        get
        {
            return Session["ReturnUrl"] as string;
        }
        set
        {
            Session["ReturnUrl"] = value;
        }
    }

    /// <summary>
    /// Return URL Rewards
    /// </summary>
    public static string RewardsUrl
    {
        get
        {
            return Session["RewardsUrl"] as string;
        }
        set
        {
            Session["RewardsUrl"] = value;
        }
    }

    /// <summary>
    /// Return URL Rewards
    /// </summary>
    public static StatisticsAts StatisticsAts
    {
        get
        {
            return Session["StatisticsAts"] as StatisticsAts;
        }
        set
        {
            Session["StatisticsAts"] = value;
        }
    }

    /// <summary>
    /// Return URL soporte
    /// </summary>
    public static string SoporteUrl
    {
        get
        {
            return Session["SoporteUrl"] as string;
        }
        set
        {
            Session["SoporteUrl"] = value;
        }
    }

    /// <summary>
    /// Redireccioaar aulacion de documentos
    /// </summary>
    public static string AnulacionForm
    {
        get
        {
            return Session["AnulacionForm"] as string;
        }
        set
        {
            Session["AnulacionForm"] = value;
        }
    }

    /// <summary>
    /// Visualizar una vez la modal de notification
    /// </summary>
    public static string NotificationView
    {
        get
        {
            return Session["Notification_View"] as string;
        }
        set
        {
            Session["Notification_View"] = value;
        }
    }

    public static int CurrentYear
    {
        get { return DateTime.Now.Year; }
    }

    public static IEnumerable<DeductibleType> DeductibleTypes
    {
        get
        {
            // Este registro se vuelve a cargar cada hora dependiendo del issuer:
            var sessionName = $"DeductibleTypes";

            var deductibleTypes = Session[sessionName] as IEnumerable<DeductibleType>;

            // Cargamos los deducibles para esta sesión
            if (deductibleTypes == null || !deductibleTypes.Any())
            {
                deductibleTypes = ServicioComprobantes.ObtenerTiposDeducibles(ApplicationToken);

                Session[sessionName] = deductibleTypes;
            }

            return deductibleTypes ?? new List<DeductibleType>();
        }
    }

    public static IEnumerable<Document> DocumentsReceived
    {
        get
        {
            // Este registro se vuelve a cargar cada hora dependiendo del issuer:
            var timestamp = DateTime.Now.ToString("yyyyMMddhhm", Culture.English);
            var sessionName = $"R{ApplicationToken}_{timestamp}";

            var recibidos = Session[sessionName] as IEnumerable<Document>;
            // Cargamos los recibidos para esta sesión
            if (recibidos == null)
            {
                recibidos = ServicioComprobantes.ObtenerComprobantesRecibidos(ApplicationToken, new ConsultaDocumentosParamsDto
                {
                    FechaInicio = new DateTime(DateTime.Now.Year, 1, 1),
                    FechaHasta = DateTime.Now.AddHours(23).AddMinutes(59),
                    Contenido = "",
                    TipoDocumento = "0",
                    TipoFecha = "1"
                })?.documents?.ToList() ?? new List<Document>();

                Session[sessionName] = recibidos;
            }

            return recibidos;
        }
    }
     
    public static void LogException(Exception ex)
    {
        Notifications.Add(ex.ToString(), SessionInfo.AlertType.Error);
    }

    public static void LogError(string msg)
    {
        Notifications.Add(msg, SessionInfo.AlertType.Error);
    }

    public static DashboardInfo DashboardInfo
    {
        get
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddhhmm", Culture.English);
            var dashboard = Session[$"DASHBOARD_{timestamp}"] as DashboardInfo;

            // Cargamos los recibidos para esta sesión
            if (dashboard == null)
            {
                dashboard = ServicioDashboard.ObtenerEstadisticas(ApplicationToken);

                if (dashboard != null) Session[$"RECIBIDOS_{timestamp}"] = dashboard;
            }

            return dashboard ?? new DashboardInfo(); ;
        }
    }

    /// <summary>
    /// Emisor Actual
    /// </summary>
    public static IssuerDto Issuer
    {
        get
        {
            return UserSession?.Issuer;
        }
    }

    /// <summary>
    /// Informacion de Inicio de sesion
    /// </summary>
    public static LoginResponseModel LoginInfo
    {
        get
        {
            LoginResponseModel login = null;
            if ((Session["LoginInfo"] is LoginResponseModel) || Ecuafact.Web.Controllers.AuthController.EnsureUserIsLoggedIn())
            {
                login = Session["LoginInfo"] as LoginResponseModel;
            }
            return login;
        }
        set
        {
            Session["LoginInfo"] = value;
        }
    }

    public static ClientModel UserInfo
    {
        get { return LoginInfo?.UserInfo; }
    }

    public static bool IsLoggedIn
    {
        get
        {
            return LoginInfo != null && UserSession != null;
        }
    }

    public static SecuritySessionRole UserRole
    {
        get
        {
            if (LoginInfo != null && UserSession == null)
            {
                return SecuritySessionRole.User;
            }

            if (LoginInfo != null && UserSession != null)
            {
                if (LoginInfo.CurrentIssuer?.UserRole == "4")
                {
                    return SecuritySessionRole.Cooperative;
                }
                return SecuritySessionRole.Issuer;
            }

            if (LoginInfo.UserInfo.Id == UserSession.IssuerRUC)
            {
                return SecuritySessionRole.Admin;
            }

            return SecuritySessionRole.None;
        }
    }


    /// <summary>
    /// Informacion de Inicio de sesion
    /// </summary>
    public static UserSession UserSession
    {
        get
        {
            return Session["UserSession"] as UserSession;
        }
        set
        {
            Session["UserSession"] = value;
        }
    }       

    /// <summary>
    /// Token de seguridad del usuario
    /// </summary>
    public static string SecurityToken
    {
        get
        {
            return UserInfo?.ClientToken;
        }
    }

    /// <summary>
    /// Token de Sesión de la Aplicacion Web API
    /// </summary>
    public static string ApplicationToken
    {
        get
        {
            return UserSession?.Token ?? UserInfo?.ClientToken;
        }
    }

    /// <summary>
    /// Lista de Emisores autorizados para este usuario
    /// </summary>
    public static List<LoginIssuerModel> IssuerList
    {
        get
        {
            return LoginInfo?.Issuers ?? new List<LoginIssuerModel>();
        }
    }

    private static T GetCookie<T>(string name)
    {
        try
        {
            var cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                var json = cookie.Value?.Decode();

                return JsonConvert.DeserializeObject<T>(json);
            }
        }
        catch (Exception ex)
        {
            ex.ToString();
        }

        return default(T);
    }

    private static HttpCookie SetCookie<T>(string name, T value)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        var cookie = new HttpCookie(name, jsonValue.Encode())
        {
            Expires = DateTime.Now.AddDays(30)
        };

        HttpContext.Current.Response.Cookies.Add(cookie);
        return cookie;
    }


    /// <summary>
    /// Informacion de Inicio de sesion
    /// </summary>
    public static Pagos PendingPayments
    {
        get
        {
            return Session["PendingPayments"] as Pagos;
        }
        set
        {
            Session["PendingPayments"] = value;
        }
    }


    public static class Culture
    {
        public static CultureInfo English { get; } = CultureInfo.GetCultureInfo("en");
        public static CultureInfo Spanish { get; } = CultureInfo.GetCultureInfo("es");
    }


    public static class Alert
    {
        private static AlertInfo lastAlert
        {
            get
            {
                return Session["LAST_ALERT"] as AlertInfo;
            }
            set
            {
                if (value == null)
                {
                    if (Session["LAST_ALERT"] != null)
                        Session.Remove("LAST_ALERT");
                }
                else
                {
                    Session["LAST_ALERT"] = value;
                }
            }
        }

        public static void SetAlert(string message, AlertType type)
        {
            SetAlert(message, string.Empty, type);
        }

        public static void SetAlert(string message, string url = "", AlertType type = AlertType.Info )
        {
            if (string.IsNullOrEmpty(message))
            {
                lastAlert = null;
            }
            else
            {
                lastAlert = new AlertInfo { Type = type, Message = message, Url = url  };
                Notifications.All.Add(lastAlert);
            }
        }

        public static AlertInfo GetAlert()
        {
            var alert = lastAlert;
            SetAlert(null);
            return alert;
        }
    }

    public static class Notifications
    {
        public static IEnumerable<AlertInfo> Unread
        {
            get
            {
                return All.Where(m => m.Unread);
            }
        }

        public static List<AlertInfo> All
        {
            get
            {
                var notificationList = Session["NOTIFICATION_LIST"] as List<AlertInfo>;
                if (notificationList == null)
                {
                    notificationList = GetCookie<List<AlertInfo>>(".express_alerts") ?? new List<AlertInfo>();
                    Session["NOTIFICATION_LIST"] = notificationList;
                }
              
                return notificationList;
            }
        }
        public static void Add(string message, AlertType type = AlertType.Info, string icon = "fa fa-bell-o")
        {
            Add(message, "", type, icon);
        }

        public static void Add(string message, string url = "", AlertType type = AlertType.Info, string icon = "fa fa-bell-o")
        {
            All.Add(new AlertInfo { Type = type, Message = message, Url = url, Icon = icon });
            SetCookie(".express_alerts", All.Where(m => m.CreatedOn > DateTime.Now.AddDays(-30)));
        }
          
        public static AlertInfo Read()
        {
            var alert = All.FirstOrDefault(m => m.Unread);

            if (alert != null)
            {
                alert.Unread = false;
                return alert;
            }

            return null;
        }
    }


    public class AlertIcons
    {
        public static string Home => "fa fa-home";
    }

    public class AlertInfo
    {
        public string BoxType
        {
            get { return Type.ToString().ToLower();  }
        }
        public string Message { get; set; } = string.Empty;
        public AlertType Type { get; set; } = AlertType.Info;
        public bool Unread { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string Url { get; set; }
        public string Icon { get; set; } = "fa fa-bell-o";
        public TimeSpan Time
        {
            get
            {
                return (DateTime.Now - CreatedOn);
            }
        }
    }

    public enum AlertType
    {
        Info,
        Success,
        Warning,
        Error
    }

    public class Pagos
    {
        public  bool Pendiente { get; set; } = false;
        public string UrlPago { get; set; }
        public ElectronicSignModel ElectronicSign { get; set; } = new ElectronicSignModel();
        public PurchaseSubscription PurchaseSubscription { get; set; } = new PurchaseSubscription();
    }    
}
