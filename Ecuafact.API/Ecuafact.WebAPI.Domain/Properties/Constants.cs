using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Ecuafact.WebAPI 
{
    public static class Constants
    {
        public const string MultipleEmailRegex = @"^((([a-zA-Z\-0-9\._]+@)([a-zA-Z\-0-9\.]+)[;, ]*)+)$";
        public const string MultiplePhoneRegex = @"^\d*(?:(,?;? ?\/?\|?-?)\d?)*$";

        public const string MSG_INVALIDCREDENTIALS = "Credenciales Inválidas";
        public const string MSG_INVALIDUSERPASS = "Usuario o Contraseña no válidos!";

        public static NameValueCollection AppSettings { get; set; } = WebConfigurationManager.AppSettings;

        public static string ServerPath { get; private set; } = HttpContext.Current.Server.MapPath("/");
         
        public static string AppToken => AppSettings["Ecuafact:AppToken"] ?? "4522";
        
        public static string SentryToken => AppSettings["Ecuafact:SentryToken"] ?? "https://ae76e7b8d9c542bb87ba878cdfa2af03@o297829.ingest.sentry.io/5210295";

        public static bool EnableLog => ToBool(AppSettings["Ecuafact:EnableLog"], true);
         
        public static string WebApiUrl = AppSettings["Ecuafact:WebApiUrl"] ?? "https://api.ecuafact.com/v3";

        public static string NexusApiUrl => AppSettings["Ecuafact:NexusUrl"] ?? "https://localhost:44347";
        
        public static string NexusToken => AppSettings["Ecuafact:NexusToken"] ?? "33A08B05-0E2C-47F6-B0A1-D0F50F7A7850";

        public static string ServiceUrl => AppSettings["Ecuafact:ServiceUrl"] ?? "http://api.ecuafact.com/appdev/v1/Service.svc";

        public static string ServiceIssueUrl => AppSettings["Ecuafact:ServiceIssueUrl"] ?? "http://api.ecuafact.com/appdev/apiEmi/ApiEmisionService.svc";         
         
        public static string LogLocation => AppSettings["Ecuafact:LogLocation"] ?? "F:\\EcuafactApp\\logs\\";

        public static string ResourcesLocation => AppSettings["Ecuafact:ResourcesLocation"] ?? "F:\\EcuafactApp\\Resources\\";

        public static string EngineLocation => AppSettings["Ecuafact:EngineLocation"] ?? "F:\\EcuafactApp\\Resources\\";
        public static string EngineLocation2 => AppSettings["Ecuafact:EngineLocation2"] ?? "F:\\EcuafactApp\\Resources2\\";

        public static string XMLFilesLocation => AppSettings["Ecuafact:XMLFilesLocation"] ?? "G:\\ExpressGeneral\\XML_DECLARACIONES";
        public static string PDFSaleNoteFilesLocation => AppSettings["Ecuafact:PDFSaleNoteFilesLocation"] ?? "X:\\XML_DECLARACIONES_APIDEV\\RECIBIDOS_NV";

        public static string WebApp = AppSettings["Ecuafact:WebApp"] ?? "https://localhost:44311/";

        public static string PlanBasic = AppSettings["Ecuafact:PlanBasic"] ?? "L01";
        public static string PlanPro = AppSettings["Ecuafact:PlanPro"] ?? "L03";
        public static string AmountDocIlimitado = AppSettings["Ecuafact:AmountDocIlimitado"] ?? "0000000000";
        public static string emailNotification = AppSettings["email:notification"] ?? "administracion@ecuanexus.com";
        public static string alignet = AppSettings["COMMERCE:ALIGNET"] ?? "EC-00002";
        public static string paymentez = AppSettings["COMMERCE:Paymentez"] ?? "EC-00001";
        public static bool viewsSalesReport => ToBool(AppSettings["Ecuafact:viewsSalesReport"], true);
        public static string CodFactorChequeoPonderadoerico => AppSettings["Ecuafact:CodFactorChequeoPonderadoerico"] ?? "98469511";

        public static string Apikey => AppSettings["Ecuafact:Apikey"] ?? "LhiIeVaxlR5cf965164d23b0Fot/KjImPm3lcX0t30OZ";
        public static int Uid => ToInt32(AppSettings["Ecuafact:Uid"], 4522);
        public static string xmlATS = Path.Combine(ServerPath, "ATSXml", "ats.xsd");



        /// <summary>
        /// Token de Acceso Administrativo
        /// </summary>
        public static string ServiceToken => AppSettings[$"Ecuafact:ServiceToken"] ?? "cb365702-e1c4-4980-b598-dc0e49366717";

        public static bool DevelopmentMode => (__developmentMode == "1" || __developmentMode == "true");

        private static string __developmentMode => AppSettings[$"Ecuafact:DevelopmentMode"]?.Trim()?.ToLower() ?? "false";


        public struct Emailing
        {
            public static string SenderName => AppSettings[$"Ecuafact:Email:SenderName"] ?? "Ecuafact App"; 
            public static string SenderAddress => AppSettings[$"Ecuafact:Email:SenderAddress"] ?? "noreply@ecuafact.com";
            public static string SmtpServer => AppSettings[$"Ecuafact:Email:SmtpServer"] ?? "email-smtp.us-east-1.amazonaws.com";
            public static string Username => AppSettings[$"Ecuafact:Email:Username"] ?? "AKIA2C7SD5Q23XWEHP4C";
            public static string Password => AppSettings[$"Ecuafact:Email:Password"] ?? "BB/XA5foexBEofn6vJq94//0FotPm3lcX0t30OZ/KjIm";
            public static int Port => ToInt32(AppSettings["Ecuafact:Email:Port"], 587);
            public static bool EnableSSL => ToBool(AppSettings["Ecuafact:Email:EnableSSL"], true);
            public static bool DefaultCredentials => ToBool(AppSettings["Ecuafact:Email:DefaultCredentials"], false);
            public static string AdminMail => AppSettings[$"Ecuafact:Email:AdminAddress"] ?? "rramirez@ecuanexus.com";
        }


        public struct ElectronicSign
        {
    #if DEBUG
            public static string ProcessorURL => AppSettings["ElectronicSign:ProcessorURL"] ?? "https://api.uanataca.ec/v4/solicitud";
            public static string ProcessStatusUrl => AppSettings["ElectronicSign:ProcessStatusUrl"] ?? "https://api.uanataca.ec/v4/consultarEstado";
            public static string ApiKey => AppSettings["ElectronicSign:APIKEY"] ?? "hSD73nV6jhk8dg2JHgcjl@fqxj8WjeOFCPAjhekL7sn3";
#else
            public static string ProcessorURL => AppSettings["ElectronicSign:ProcessorURL"] ?? "https://api.uanataca.ec/v4/solicitud";
            public static string ProcessStatusUrl => AppSettings["ElectronicSign:ProcessStatusUrl"] ?? "https://api.uanataca.ec/v4/consultarEstado";
            public static string ApiKey => AppSettings["ElectronicSign:APIKEY"] ?? "ERqjasdfkjlKJLH89234thJHVhgvsdfl[7hG@hgfdjklDFPs2";
#endif
            public static string UID => AppSettings["ElectronicSign:UID"] ?? "9";
            public static int SupplierElectronicSign => ToInt32(AppSettings["ElectronicSign:Proveedor"], 2);
            public static decimal Price => ToDecimal(AppSettings["ElectronicSign:Price"], 19.90M);                

        }

        public struct Subscription
        {
            public static decimal Price => ToDecimal(AppSettings["Subscription:Price"], 29.90M);
        }

        public struct EncryptionKeys
        { 
            public static string PasswordHash => AppSettings["EncryptionKeys:PasswordHash"]?? "3cu4n3xu5@3cu4d0r";

            public static string SaltKey => AppSettings["EncryptionKeys:SaltKey"] ?? "3CU4;F4C7@1854665448721";

            public static string VIKey => AppSettings["EncryptionKeys:VIKey"] ?? "@5X,3c2/D7e9a*3g2Z5";
        }


        public struct VPOS2
        {
            public const string CURRENCY_CODE = "840";
            public const string LANGUAGE_CODE = "SP";
            public const string PROGRAM_LANGUAGE = "C# .NET";

            public static string GetStringSHA(string text)
            {
                SHA512 sha2 = SHA512Managed.Create();
                ASCIIEncoding encoding = new ASCIIEncoding();
                StringBuilder sb = new StringBuilder();

                byte[] stream = sha2.ComputeHash(encoding.GetBytes(text));

                for (int i = 0; i < stream.Length; i++)
                {
                    sb.AppendFormat("{0:x2}", stream[i]);
                }

                return sb.ToString();
            }

            static string GetSecretKey(string configFileKey)
            {
                var keyValue = AppSettings[$"VPOS2:{configFileKey}:FILE"] as string;

                if (string.IsNullOrEmpty(keyValue))
                {
                    throw new ConfigurationErrorsException($"Error de Configuracion: No se ha registrado el codigo {configFileKey}!");
                }

                var keyFile = Path.Combine(ServerPath, "VPOS2", $"{keyValue}.txt");

                if (!File.Exists(keyFile))
                {
                    throw new ConfigurationErrorsException($"Error de Configuracion: No existe el archivo {configFileKey}!");
                }

                return File.ReadAllText(keyFile);
            }

            public static string WalletSecretKey => GetSecretKey("WALLET"); // AL BUSCARLA GENERA EL RESTO Y OBTIENE SU CONTENIDO: NOMBRE_SHA2KEY

            public static string PasarelaSecretKey => GetSecretKey("PASARELA"); // AL BUSCARLA GENERA EL RESTO Y OBTIENE SU CONTENIDO: NOMBRE_SHA2KEY
 
            public static string WalletServiceURL => AppSettings[$"VPOS2:Wallet:URL"];

            public static string IDWalletCode => AppSettings[$"VPOS2:IDWalletCode"];

            public static string IDCommerceCode => AppSettings[$"VPOS2:IDCommerceCode"];

            public static string IDCommerceMall => AppSettings[$"VPOS2:IDCommerceMall"];

            public static string IDAcquirer => AppSettings[$"VPOS2:IDAcquirer"] ;

            public static string TerminalNumber => AppSettings[$"VPOS2:TerminalNumber"] ;

            public static string UrlAPI => AppSettings[$"VPOS2:API:URL"];


        }


        public static void SetAppSettings(NameValueCollection appSettings)
        {
            AppSettings = appSettings;
        }

        public static void SetPath(string path)
        {
            ServerPath = path;
        }

        private static int ToInt32(string value, int defaultValue)
        { 
            if (!int.TryParse(value, out int result))
            {
                return defaultValue;
            }

            return result;
        }

        private static decimal ToDecimal(string value, decimal defaultValue)
        {

            if (!decimal.TryParse(value, out decimal result))
            {
                return defaultValue;
            }

            return result;
        }

        private static bool ToBool(string value, bool defaultValue)
        {
            //if (string.IsNullOrEmpty(value) && (value.ToLower() == "true" || value == "1"))
            //{
            //    return true;
            //}

            if (!string.IsNullOrEmpty(value))
            {
                return (value.ToLower() == "true" || value == "1");
            }

            return defaultValue;
        }

        public static string ComputeSHA256Hash(string text)
        {
            //using (var sha256 = new SHA256Managed())
            //{
            //    return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "");
            //}

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

        public static string DecodeBase64(this string value)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
            string auth_token = Convert.ToBase64String(toEncodeAsBytes);
            return auth_token;
        }


    }
}
