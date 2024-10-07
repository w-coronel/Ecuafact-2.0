using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Ecuafact.Web.Domain.Entities.API
{
    public class LoginRequestModel
    {
        public LoginRequestModel(LoginModel login)
        {
            Login = login;
        }

		[JsonProperty("login")]
		public LoginModel Login { get; set; }
    }

	public class LoginResponseModel : ErrorResponseModel
    {
        [JsonProperty("issuers")]
        public List<LoginIssuerModel> Issuers { get; set; } = new List<LoginIssuerModel>();

        [JsonProperty("client")]
		public ClientModel UserInfo { get; set; }

        public LoginIssuerModel CurrentIssuer { get; set; }        

    }

    public class LoginIssuerModel
    {
        [JsonProperty("businessName")]
        public string BusinessName { get; set; }

        [JsonProperty("clientToken")]
        public string ClientToken { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("id")]
        public string RUC { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pk")]
        public string PK { get; set; }

        [JsonProperty("userRole")]
        public string UserRole { get; set; }

        [JsonProperty("modules")]
        public string Modules { get; set; }

        [JsonProperty("subscription")]
        public SubscriptionModel Subscription { get; set; }

    }

	public class LoginModel
	{
        [Required(ErrorMessage = "Por favor, debe ingresar su número de cédula o ruc")]
        [StringLength(13, ErrorMessage = "Solo se permiten {0} caracteres.")]
        [Display(Name = "Usuario")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Por favor, debe ingresar {0}")]
        [StringLength(20, ErrorMessage = "Solo se permiten {0} caracteres.")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }

	public class ClientModel
	{
		[JsonProperty("id")]
		public string Username { get; set; }

		[JsonProperty("idUsuario")]
		public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("businessName")]
        public string BusinessName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("streetAddress")]
		public string Address { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("clientType")]
		public string ClientType { get; set; }

		[JsonProperty("clientToken")]
		public string ClientToken { get; set; }

        [JsonProperty("claveSRI")]
        public bool SRIConnected { get; set; }
        
        [JsonProperty("messages")]
        public List<string> Messages { get; set; } = new List<string>();


        public HttpPostedFileBase Avatar { get; set; }

    }

    public class SubscriptionModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("notify")]
        public bool Notify { get; set; }

        [JsonProperty("day")]
        public long Day { get; set; }

        [JsonProperty("status")]
        public SubscriptionStatusEnum? Status { get; set; }

        [JsonProperty("purchaseOrderId")]
        public long PurchaseOrderId { get; set; }

        [JsonProperty("style")]
        public string style { get {return Day <= 0 ? "text-danger" : "text-warning"; }}

        [JsonProperty("urlPurchaseOrder")]
        public string UrlPurchaseOrder { get { return Day <= 30 ? "~/payment/typePayment?purchaseorderid=" + PurchaseOrderId : "#"; } }

        [JsonProperty("issuerId")]
        public long IssuerId { get; set; }
    }
}