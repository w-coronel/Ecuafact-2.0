using Ecuafact.Web.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace System
{
    public static class RequestExtensions
    {

        public static string ToDecimalString(this decimal value)
        {
            var quantity = value.ToString() ?? "0";

            if (quantity.Contains(".") || quantity.Contains(","))
            {
                if (quantity.EndsWith(".000000") || quantity.EndsWith(",000000"))
                {
                    quantity = quantity.Substring(0, quantity.Length - 7);
                }
                else if (quantity.EndsWith("0000"))
                {
                    quantity = quantity.Substring(0, quantity.Length - 4);
                }
                else if (quantity.EndsWith(",00") || quantity.EndsWith(".00"))
                {
                    quantity = quantity.Substring(0, quantity.Length - 3);
                }
            }
            if (!quantity.Contains("."))
            {
                quantity = $"{quantity}.00";
            }
            return quantity;
        }

        static Dictionary<string, string> styles = new Dictionary<string, string>();

        static RequestExtensions()
        {
            styles.Add("01", "brand");
            styles.Add("04", "warning");
            styles.Add("05", "info");
            styles.Add("06", "dark");
            styles.Add("07", "success");
            styles.Add("02", "primary");
            styles.Add("03", "danger");
            styles.Add("08", "secondary");
        }


        public static IEnumerable<StatusListItem> GetStatusList(this HttpRequestBase Request)
        {
            var values = Enum.GetValues(typeof(DocumentStatusEnum));
            var items = new List<StatusListItem>
            {
                new StatusListItem {  id = null, text = "Todos", icon = "home" }
            };

            for (int i = 0; i < values.Length; i++)
            {
                var value = values.GetValue(i);

                DocumentStatusEnum? status = (DocumentStatusEnum)value;
                var text = status.GetDisplayValue();
                var icon = Request.GetStatusIcon(status);
                var style = status.GetStyle();

                items.Add(new StatusListItem { id = Convert.ToString(Convert.ToInt32(status)), text = text, icon = icon, status = status, style = style });
            }

            return items;
        }

        public static string GetColor(this HttpRequestBase Request, string id)
        {
            if (styles.ContainsKey(id))
            {
                return styles[id];
            }

            return styles?.FirstOrDefault().Value;
        }

        public static string GetColorDeductibles(this HttpRequestBase request, object id)
        {
            var colors = new string[]
            {
                "#9e9e9e", // 0 Sin Clasificar 
                "#009788", // 1 Vivienda
                "#9a29ad", // 2 Educación
                "#fec107", // 3 Alimentación
                "#f34638", // 4 Vestimenta
                "#2097f5", // 5 Salud
                "#00c1d4", // 6 Arte y Cultura
                "#CD6539", // 7 Turismo
                "#00bdd3", // 8 Relacionado con la actividad
                "#0ed145", // 9 Empty
                "#ff7f27", // 10 Empty
                "#448afa" // 11 Total
            };

            var idValue = 0;
            int.TryParse(Convert.ToString(id), NumberStyles.Integer, CultureInfo.GetCultureInfo("en-US"), out idValue);

            var color = colors[0];
            if (idValue < colors.Length)
            {
                color = colors[idValue];
            }

            return color;
        }

        public static string GetController(this HttpRequestBase Request, string code)
        {
            if (code == "01")
            {
                return "Factura";
            }
            else if (code == "03")
            {
                return "Liquidacion";
            }
            else if (code == "04")
            {
                return "NotaCredito";
            }
            else if (code == "05")
            {
                return "NotaDebito";
            }
            else if(code == "06")
            {
                return "GuiaRemision";
            }
            else if(code == "07")
            {
                return "Retencion";
            }

            return "Comprobantes";
        }

        public static string GetDocumentType(this HttpRequestBase Request, string code)
        {
            var type = SessionInfo.Catalog.DocumentTypes.FirstOrDefault(m => m.SriCode == code)?.Name;

            return type ?? "Documento";
        }

        public static string GetStatusIcon(this HttpRequestBase Request, DocumentStatusEnum? status)
        {
            var iconClass = "file";
            if (status.HasValue)
            {
                switch (status.Value)
                {
                    case DocumentStatusEnum.Issued:
                        iconClass = "hourglass-half";
                        break;
                    case DocumentStatusEnum.Error:
                        iconClass = "times";
                        break;
                    case DocumentStatusEnum.Authorized:
                        iconClass = "check";
                        break;
                    case DocumentStatusEnum.Revoked:
                        iconClass = "user-times";
                        break;
                    case DocumentStatusEnum.Deleted:
                        iconClass = "trash";
                        break;
                    case DocumentStatusEnum.Draft:
                    default:
                        break;
                }
            }

            return iconClass;

        }
        /*
         *  .AddItem("Facturas", Url.Action("Index", "Factura"), "icon-doc")
        .AddItem("Notas de Credito", Url.Action("Index", "NotaCredito"), "icon-docs")
        .AddItem("Retenciones", Url.Action("Index", "Retencion"), "fa fa-file-code-o")
        .AddItem("Guias de Remision", Url.Action("Index", "GuiaRemision"), "fa fa-truck");
        */

        public static string GetDocumentIcon(this HttpRequestBase Request, string type)
        {
            var iconClass = "flaticon2-checking";
            if (!string.IsNullOrEmpty(type))
            {
                switch (type)
                {
                    case "01": // Factura
                        iconClass = "flaticon2-writing";
                        break;
                    case "04": // Nota de Credito
                        iconClass = "flaticon2-layers-2";
                        break;
                    case "05": // Nota de Debito
                        iconClass = "flaticon2-list-2";
                        break;
                    case "07": // Retencion
                        iconClass = "flaticon2-ui";
                        break;
                    case "06": // Guia de Remision
                        iconClass = "flaticon2-lorry";
                        break;
                }
            }

            return iconClass;
        }

        public static string GetStatusType(this HttpRequestBase Request, DocumentStatusEnum? status)
        {
            var iconClass = "info";
            if (status.HasValue)
            {
                switch (status.Value)
                {
                    case DocumentStatusEnum.Draft:
                        iconClass = "info";
                        break;
                    case DocumentStatusEnum.Issued:
                        iconClass = "warning";
                        break;
                    case DocumentStatusEnum.Authorized:
                    case DocumentStatusEnum.Validated:
                        iconClass = "success";
                        break;
                    case DocumentStatusEnum.Revoked:
                    case DocumentStatusEnum.Deleted:
                    case DocumentStatusEnum.Error:
                        iconClass = "error";
                        break;
                }
            }

            return iconClass;
        }


        public static string GetLogoUrl(this HttpServerUtilityBase Server, string logoFile, string rucDefault = null)
        {
            return GetLogoFile(Server, logoFile, rucDefault, true);
        }

        public static string GetLogoFile(this HttpServerUtilityBase Server, string logoFile, string rucDefault = null, bool timestamp = false)
        {
            var logo = logoFile;

            if (!string.IsNullOrEmpty(logoFile))
            {
                if (!System.IO.File.Exists(logo))
                {
                    logo = $"~/{logoFile}";

                    if (!System.IO.File.Exists(Server.MapPath(logo)))
                    {
                        logo = $"~/Logos/{logoFile}";

                        if (!System.IO.File.Exists(Server.MapPath(logo)))
                        {
                            logo = $"~/Logos/{logoFile}.jpg";

                            if (!System.IO.File.Exists(Server.MapPath(logo)))
                            {
                                logo = $"~/Logos/{logoFile}_logo.jpg";

                                if (!System.IO.File.Exists(Server.MapPath(logo)))
                                {
                                    logo = $"~/Logos/no_logo.jpg";
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                logo = $"~/Logos/{rucDefault}_logo.jpg";
                if (!System.IO.File.Exists(Server.MapPath(logo)))
                {
                    logo = $"~/{rucDefault}_logo.jpg";
                    if (!System.IO.File.Exists(Server.MapPath(logo)))
                    {
                        logo = $"~/Logos/no_logo.jpg";
                    }
                }
            }

            if (timestamp)
            {
                logo = $"{logo}?{DateTime.Now.ToFileTime()}";
            }

            return logo;
        }

        public static bool ExistsLogoFile(this HttpServerUtilityBase Server, string logoFile)
        {
            var logo = logoFile;
            if (!string.IsNullOrEmpty(logo))
            {
                logo = $"~/Logos/{logoFile}";
                if (System.IO.File.Exists(Server.MapPath(logo))){
                    return true;
                }
                logo = $"~/Logos/{logoFile}.jpg";
                if (System.IO.File.Exists(Server.MapPath(logo))){
                    return true;
                }
                logo = $"~/Logos/{logoFile}_logo.jpg";
                if (System.IO.File.Exists(Server.MapPath(logo))) {
                    return true;
                }               
            }
            return false;
        }
    }
    public static class ResponseExtensions
    {
        public static int GetPageSize(this HttpResponseMessage responseMessage, int defaultSize = 5)
        {
            return GetIntFromQueryString(responseMessage, "pageSize", defaultSize);
        }

        public static int GetPageIndex(this HttpResponseMessage responseMessage, int defaultIndex = 0)
        {
            return GetIntFromQueryString(responseMessage, "pageIndex", defaultIndex);
        }

        public static int GetIntFromQueryString(this HttpResponseMessage responseMessage, string key, int defaultValue)
        {
            var pair = responseMessage
                .Headers
                .FirstOrDefault(p => p.Key.Equals(key,
                    StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(pair.Value.FirstOrDefault()))
            {
                int value;
                if (int.TryParse(pair.Value.FirstOrDefault(), out value))
                {
                    return value;
                }
            }

            return defaultValue;
        }


    }

    public static class ControllerExtensions
    {
        public static string Encode(this Controller controller, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var text = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            var key = Guid.NewGuid().ToString();
            controller.Session[key] = text;

            return key;
        }

        public static T Decode<T>(this Controller controller, string key)
        {
            T result = default(T);

            try
            {
                string token = controller.Session[key] as string;

                if (!string.IsNullOrEmpty(token))
                {
                    var decryptedJson = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                    result = JsonConvert.DeserializeObject<T>(decryptedJson);

                    // Se libera este valor de la sesion:
                    controller.Session[key] = null;
                }
            }
            catch (Exception ex)
            {
                // Do Nothing
                ex.ToString();
            }

            return result;
        }




    }

    public class StatusListItem
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("text")]
        public string text { get; set; }
        [JsonProperty("icon")]
        public string icon { get; set; }
        [JsonProperty("style")]
        public string style { get; set; }
        public DocumentStatusEnum? status { get; set; }
    }

}