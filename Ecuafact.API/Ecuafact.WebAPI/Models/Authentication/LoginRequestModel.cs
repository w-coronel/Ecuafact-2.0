using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecuafact.WebAPI.Models.Authentication
{
    /// <summary>
    /// Modelo de Datos para el Inicio de Sesion basado en Token
    /// </summary>
    public class LoginRequestModel
    {
        /// <summary>
        /// Token de Seguridad
        /// </summary>
        [JsonProperty("securityToken")]
        public string SecurityToken { get; set; }
         
        /// <summary>
        /// Datos del Usuario
        /// </summary>
        [JsonProperty("login")]
        public LoginInfo Login { get; set; }

    }


    /// <summary>
    /// Informacion del Usuario
    /// </summary>
    public class LoginInfo
    {
        /// <summary>
        /// Nombre del Usuario
        /// </summary>
        [JsonProperty("user")]
        public string User { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Fecha de envio
        /// </summary>
        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }


    public class sriLoginResult
    {
        public string contenido { get; set; }
        public string mensaje { get; set; }
        public string codigo { get; set; }
        public string detalles { get; set; }
    }


    public class loginSRIResponse
    {
        public result result { get; set; }            
    }

    public class result
    {
        public string ExtensionData { get; set; }
        public string code { get; set; }
        public string message { get; set; }
    }

    public class loginSRIRequest
    {
        public loginSRI login { get; set; }        
    }

    public class loginSRI
    {
        public string token { get; set; }
        public string user { get; set; }
        public string password { get; set; }        
    }
}