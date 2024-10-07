using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Ecuafact.Web.Domain.Entities.API
{
	public class RegisterSend
	{
		[JsonProperty("new_client")]
		public RegisterModel UserInfo { get; set; }
	}

	public class RegisterResult : ErrorResponseModel
	{
		[JsonProperty("client")]
		public Client Client { get; set; }
	}

	public class RegisterModel
	{
        [JsonProperty("id")]
		public string Identification { get; set; }

        [RegularExpression(@"^[A-Za-zÑñÜü\s]+$", ErrorMessage = "Debe escribir un nombre válido")]
        [JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("email")]
        [Required(ErrorMessage = "Debe especificar un correo electrónico válido")]
        [DataType(DataType.EmailAddress, ErrorMessage = "El E-mail ingresado no es válido")]
        public string Email { get; set; }

		[JsonProperty("dependants")]
		public short? Dependents { get; set; }

		[JsonProperty("phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe especificar una identificación válida")]
        [RegularExpression(@"^[0-9\s]+$", ErrorMessage = "Debe especificar solo Números")]
        [JsonProperty("user")]
		public string User { get; set; }

		[JsonProperty("password")]
		public string Password { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9,/\s]*$", ErrorMessage ="Debe escribir una dirección válida")]
		[JsonProperty("streetAddress")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe especificar una dirección válida")]
        public string Address { get; set; }

        [JsonProperty("emisor")]
        [Required]
        public bool UserIssuer { get; set; } = true;

        [JsonProperty("sriPassword")]
        public string SRIPassword { get; set; }

        public IssuerDto Issuer { get; set; }

		public UTMS UTMS { get; set; }

	}

	public class Client
	{
		[JsonProperty("businessName")]
		public string BusinessName { get; set; }
		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("clientType")]
		public string ClientType { get; set; }
		[JsonProperty("clientToken")]
		public string ClientToken { get; set; }
	}

	public class UTMS
	{
		[JsonProperty("source")]
		public string source { get; set; }
		[JsonProperty("medium")]
		public string medium { get; set; }
		[JsonProperty("campaign")]
		public string campaign { get; set; }		
	}
}