using Ecuafact.WebAPI.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecuafact.WebAPI.Models.Authentication
{

    /// <summary>
    /// TokenResponse
    /// </summary>
    public class LoginResponseModel
    {
        /// <summary>
        /// Resultado
        /// </summary>
        [JsonProperty("result")]
        public LoginErrorModel Result { get; set; }

        /// <summary>
        /// Error
        /// </summary>
        [JsonProperty("error")]
        public LoginErrorModel Error { get; set; }

        /// <summary>
        /// Cliente
        /// </summary>
        [JsonProperty("client")]
        public LoginClientModel Client { get; set; }

        /// <summary>
        /// Mensajes de Notificacion de autenticacion.
        /// </summary>
        [JsonProperty("messages")]
        public List<string> Messages { get; set; } = new List<string>();

        /// <summary>
        /// Lista de Emisores Permitidos
        /// </summary>
        [JsonProperty("issuers")]
        public List<LoginIssuerModel> Issuers { get; set; }        


        /// <summary>
        /// Error
        /// </summary>
        public class LoginErrorModel
        {
            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("code")]
            public string Code { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public class RegisterClientModel 
        {
            [JsonProperty("businessName")]
            public string BusinessName { get; set; }
                
            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("id")]
            public string Username { get; set; }
              
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("streetAddress")]
            public string StreetAddress { get; set; }

            [JsonProperty("phone")]
            public string Phone { get; set; }

            [JsonProperty("appname")]
            public string AppName { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("codProducto")]
            public string codProducto { get; set; }            

            [JsonProperty("numeroPedido")]
            public string numeroPedido { get; set; }

            [JsonProperty("discountCoupon")]
            public string discountCoupon { get; set; }
            
        }

        /// <summary>
        /// Cliente
        /// </summary>
        public partial class LoginClientModel
        {
            [JsonProperty("businessName")]
            public string BusinessName { get; set; }

            [JsonProperty("clientToken")]
            public string ClientToken { get; set; }

            [JsonProperty("clientType")]
            public string ClientType { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("id")]
            public string Username { get; set; }

            [JsonProperty("idUsuario")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("streetAddress")]
            public string StreetAddress { get; set; }

            [JsonProperty("claveSRI")]
            public bool SRIConnected { get; set; }
        }

        /// <summary>
        /// Emisor
        /// </summary>
        public partial class LoginIssuerModel
        {
            [JsonProperty("id")]
            public string RUC { get; set; }

            [JsonProperty("businessName")]
            public string BusinessName { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }
             
            [JsonProperty("name")]
            public string Name { get; set; }
             
            [JsonProperty("pk")]
            public string PK { get; set; }

            [JsonProperty("userRole")]
            public UserRolEnum UserRole { get; set; }

            [JsonProperty("modules")]
            public string Modules { get; set; }

            /// <summary>
            /// Subscripcion del emisor
            /// </summary>
            [JsonProperty("subscription")]
            public SubscriptionModel Subscription { get; set; }
        }

        /// <summary>
        /// Subscripcion
        /// </summary>
        public class SubscriptionModel
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("statusMsg")]
            public string StatusMsg { get; set; }

            [JsonProperty("notify")]
            public bool Notify { get; set; }

            [JsonProperty("day")]
            public long Day { get; set; }

            [JsonProperty("status")]
            public SubscriptionStatusEnum? Status { get; set; }            

            [JsonProperty("issuerId")]
            public long IssuerId { get; set; }

            [JsonProperty("licenceId")]
            public long LicenceId { get; set; }


        }
    }
   
}