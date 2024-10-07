using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Ecuafact.Web
{
    public static class Constants
    {
        public static NameValueCollection AppSettings { get; set; } = WebConfigurationManager.AppSettings ?? ConfigurationManager.AppSettings;

        public const string MultipleEmailRegex = @"^((([a-zA-Z\-0-9\._]+@)([a-zA-Z\-0-9\.]+)[;, ]*)+)$";

        public const string MultiplePhoneRegex = @"^\d*(?:(,?;? ?\/?\|?-?)\d?)*$";

        public const string LastName = @"[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]{3,48}";// @"([a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]{4,32}){1}([ ]{1})([a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]{4,32}){1}";

        public const string FirstName = @"[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]{3,48}";

        public static bool Development => GetConfigValue("Development", "false") == "true" || GetConfigValue("Development", "0") == "1";

        public static bool Ecuanexus => GetConfigValue("Ecuanexus", "false") == "true" || GetConfigValue("Ecuanexus", "0") == "1";

        
        public static string WebApiUrl => GetConfigValue("Ecuafact:WebApiUrl", "https://api.ecuafact.com/dev/v3");

        public static string SentryToken => GetConfigValue("Ecuafact:SentryToken", "https://ae76e7b8d9c542bb87ba878cdfa2af03@o297829.ingest.sentry.io/5210295"); 

        public static string ServiceUrl => GetConfigValue("Ecuafact:ServiceUrl", "https://api.ecuafact.com/appdev/v1");

        public static string EngineUrl => GetConfigValue("Ecuafact:EngineUrl", "https://nube.ecuafact.com/expressdev/admin");

        public static string ServiceToken => GetConfigValue("Ecuafact:ServiceToken", "cb365702-e1c4-4980-b598-dc0e49366717");

        public static string EcuanexusToken => GetConfigValue("Ecuafact:Ecuanexus", "0e4e2032-a11d-4bde-b43c-9bafc5dcd2f5-0992882549001");

        public static string VPOSDirectory => GetConfigValue("VPOS:Directory", "C:\\Tmp");

        public static string ServiceEngineUrl => $"{EngineUrl}/service.svc";

        public static string ServiceAppUrl => $"{ServiceUrl}/service.svc";

        public static string SriUrl => GetConfigValue("Ecuafact:SriUrl", "https://srienlinea.sri.gob.ec/sri-en-linea/inicio/NAT");

        public static string RewardsUrl => GetConfigValue("Ecuafact:RewardsUrl", "https://rewards.ecuafact.com?token=");

        public static string SoporteUrl => GetConfigValue("Ecuafact:SoporteUrl", "https://soporteecuafact.freshdesk.com");

        public static void SetAppSettings(NameValueCollection appSettings) => AppSettings = appSettings; 

        public static string GetConfigValue(string key, string defaultValue) => AppSettings[key] ?? defaultValue;
        public static int Older => ToInt32(AppSettings["Ecuanexus:Older"], 18);

        public static int SupplierElectronicSign => ToInt32(AppSettings["Ecuanexus:Proveedor"], 1);

        public static bool TypePayment => AppSettings["Ecuanexus:TypePayment"] == "true" || AppSettings["Ecuanexus:TypePayment"] == "1";

        public static bool modalComercio => AppSettings["Ecuanexus:ModalComercio"] == "true" || AppSettings["Ecuanexus:ModalComercio"] == "1";

        public static string urlComercio => GetConfigValue("Ecuanexus:UrlComercio", "https://vpayment.verifika.com/VPOS2/js/modalcomercio.js");

        public static string PlanBasic = AppSettings["Ecuafact:PlanBasic"] ?? "L01";

        public static string PlanPro = AppSettings["Ecuafact:PlanPro"] ?? "L03";

        public const string limitePlanMsj = "Sin emisión, a llegado al limite del plan.";

        public static string alignet = AppSettings["COMMERCE:ALIGNET"] ?? "EC-00002";

        private static int ToInt32(string value, int defaultValue)
        {
            if (!int.TryParse(value, out int result))
            {
                return defaultValue;
            }

            return result;
        }


    }
}
