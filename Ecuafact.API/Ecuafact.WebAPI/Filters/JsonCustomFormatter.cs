using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;

namespace Ecuafact.WebAPI.Filters
{
    /// <summary>
    /// Formateador personalizado para JSON
    /// </summary>
    public sealed class JsonCustomFormatter : JsonMediaTypeFormatter
    {
        private JsonCustomFormatter()
        {
            this.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            this.SerializerSettings.Formatting = Formatting.Indented;
            this.SerializerSettings.Culture = new CultureInfo("es-US");
        }
        
        /// <summary>
        /// 
        /// </summary>
        public static JsonCustomFormatter Default => new JsonCustomFormatter();
    }
}